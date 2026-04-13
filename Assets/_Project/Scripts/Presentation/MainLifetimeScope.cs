using UnityEngine;
using VContainer;
using VContainer.Unity;
using LL.Core.Navigation;
using LL.Presentation.Navigation;

public class MainLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<NavigationState>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<NavigationView>();
        builder.RegisterEntryPoint<NavigationPresenter>();
    }
}
