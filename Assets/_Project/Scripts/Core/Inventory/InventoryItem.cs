using BreakInfinity;

namespace LL.Core.Inventory
{
    public readonly struct InventoryItem
    {
        public string Id { get; }
        public BigDouble Amount { get; }

        public InventoryItem(string id, BigDouble amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}
