using System;
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using LL.Core.Shop;

namespace LL.Core.Tests
{
    [TestFixture]
    public class ShopStateTests
    {
        private ShopState _sut;
        private List<IDisposable> _subscriptions;

        [SetUp]
        public void SetUp()
        {
            _sut = new ShopState();
            _subscriptions = new List<IDisposable>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var sub in _subscriptions)
                sub.Dispose();
            _subscriptions.Clear();
        }

        [Test]
        public void SelectedCategory_InitialValue_IsFeatured()
        {
            Assert.AreEqual(ShopCategory.Featured, _sut.SelectedCategory.Value);
        }

        [Test]
        public void SelectedCategory_WhenSetToCurrency_EmitsCurrency()
        {
            // received[0] = Featured (initial), received[1] = Currency
            var received = new List<ShopCategory>();
            _subscriptions.Add(_sut.SelectedCategory.Subscribe(c => received.Add(c)));

            _sut.SelectedCategory.Value = ShopCategory.Currency;

            Assert.AreEqual(2, received.Count);
            Assert.AreEqual(ShopCategory.Currency, received[1]);
        }

        [Test]
        public void SelectedCategory_WhenSetToSameValue_DoesNotEmitAgain()
        {
            var received = new List<ShopCategory>();
            _subscriptions.Add(_sut.SelectedCategory.Subscribe(c => received.Add(c)));

            _sut.SelectedCategory.Value = ShopCategory.Featured; // same as initial

            Assert.AreEqual(1, received.Count);
        }

        [Test]
        public void SelectedCategory_CanCycleThroughAllCategories()
        {
            foreach (ShopCategory cat in System.Enum.GetValues(typeof(ShopCategory)))
            {
                _sut.SelectedCategory.Value = cat;
                Assert.AreEqual(cat, _sut.SelectedCategory.Value);
            }
        }
    }
}
