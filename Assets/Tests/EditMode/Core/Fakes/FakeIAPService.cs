using System;
using System.Collections.Generic;
using R3;
using LL.Common.IAP;

namespace LL.Core.Tests.Fakes
{
    /// <summary>
    /// Hand-written test double for IIAPService.
    /// Call SimulatePurchase(productId) from a test to fire OnPurchaseSucceeded.
    /// </summary>
    public class FakeIAPService : IIAPService, IDisposable
    {
        private readonly Subject<string> _purchaseSubject = new Subject<string>();

        public Observable<string> OnPurchaseSucceeded => _purchaseSubject;

        public List<string> BuyProductCalls { get; } = new List<string>();

        public void BuyProduct(string productId)
        {
            BuyProductCalls.Add(productId);
        }

        /// <summary>Triggers a successful purchase event as if the store confirmed it.</summary>
        public void SimulatePurchase(string productId)
        {
            _purchaseSubject.OnNext(productId);
        }

        public void Dispose()
        {
            _purchaseSubject.Dispose();
        }
    }
}
