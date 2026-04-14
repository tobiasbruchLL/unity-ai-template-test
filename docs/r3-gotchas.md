# R3 Gotchas

## CombineLatest with an array of observables requires an explicit type argument

When passing an `Observable<T>[]` to `Observable.CombineLatest`, the compiler cannot infer `T` and produces:

```
error CS0411: The type arguments for method 'Observable.CombineLatest<T>(params Observable<T>[])'
cannot be inferred from the usage.
```

**Wrong:**
```csharp
Observable.CombineLatest(streams, bools => Array.TrueForAll(bools, b => b))
```

**Right:** split into two calls — provide the type argument explicitly, then chain `.Select`:
```csharp
Observable.CombineLatest<bool>(streams).Select(bools => Array.TrueForAll(bools, b => b))
```
