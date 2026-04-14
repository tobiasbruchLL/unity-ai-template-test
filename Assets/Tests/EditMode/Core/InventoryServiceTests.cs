using System;
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using BreakInfinity;
using LL.Core.Inventory;

namespace LL.Core.Tests
{
    [TestFixture]
    public class InventoryServiceTests
    {
        private InventoryService _sut;
        private List<IDisposable> _subscriptions;

        [SetUp]
        public void SetUp()
        {
            _sut = new InventoryService();
            _subscriptions = new List<IDisposable>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var sub in _subscriptions)
                sub.Dispose();
            _subscriptions.Clear();
        }

        // ── GetAmount ────────────────────────────────────────────────────────

        [Test]
        public void GetAmount_UnknownItem_ReturnsZero()
        {
            Assert.AreEqual(BigDouble.Zero, _sut.GetAmount("unknown").CurrentValue);
        }

        [Test]
        public void GetAmount_SameIdTwice_ReturnsSameInstance()
        {
            var a = _sut.GetAmount("gems");
            var b = _sut.GetAmount("gems");
            Assert.AreSame(a, b);
        }

        // ── Add ──────────────────────────────────────────────────────────────

        [Test]
        public void Add_SingleItem_IncreasesAmountByItemAmount()
        {
            _sut.Add(new InventoryItem("coins", new BigDouble(100)));

            Assert.AreEqual(new BigDouble(100), _sut.GetAmount("coins").CurrentValue);
        }

        [Test]
        public void Add_SameItemTwice_AccumulatesAmount()
        {
            var item = new InventoryItem("gems", new BigDouble(50));
            _sut.Add(item);
            _sut.Add(item);

            Assert.AreEqual(new BigDouble(100), _sut.GetAmount("gems").CurrentValue);
        }

        [Test]
        public void Add_EmitsUpdatedValueOnReactiveProperty()
        {
            // ReactiveProperty emits its current value synchronously on subscribe,
            // so received[0] = initial 0, received[1] = value after Add.
            var received = new List<BigDouble>();
            _subscriptions.Add(_sut.GetAmount("gems").Subscribe(v => received.Add(v)));

            _sut.Add(new InventoryItem("gems", new BigDouble(200)));

            Assert.AreEqual(2, received.Count);
            Assert.AreEqual(new BigDouble(200), received[1]);
        }

        // ── TrySpend (single item) ───────────────────────────────────────────

        [Test]
        public void TrySpend_SingleItem_WhenSufficientFunds_ReturnsTrueAndDeductsAmount()
        {
            _sut.Add(new InventoryItem("gems", new BigDouble(500)));

            var result = _sut.TrySpend(new InventoryItem("gems", new BigDouble(200)));

            Assert.IsTrue(result);
            Assert.AreEqual(new BigDouble(300), _sut.GetAmount("gems").CurrentValue);
        }

        [Test]
        public void TrySpend_SingleItem_WhenInsufficientFunds_ReturnsFalseAndLeavesAmountUnchanged()
        {
            _sut.Add(new InventoryItem("gems", new BigDouble(100)));

            var result = _sut.TrySpend(new InventoryItem("gems", new BigDouble(200)));

            Assert.IsFalse(result);
            Assert.AreEqual(new BigDouble(100), _sut.GetAmount("gems").CurrentValue);
        }

        [Test]
        public void TrySpend_SingleItem_WhenInsufficientFunds_EmitsItemIdOnOnSpendFailed()
        {
            var failedIds = new List<string>();
            _subscriptions.Add(_sut.OnSpendFailed.Subscribe(id => failedIds.Add(id)));

            _sut.TrySpend(new InventoryItem("gems", new BigDouble(999)));

            Assert.AreEqual(1, failedIds.Count);
            Assert.AreEqual("gems", failedIds[0]);
        }

        [Test]
        public void TrySpend_SingleItem_WhenSuccessful_DoesNotEmitOnSpendFailed()
        {
            _sut.Add(new InventoryItem("gems", new BigDouble(500)));
            var failedIds = new List<string>();
            _subscriptions.Add(_sut.OnSpendFailed.Subscribe(id => failedIds.Add(id)));

            _sut.TrySpend(new InventoryItem("gems", new BigDouble(100)));

            Assert.AreEqual(0, failedIds.Count);
        }

        [Test]
        public void TrySpend_SingleItem_ExactAmount_ReducesBalanceToZero()
        {
            _sut.Add(new InventoryItem("coins", new BigDouble(300)));
            _sut.TrySpend(new InventoryItem("coins", new BigDouble(300)));

            Assert.AreEqual(BigDouble.Zero, _sut.GetAmount("coins").CurrentValue);
        }

