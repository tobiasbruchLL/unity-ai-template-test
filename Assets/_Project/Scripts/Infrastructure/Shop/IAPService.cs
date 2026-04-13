using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using BreakInfinity;
using LL.Core.Inventory;
using LL.Core.Shop;

namespace LL.Infrastructure.Shop
{
    /// <summary>
    /// Initializes Unity IAP and handles purchase callbacks.
    /// Registered as a singleton in MainLifetimeScope; inject via IIAPService.
    /// Call Initialize() once from ShopPresenter.Start().
    /// </summary>
    public class IAPService : IIAPService, IDetailedStoreListener
    {
        private readonly Inventory _inventory;
        private IStoreController _controller;
        private bool _initialized;

        // Maps store product ID → gem amount granted on successful purchase
        private static readonly Dictionary<string, int> GemAmounts = new()
        {
            { IAPProductIds.Gems100,    100   },
            { IAPProductIds.Gems500,    500   },
            { IAPProductIds.Gems1000,   1_000 },
            { IAPProductIds.Gems2500,   2_500 },
            { IAPProductIds.Gems5000,   5_000 },
            { IAPProductIds.Gems10000, 10_000 },
        };

        public IAPService(Inventory inventory)
        {
            _inventory = inventory;
        }

        // ── IIAPService ───────────────────────────────────────────────────────

        public void Initialize()
        {
            if (_initialized)
                throw new InvalidOperationException("[IAPService] Initialize() called more than once.");

            _initialized = true;
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            foreach (var productId in GemAmounts.Keys)
                builder.AddProduct(productId, ProductType.Consumable);
            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyProduct(string productId)
        {
            if (_controller == null)
            {
                Debug.LogWarning("[IAPService] Store not initialized yet.");
                return;
            }
            _controller.InitiatePurchase(productId);
        }

        // ── IDetailedStoreListener ────────────────────────────────────────────

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _controller = controller;
            Debug.Log("[IAPService] Store initialized successfully.");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
            => Debug.LogWarning($"[IAPService] Store initialization failed: {error}");

        public void OnInitializeFailed(InitializationFailureReason error, string message)
            => Debug.LogWarning($"[IAPService] Store initialization failed: {error} — {message}");

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var productId = args.purchasedProduct.definition.id;
            if (GemAmounts.TryGetValue(productId, out var gemAmount))
            {
                _inventory.Add(new InventoryItem(InventoryIds.Gems, new BigDouble(gemAmount)));
                Debug.Log($"[IAPService] Purchase complete: {productId} → +{gemAmount} gems.");
            }
            else
            {
                Debug.LogWarning($"[IAPService] ProcessPurchase: unknown product '{productId}'.");
            }
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
            => Debug.LogWarning($"[IAPService] Purchase failed: {product.definition.id} — {failureReason}");

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
            => Debug.LogWarning($"[IAPService] Purchase failed: {product.definition.id} — {failureDescription.reason}: {failureDescription.message}");
    }
}
