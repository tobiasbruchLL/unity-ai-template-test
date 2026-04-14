# VContainer Patterns

## LifetimeScope setup

Each scene gets one `LifetimeScope` subclass on a dedicated GameObject. The `Configure` override is the only place dependencies are registered — never register elsewhere.

```csharp
public class MainLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<NavigationState>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<NavigationView>();
        builder.RegisterEntryPoint<NavigationPresenter>();
    }
}
```

## Registering MonoBehaviours

**Option A — `RegisterComponentInHierarchy<T>()`**
VContainer searches the scene hierarchy for an existing component of that type. Simple, but produces a cryptic runtime `VContainerException` if the component is missing from the scene.

**Option B — `[SerializeField]` + `RegisterComponent(instance)`**
More explicit: the LifetimeScope holds a serialized reference and the user drags the component in the Inspector. Missing wiring shows up as a null Inspector slot rather than a runtime exception.

```csharp
[SerializeField] NavigationView navigationView;

protected override void Configure(IContainerBuilder builder)
{
    builder.RegisterComponent(navigationView);
}
```

Prefer Option B for components that live on a specific known GameObject. Use Option A when the component could be anywhere in the hierarchy and its location isn't fixed.

## Entry points

`RegisterEntryPoint<T>()` automatically handles `IInitializable`, `IStartable`, `ITickable`, and `IDisposable`. VContainer calls them in order — `Initialize()` first, then `Start()` — and `Dispose()` on scene teardown.

```csharp
public class NavigationPresenter : IStartable, IDisposable { ... }

builder.RegisterEntryPoint<NavigationPresenter>();
```

Use `IInitializable` for services that need to run setup logic once before any `IStartable` runs:

```csharp
public class UnityIAPService : IIAPService, IInitializable, IDisposable
{
    public void Initialize() { /* kicks off async Unity IAP init */ }
    public void Dispose()    { /* clean up subjects / handles */ }
}

// Register as entry point and expose via interface:
builder.RegisterEntryPoint<UnityIAPService>().As<IIAPService>();
```

## Dual registration: interface + concrete type

When a service needs to be resolved **both** as an interface (by most consumers) and as its concrete type (by an internal component that needs access to members not on the interface), chain `.As<IFoo>().AsSelf()`:

```csharp
// ToastService implements IToastService but also exposes OnToastRequested (Observable)
// which ToastPresenter needs directly.
builder.Register<ToastService>(Lifetime.Singleton).As<IToastService>().AsSelf();
```

Without `.AsSelf()`, a constructor that declares `ToastService` (concrete) as a parameter will fail at runtime with a `VContainerException` — the container only knows the interface binding. With both registrations, `IToastService` resolves for general consumers and `ToastService` resolves for internal wiring.

## Constructor injection

VContainer resolves constructor parameters automatically. No attributes needed.

```csharp
public NavigationPresenter(NavigationState state, NavigationView view)
{
    _state = state;
    _view  = view;
}
```
