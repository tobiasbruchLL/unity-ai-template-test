namespace LL.Core.Shop
{
    public readonly struct CoinPackDefinition
    {
        public int Coins   { get; }
        public int GemCost { get; }

        public CoinPackDefinition(int coins, int gemCost)
        {
            Coins   = coins;
            GemCost = gemCost;
        }
    }
}
