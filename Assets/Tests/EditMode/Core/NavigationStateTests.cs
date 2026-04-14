using System;
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using LL.Core.Navigation;

namespace LL.Core.Tests
{
    [TestFixture]
    public class NavigationStateTests
    {
        private NavigationState _sut;
        private List<IDisposable> _subscriptions;

        [SetUp]
        public void SetUp()
        {
            _sut = new NavigationState();
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
        public void CurrentTab_InitialValue_IsHome()
        {
            Assert.AreEqual(Tab.Home, _sut.CurrentTab.Value);
        }

        [Test]
        public void CurrentTab_WhenSetToShop_EmitsShop()
        {
            // ReactiveProperty emits current value on subscribe, then each change.
            // received[0] = Tab.Home (initial), received[1] = Tab.Shop
            var received = new List<Tab>();
            _subscriptions.Add(_sut.CurrentTab.Subscribe(t => received.Add(t)));

            _sut.CurrentTab.Value = Tab.Shop;

            Assert.AreEqual(2, received.Count);
            Assert.AreEqual(Tab.Shop, received[1]);
        }

        [Test]
        public void CurrentTab_WhenSetToSameValue_DoesNotEmitAgain()
        {
            // R3 ReactiveProperty uses DistinctUntilChanged — no duplicate emission
            var received = new List<Tab>();
            _subscriptions.Add(_sut.CurrentTab.Subscribe(t => received.Add(t)));

            _sut.CurrentTab.Value = Tab.Home; // same as initial

            Assert.AreEqual(1, received.Count);
        }

        [Test]
        public void CurrentTab_CanBeSetThroughAllEnumValues()
        {
            foreach (Tab tab in System.Enum.GetValues(typeof(Tab)))
            {
                _sut.CurrentTab.Value = tab;
                Assert.AreEqual(tab, _sut.CurrentTab.Value);
            }
        }
    }
}
