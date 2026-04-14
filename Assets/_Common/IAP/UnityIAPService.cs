using System;
using R3;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using VContainer.Unity;

namespace LL.Common.IAP
{
    /// <summary>
    /// Initializes Unity IAP and fires OnPurchaseSucceeded on each successful purchase.
    /// Registered as an entry point; VContainer calls Initialize() automatically before Start().
    /// Inject via IIAPService. Product IDs are supplied via IAPConfig.
    /// </summary>
    public class UnityIAPService : IIAPService, IInitializable, IDisposable, IDetailedStoreListener
    {
        private readonly IAPConfig       _config;
        private readonly Subject<string> _purchaseSucceeded = new();
        private IStoreController         _controller;

        public Observable<string> OnPurchaseSucceeded => _purchaseSucceeded;

        public UnityIAPService(IAPConfig config)
        {
            _config = config;
        }

        // ── IInitializable ────────────────────────────────────────────────────

        public void Initialize()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            foreach (var productId in _config.ProductIds)
                builder.AddProduct(productId, ProductType.Consumable);
            UnityPurchasing.Initialize(this, builder);
        }

        // ── IIAPService ───────────────────────────────────────────────────────

        public void BuyProduct(string productId)
        {
            if (_controller == null)
            {
                Debug.LogWarning("[UnityIAPService] Store not initialized yet.");
                return;
            }
            _controller.InitiatePurchase(productId);
        }

        // ── IDetailedStoreListener ────────────────────────────────────────────

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _controller = controller;
            Debug.Log("[UnityIAPService] Store initialized successfully.");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
            => Debug.LogWarning($"[UnityIAPService] Store initialization failed: {error}");

        public void OnInitializeFailed(InitializationFailureReason error, string message)
            => Debug.LogWarning($"[UnityIAPService] Store initialization failed: {error} — {message}");

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var productId = args.purchasedProduct.definition.id;
            _purchaseSucceeded.OnNext(productId);
            Debug.Log($"[UnityIAPService] Purchase complete: {productId}");
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
            => Debug.LogWarning($"[UnityIAPService] Purchase failed: {product.definition.id} — {failureReason}");

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
            => Debug.LogWarning($"[UnityIAPService] Purchase failed: {product.definition.id} — {failureDescription.reason}: {failureDescription.message}");

        // ── IDisposable ───────────────────────────────────────────────────────

        public void Dispose() => _purchaseSucceeded.Dispose();
    }
}
