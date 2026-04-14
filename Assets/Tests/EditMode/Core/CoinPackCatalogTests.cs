using NUnit.Framework;
using LL.Core.Shop;

namespace LL.Core.Tests
{
    [TestFixture]
    public class CoinPackCatalogTests
    {
        [Test]
        public void Packs_HasExactlyFourEntries()
        {
            Assert.AreEqual(4, CoinPackCatalog.Packs.Length);
        }

        [Test]
        public void Packs_AllEntriesHavePositiveCoinsAndGemCost()
        {
            foreach (var pack in CoinPackCatalog.Packs)
            {
                Assert.Greater(pack.Coins,   0, "All packs must have positive coin value");
                Assert.Greater(pack.GemCost, 0, "All packs must have positive gem cost");
            }
        }

        [Test]
        public void Packs_AreOrderedByAscendingCoinValue()
        {
            for (int i = 1; i < CoinPackCatalog.Packs.Length; i++)
            {
                Assert.Less(
                    CoinPackCatalog.Packs[i - 1].Coins,
                    CoinPackCatalog.Packs[i].Coins,
                    $"Pack at index {i - 1} should have fewer coins than pack at index {i}");
            }
        }

        [Test]
        public void Packs_GemCostIncreasesWithCoinValue()
        {
            for (int i = 1; i < CoinPackCatalog.Packs.Length; i++)
            {
                Assert.Less(
                    CoinPackCatalog.Packs[i - 1].GemCost,
                    CoinPackCatalog.Packs[i].GemCost,
                    $"Pack at index {i - 1} should cost fewer gems than pack at index {i}");
            }
        }

        [TestCase(0,  1_000,    50)]
        [TestCase(1,  5_000,   200)]
        [TestCase(2, 15_000,   500)]
        [TestCase(3, 50_000, 1_500)]
        public void Packs_EntryAtIndex_HasExpectedCoinsAndGemCost(int index, int expectedCoins, int expectedGemCost)
        {
            Assert.AreEqual(expectedCoins,   CoinPackCatalog.Packs[index].Coins);
            Assert.AreEqual(expectedGemCost, CoinPackCatalog.Packs[index].GemCost);
        }
    }
}
