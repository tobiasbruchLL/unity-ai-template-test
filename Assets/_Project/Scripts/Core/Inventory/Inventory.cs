using System.Collections.Generic;
using BreakInfinity;
using R3;

namespace LL.Core.Inventory
{
    public class Inventory
    {
        private readonly Dictionary<string, ReactiveProperty<BigDouble>> _amounts = new();

        /// <summary>
        /// Returns a reactive property tracking the current amount for the given item.
        /// Emits 0 if the item has never been added.
        /// </summary>
        public IReadOnlyReactiveProperty<BigDouble> GetAmount(string itemId)
            => GetOrCreate(itemId);

        /// <summary>
        /// Returns an observable that emits whenever the affordability of the given item changes.
        /// </summary>
        public IObservable<bool> CanSpend(InventoryItem item)
            => GetOrCreate(item.Id).Select(amount => amount >= item.Amount);

        /// <summary>
        /// Adds the item's amount to the inventory.
        /// </summary>
        public void Add(InventoryItem item)
            => GetOrCreate(item.Id).Value += item.Amount;

        /// <summary>
        /// Subtracts the item's amount if the player can afford it.
        /// Returns false without mutating state if the current amount is insufficient.
        /// </summary>
        public bool TrySpend(InventoryItem item)
        {
            var prop = GetOrCreate(item.Id);
            if (prop.Value < item.Amount) return false;
            prop.Value -= item.Amount;
            return true;
        }

        private ReactiveProperty<BigDouble> GetOrCreate(string itemId)
        {
            if (!_amounts.TryGetValue(itemId, out var prop))
            {
                prop = new ReactiveProperty<BigDouble>(BigDouble.Zero);
                _amounts[itemId] = prop;
            }
            return prop;
        }
    }
}
