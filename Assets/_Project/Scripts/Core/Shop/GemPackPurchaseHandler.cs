using System;
using System.Collections.Generic;
using BreakInfinity;
using R3;
using VContainer.Unity;
using LL.Common.IAP;
using LL.Core.Inventory;

namespace LL.Core.Shop
{
    /// <summary>
    /// Listens to IAP purchase events and grants the corresponding gem reward to the player's inventory.
    /// Keeps reward logic separate from the IAP service so UnityIAPService stays project-agnostic.
    /// </summary>
    public class GemPackPurchaseHandler : IStartable, IDisposable
    {
        private readonly IIAPService     _iapService;
        private readonly InventoryService _inventory;
        private IDisposable               _subscription;

        private static readonly Dictionary<string, int> GemAmounts = new()
        {
            { IAPProductIds.Gems100,    100   },
            { IAPProductIds.Gems500,    500   },
            { IAPProductIds.Gems1000,   1_000 },
            { IAPProductIds.Gems2500,   2_500 },
            { IAPProductIds.Gems5000,   5_000 },
            { IAPProductIds.Gems10000, 10_000 },
        };

        public GemPackPurchaseHandler(IIAPService iapService, InventoryService inventory)
        {
            _iapService = iapService;
            _inventory  = inventory;
        }

        public void Start()
        {
            _subscription = _iapService.OnPurchaseSucceeded.Subscribe(OnPurchaseSucceeded);
        }

        private void OnPurchaseSucceeded(string productId)
        {
            if (GemAmounts.TryGetValue(productId, out var gemAmount))
                _inventory.Add(new InventoryItem(InventoryIds.Gems, new BigDouble(gemAmount)));
        }

        public void Dispose() => _subscription?.Dispose();
    }
}
