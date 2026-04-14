using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;
using LL.Common.IAP;
using LL.Core.Inventory;
using LL.Core.Navigation;
using LL.Core.Shop;
using LL.Presentation.Navigation;
using LL.Presentation.Shop;

public class MainLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<UIDocument>();

        // Inventory
        builder.Register<InventoryService>(Lifetime.Singleton);

        // IAP
        builder.RegisterInstance(new IAPConfig(IAPProductIds.AllIds));
        builder.RegisterEntryPoint<UnityIAPService>().As<IIAPService>();
        builder.RegisterEntryPoint<GemPackPurchaseHandler>();

        // Navigation
        builder.Register<NavigationState>(Lifetime.Singleton);
        builder.RegisterEntryPoint<NavigationView>()
            .AsSelf();
        builder.RegisterEntryPoint<NavigationPresenter>();

        // Shop
        builder.Register<ShopState>(Lifetime.Singleton);
        builder.RegisterEntryPoint<ShopView>()
            .AsSelf();
        builder.RegisterEntryPoint<ShopPresenter>();
    }
}
