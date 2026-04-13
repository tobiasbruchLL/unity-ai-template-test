using R3;

namespace LL.Core.Shop
{
    public class ShopState
    {
        public ReactiveProperty<ShopCategory> SelectedCategory { get; }
            = new ReactiveProperty<ShopCategory>(ShopCategory.Featured);
    }
}
