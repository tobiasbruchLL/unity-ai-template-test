using R3;

namespace LL.Common.IAP
{
    public interface IIAPService
    {
        Observable<string> OnPurchaseSucceeded { get; }
        void BuyProduct(string productId);
    }
}