        // ── TrySpend (IEnumerable) ───────────────────────────────────────────

        [Test]
        public void TrySpend_MultipleItems_WhenAllAffordable_ReturnsTrueAndDeductsAll()
        {
            _sut.Add(new InventoryItem("gems",  new BigDouble(1000)));
            _sut.Add(new InventoryItem("coins", new BigDouble(500)));

            var result = _sut.TrySpend(new[]
            {
                new InventoryItem("gems",  new BigDouble(300)),
                new InventoryItem("coins", new BigDouble(200)),
            });

            Assert.IsTrue(result);
            Assert.AreEqual(new BigDouble(700), _sut.GetAmount("gems").CurrentValue);
            Assert.AreEqual(new BigDouble(300), _sut.GetAmount("coins").CurrentValue);
        }

        [Test]
        public void TrySpend_MultipleItems_WhenOneUnaffordable_ReturnsFalseAndLeavesAllUnchanged()
        {
            _sut.Add(new InventoryItem("gems",  new BigDouble(1000)));
            _sut.Add(new InventoryItem("coins", new BigDouble(50)));

            var result = _sut.TrySpend(new[]
            {
                new InventoryItem("gems",  new BigDouble(300)),
                new InventoryItem("coins", new BigDouble(200)),
            });

            Assert.IsFalse(result);
            Assert.AreEqual(new BigDouble(1000), _sut.GetAmount("gems").CurrentValue);
            Assert.AreEqual(new BigDouble(50),   _sut.GetAmount("coins").CurrentValue);
        }

        [Test]
        public void TrySpend_MultipleItems_DuplicateIdsAreSummedBeforeCheck()
        {
            // 300 gems, trying to spend 200+200=400 — should fail
            _sut.Add(new InventoryItem("gems", new BigDouble(300)));

            var result = _sut.TrySpend(new[]
            {
                new InventoryItem("gems", new BigDouble(200)),
                new InventoryItem("gems", new BigDouble(200)),
            });

            Assert.IsFalse(result);
            Assert.AreEqual(new BigDouble(300), _sut.GetAmount("gems").CurrentValue);
        }

        [Test]
        public void TrySpend_EmptyList_ReturnsTrueWithNoStateChange()
        {
            _sut.Add(new InventoryItem("gems", new BigDouble(100)));

            var result = _sut.TrySpend(new InventoryItem[] { });

            Assert.IsTrue(result);
            Assert.AreEqual(new BigDouble(100), _sut.GetAmount("gems").CurrentValue);
        }

        // ── CanSpend (single item) ────────────────────────────────────────────

        [Test]
        public void CanSpend_SingleItem_WhenAmountIsZero_EmitsFalseOnSubscribe()
        {
            var results = new List<bool>();
            _subscriptions.Add(
                _sut.CanSpend(new InventoryItem("gems", new BigDouble(100)))
                    .Subscribe(b => results.Add(b)));

            Assert.AreEqual(1, results.Count);
            Assert.IsFalse(results[0]);
        }

        [Test]
        public void CanSpend_SingleItem_EmitsTrueAfterSufficientAmountAdded()
        {
            var results = new List<bool>();
            _subscriptions.Add(
                _sut.CanSpend(new InventoryItem("gems", new BigDouble(100)))
                    .Subscribe(b => results.Add(b)));

            _sut.Add(new InventoryItem("gems", new BigDouble(100)));

            // results[0] = false (initial, 0 < 100), results[1] = true (100 >= 100)
            Assert.AreEqual(2, results.Count);
            Assert.IsFalse(results[0]);
            Assert.IsTrue(results[1]);
        }

        [Test]
        public void CanSpend_MultipleItems_ReturnsTrueOnlyWhenAllAffordable()
        {
            var results = new List<bool>();
            _subscriptions.Add(_sut.CanSpend(new[]
            {
                new InventoryItem("gems",  new BigDouble(100)),
                new InventoryItem("coins", new BigDouble(200)),
            }).Subscribe(b => results.Add(b)));

            _sut.Add(new InventoryItem("gems",  new BigDouble(200)));
            _sut.Add(new InventoryItem("coins", new BigDouble(300)));

            Assert.IsTrue(results[results.Count - 1]);
        }

        [Test]
        public void CanSpend_EmptyList_EmitsTrueImmediately()
        {
            var results = new List<bool>();
            _subscriptions.Add(_sut.CanSpend(new InventoryItem[] { }).Subscribe(b => results.Add(b)));

            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0]);
        }
    }
}
