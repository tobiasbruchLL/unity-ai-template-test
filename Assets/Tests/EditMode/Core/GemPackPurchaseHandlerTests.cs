using NUnit.Framework;
using BreakInfinity;
using LL.Core.Inventory;
using LL.Core.Shop;
using LL.Core.Tests.Fakes;

namespace LL.Core.Tests
{
    /// <summary>
    /// Tests for GemPackPurchaseHandler.
    /// IStartable.Start() is called manually in SetUp — no VContainer runtime needed.
    /// </summary>
    [TestFixture]
    public class GemPackPurchaseHandlerTests
    {
        private FakeIAPService _fakeIap;
        private InventoryService _inventory;
        private GemPackPurchaseHandler _sut;

        [SetUp]
        public void SetUp()
        {
            _fakeIap   = new FakeIAPService();
            _inventory = new InventoryService();
            _sut       = new GemPackPurchaseHandler(_fakeIap, _inventory);
            _sut.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _sut.Dispose();
            _fakeIap.Dispose();
        }

        // ── Reward per product ID ─────────────────────────────────────────────

        [Test]
        public void OnPurchaseSucceeded_Gems100_AddsOneHundredGems()
        {
            _fakeIap.SimulatePurchase(IAPProductIds.Gems100);

            Assert.AreEqual(new BigDouble(100), _inventory.GetAmount(InventoryIds.Gems).CurrentValue);
        }

        [Test]
        public void OnPurchaseSucceeded_Gems500_AddsFiveHundredGems()
        {
            _fakeIap.SimulatePurchase(IAPProductIds.Gems500);

            Assert.AreEqual(new BigDouble(500), _inventory.GetAmount(InventoryIds.Gems).CurrentValue);
        }

        [Test]
        public void OnPurchaseSucceeded_Gems1000_AddsOneThousandGems()
        {
            _fakeIap.SimulatePurchase(IAPProductIds.Gems1000);

            Assert.AreEqual(new BigDouble(1_000), _inventory.GetAmount(InventoryIds.Gems).CurrentValue);
        }

        [Test]
        public void OnPurchaseSucceeded_Gems2500_AddsTwoThousandFiveHundredGems()
        {
            _fakeIap.SimulatePurchase(IAPProductIds.Gems2500);

            Assert.AreEqual(new BigDouble(2_500), _inventory.GetAmount(InventoryIds.Gems).CurrentValue);
        }

        [Test]
        public void OnPurchaseSucceeded_Gems5000_AddsFiveThousandGems()
        {
            _fakeIap.SimulatePurchase(IAPProductIds.Gems5000);

            Assert.AreEqual(new BigDouble(5_000), _inventory.GetAmount(InventoryIds.Gems).CurrentValue);
        }

        [Test]
        public void OnPurchaseSucceeded_Gems10000_AddsTenThousandGems()
        {
            _fakeIap.SimulatePurchase(IAPProductIds.Gems10000);

            Assert.AreEqual(new BigDouble(10_000), _inventory.GetAmount(InventoryIds.Gems).CurrentValue);
        }

        [Test]
        public void OnPurchaseSucceeded_UnknownProductId_DoesNotModifyInventory()
        {
            _fakeIap.SimulatePurchase("com.company.unknown.product");

            Assert.AreEqual(BigDouble.Zero, _inventory.GetAmount(InventoryIds.Gems).CurrentValue);
        }

        [Test]
        public void OnPurchaseSucceeded_CalledTwice_AccumulatesGems()
        {
            _fakeIap.SimulatePurchase(IAPProductIds.Gems100);
            _fakeIap.SimulatePurchase(IAPProductIds.Gems100);

            Assert.AreEqual(new BigDouble(200), _inventory.GetAmount(InventoryIds.Gems).CurrentValue);
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────

        [Test]
        public void AfterDispose_PurchaseEventsAreIgnored()
        {
            _sut.Dispose();

            _fakeIap.SimulatePurchase(IAPProductIds.Gems100);

            Assert.AreEqual(BigDouble.Zero, _inventory.GetAmount(InventoryIds.Gems).CurrentValue);
        }

        [Test]
        public void BeforeStart_PurchaseEventsAreIgnored()
        {
            var freshIap     = new FakeIAPService();
            var freshInv     = new InventoryService();
            var freshHandler = new GemPackPurchaseHandler(freshIap, freshInv);
            // Start() intentionally NOT called

            try
            {
                freshIap.SimulatePurchase(IAPProductIds.Gems100);

                Assert.AreEqual(BigDouble.Zero, freshInv.GetAmount(InventoryIds.Gems).CurrentValue);
            }
            finally
            {
                freshHandler.Dispose();
                freshIap.Dispose();
            }
        }
    }
}
