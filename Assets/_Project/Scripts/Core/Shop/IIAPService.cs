namespace LL.Core.Shop
{
    public interface IIAPService
    {
        void Initialize();
        void BuyProduct(string productId);
    }
}
