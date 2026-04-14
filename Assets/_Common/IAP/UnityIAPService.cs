using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using R3;
using UnityEngine;
using UnityEngine.Purchasing;
using VContainer.Unity;

namespace LL.Common.IAP
{
    /// <summary>
    /// Initializes Unity IAP (v5) and fires OnPurchaseSucceeded on each successful purchase.
    /// Registered as an entry point; VContainer calls StartAsync() automatically.
    /// Inject via IIAPService. Product IDs are supplied via IAPConfig.
    /// </summary>
    public class UnityIAPService : IIAPService, IAsyncStartable, IDisposable
    {
        private readonly IAPConfig       _config;
        private readonly Subject<string> _purchaseSucceeded = new();
        private StoreController          _controller;

        public Observable<string> OnPurchaseSucceeded => _purchaseSucceeded;

        public UnityIAPService(IAPConfig config)
        {
            _config = config;
        }

        // ── IAsyncStartable ───────────────────────────────────────────────────

        public async Awaitable StartAsync(CancellationToken cancellation = default)
        {
            _controller = UnityIAPServices.StoreController();

            _controller.OnStoreDisconnected   += OnStoreDisconnected;
            _controller.OnProductsFetched     += OnProductsFetched;
            _controller.OnProductsFetchFailed += OnProductsFetchFailed;
            _controller.OnPurchasePending     += OnPurchasePending;
            _controller.OnPurchaseFailed      += OnPurchaseFailed;

            try
            {
                await _controller.Connect();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[UnityIAPService] Store connection failed: {e.Message}");
                return;
            }

            var products = _config.ProductIds
                .Select(id => new ProductDefinition(id, ProductType.Consumable))
                .ToList();

            _controller.FetchProducts(products);
        }

        // ── IIAPService ───────────────────────────────────────────────────────

        public void BuyProduct(string productId)
        {
            if (_controller == null)
            {
                Debug.LogWarning("[UnityIAPService] Store not initialized yet.");
                return;
            }
            _controller.PurchaseProduct(productId);
        }

        // ── StoreController event handlers ────────────────────────────────────

        private void OnStoreDisconnected(StoreConnectionFailureDescription failure)
            => Debug.LogWarning($"[UnityIAPService] Store disconnected: {failure.Message}");

        private void OnProductsFetched(List<Product> products)
        {
            Debug.Log($"[UnityIAPService] Products fetched: {products.Count}. Fetching existing purchases.");
            _controller.FetchPurchases();
        }

        private void OnProductsFetchFailed(ProductFetchFailed failure)
            => Debug.LogWarning($"[UnityIAPService] Products fetch failed: {failure.FailureReason}");

        private void OnPurchasePending(PendingOrder order)
        {
            _controller.ConfirmPurchase(order);

            foreach (var item in order.CartOrdered.Items())
            {
                var productId = item.Product.definition.id;
                _purchaseSucceeded.OnNext(productId);
                Debug.Log($"[UnityIAPService] Purchase complete: {productId}");
            }
        }

        private void OnPurchaseFailed(FailedOrder order)
        {
            foreach (var item in order.CartOrdered.Items())
                Debug.LogWarning($"[UnityIAPService] Purchase failed: {item.Product.definition.id} — {order.FailureReason}: {order.Details}");
        }

        // ── IDisposable ───────────────────────────────────────────────────────

        public void Dispose()
        {
            if (_controller != null)
            {
                _controller.OnStoreDisconnected   -= OnStoreDisconnected;
                _controller.OnProductsFetched     -= OnProductsFetched;
                _controller.OnProductsFetchFailed -= OnProductsFetchFailed;
                _controller.OnPurchasePending     -= OnPurchasePending;
                _controller.OnPurchaseFailed      -= OnPurchaseFailed;
            }
            _purchaseSucceeded.Dispose();
        }
    }
}
