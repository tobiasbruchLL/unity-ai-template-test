namespace LL.Core.Shop
{
    public static class CoinPackCatalog
    {
        public static readonly CoinPackDefinition[] Packs =
        {
            new CoinPackDefinition(coins:  1_000, gemCost:     50),
            new CoinPackDefinition(coins:  5_000, gemCost:    200),
            new CoinPackDefinition(coins: 15_000, gemCost:    500),
            new CoinPackDefinition(coins: 50_000, gemCost:  1_500),
        };
    }
}
