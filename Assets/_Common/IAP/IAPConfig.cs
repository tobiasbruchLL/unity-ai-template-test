namespace LL.Common.IAP
{
    public class IAPConfig
    {
        public readonly string[] ProductIds;

        public IAPConfig(params string[] productIds)
        {
            ProductIds = productIds;
        }
    }
}
