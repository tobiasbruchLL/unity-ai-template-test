using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using R3;

namespace LL.Core.Inventory
{
    public class InventoryService
    {
        private readonly Dictionary<string, ReactiveProperty<BigDouble>> _amounts = new();
        private readonly Dictionary<string, BigDouble> _aggregateBuffer = new();
        private readonly Subject<string> _spendFailed = new();

        /// <summary>
        /// Fires whenever a TrySpend call fails due to insufficient funds.
        /// Emits the item ID that could not be afforded.
        /// </summary>
        public Observable<string> OnSpendFailed => _spendFailed;

        /// <summary>
        /// Returns a reactive property tracking the current amount for the given item.
        /// Emits 0 if the item has never been added.
        /// </summary>
        public ReadOnlyReactiveProperty<BigDouble> GetAmount(string itemId)
            => GetOrCreate(itemId);

        /// <summary>
        /// Returns an observable that is true whenever the player can afford the item.
        /// </summary>
        public Observable<bool> CanSpend(InventoryItem item)
            => GetOrCreate(item.Id).Select(amount => amount >= item.Amount);

        /// <summary>
        /// Returns an observable that is true whenever the player can afford ALL of the supplied
        /// items simultaneously. Duplicate IDs are summed before comparison.
        /// </summary>
        public Observable<bool> CanSpend(IEnumerable<InventoryItem> items)
        {
            var totals = Aggregate(items);

            if (totals.Count == 0)
                return Observable.Return(true);

            if (totals.Count == 1)
            {
                var kvp = totals.First();
                return GetOrCreate(kvp.Key).Select(amount => amount >= kvp.Value);
            }

            var streams = totals
                .Select(kvp => GetOrCreate(kvp.Key).Select(amount => amount >= kvp.Value))
                .ToArray();

            return Observable.CombineLatest<bool>(streams).Select(bools => System.Array.TrueForAll(bools, b => b));
        }

        /// <inheritdoc cref="CanSpend(IEnumerable{InventoryItem})"/>
        public Observable<bool> CanSpend(params InventoryItem[] items) => CanSpend((IEnumerable<InventoryItem>)items);

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
            if (prop.Value < item.Amount)
            {
                _spendFailed.OnNext(item.Id);
                return false;
            }
            prop.Value -= item.Amount;
            return true;
        }

        /// <summary>
        /// Atomically attempts to spend ALL of the supplied items.
        /// If ANY item is unaffordable the inventory is unchanged and false is returned.
        /// Duplicate IDs in the input are summed before the check.
        /// </summary>
        public bool TrySpend(IEnumerable<InventoryItem> items)
        {
            var totals = Aggregate(items);

            foreach (var kvp in totals)
                if (GetOrCreate(kvp.Key).Value < kvp.Value)
                {
                    _spendFailed.OnNext(kvp.Key);
                    return false;
                }

            foreach (var kvp in totals)
                GetOrCreate(kvp.Key).Value -= kvp.Value;

            return true;
        }

        /// <inheritdoc cref="TrySpend(IEnumerable{InventoryItem})"/>
        public bool TrySpend(params InventoryItem[] items) => TrySpend((IEnumerable<InventoryItem>)items);

        private ReactiveProperty<BigDouble> GetOrCreate(string itemId)
        {
            if (!_amounts.TryGetValue(itemId, out var prop))
            {
                prop = new ReactiveProperty<BigDouble>(BigDouble.Zero);
                _amounts[itemId] = prop;
            }
            return prop;
        }

        private Dictionary<string, BigDouble> Aggregate(IEnumerable<InventoryItem> items)
        {
            _aggregateBuffer.Clear();
            foreach (var item in items)
            {
                _aggregateBuffer.TryGetValue(item.Id, out var existing);
                _aggregateBuffer[item.Id] = existing + item.Amount;
            }
            return _aggregateBuffer;
        }
    }
}
