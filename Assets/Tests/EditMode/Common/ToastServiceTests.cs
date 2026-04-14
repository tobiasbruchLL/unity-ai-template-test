using System;
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using LL.Common.Toast;

namespace LL.Common.Tests
{
    [TestFixture]
    public class ToastServiceTests
    {
        private ToastService _sut;
        private List<IDisposable> _subscriptions;

        [SetUp]
        public void SetUp()
        {
            _sut = new ToastService();
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
        public void Show_WhenCalled_EmitsMessageOnOnToastRequested()
        {
            var received = new List<string>();
            _subscriptions.Add(_sut.OnToastRequested.Subscribe(msg => received.Add(msg)));

            _sut.Show("hello");

            Assert.AreEqual(1, received.Count);
            Assert.AreEqual("hello", received[0]);
        }

        [Test]
        public void Show_CalledMultipleTimes_EmitsEachMessageInOrder()
        {
            var received = new List<string>();
            _subscriptions.Add(_sut.OnToastRequested.Subscribe(msg => received.Add(msg)));

            _sut.Show("first");
            _sut.Show("second");
            _sut.Show("third");

            Assert.AreEqual(new[] { "first", "second", "third" }, received);
        }

        [Test]
        public void Show_WithEmptyString_EmitsEmptyString()
        {
            var received = new List<string>();
            _subscriptions.Add(_sut.OnToastRequested.Subscribe(msg => received.Add(msg)));

            _sut.Show(string.Empty);

            Assert.AreEqual(1, received.Count);
            Assert.AreEqual(string.Empty, received[0]);
        }

        [Test]
        public void OnToastRequested_BeforeAnyShow_EmitsNoValues()
        {
            var received = new List<string>();
            _subscriptions.Add(_sut.OnToastRequested.Subscribe(msg => received.Add(msg)));

            Assert.AreEqual(0, received.Count);
        }

        [Test]
        public void OnToastRequested_MultipleSubscribers_BothReceiveMessage()
        {
            var receivedA = new List<string>();
            var receivedB = new List<string>();
            _subscriptions.Add(_sut.OnToastRequested.Subscribe(msg => receivedA.Add(msg)));
            _subscriptions.Add(_sut.OnToastRequested.Subscribe(msg => receivedB.Add(msg)));

            _sut.Show("broadcast");

            Assert.AreEqual(1, receivedA.Count, "subscriber A should receive the message");
            Assert.AreEqual(1, receivedB.Count, "subscriber B should receive the message");
            Assert.AreEqual("broadcast", receivedA[0]);
            Assert.AreEqual("broadcast", receivedB[0]);
        }

        [Test]
        public void OnToastRequested_AfterSubscriptionDisposed_ReceivesNoFurtherMessages()
        {
            var received = new List<string>();
            var sub = _sut.OnToastRequested.Subscribe(msg => received.Add(msg));
            sub.Dispose();

            _sut.Show("should not arrive");

            Assert.AreEqual(0, received.Count);
        }
    }
}
