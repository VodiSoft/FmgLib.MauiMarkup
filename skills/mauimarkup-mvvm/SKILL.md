---
name: mauimarkup-mvvm
description: Structure FmgLib.MauiMarkup apps with MVVM — FmgLibContentPage<TViewModel>, typed BindingContext, compiled Getter/Setter bindings, commands, dependency injection and CommunityToolkit.Mvvm. Use when writing or refactoring view models, wiring pages to view models, registering pages in MauiProgram, or when bindings in a C# markup MAUI app need to be made type-safe.
license: MIT
---

# MVVM with FmgLib.MauiMarkup

Requires the `mauimarkup` core skill. FmgLib changes only how views are constructed; MVVM, DI and
`INotifyPropertyChanged` work exactly as in any MAUI app — but C# markup lets you make every binding
compile-checked, which is the whole point of doing MVVM here.

## The standard page

```csharp
public class ProfilePage : FmgLibContentPage<ProfileViewModel>
{
    public ProfilePage(ProfileViewModel vm) : base(vm) { }

    public override void Build() =>
        this.Content(
            new VerticalStackLayout()
            .Padding(20)
            .Spacing(12)
            .Children(
                new Label()
                    .Text(e => e.Getter(static (ProfileViewModel v) => v.UserName))
                    .FontSize(24),

                new Entry()
                    .Text(e => e
                        .Getter(static (ProfileViewModel v) => v.Email)
                        .Setter(static (ProfileViewModel v, string s) => v.Email = s)
                        .BindingMode(BindingMode.TwoWay)),

                new Button()
                    .Text("Refresh")
                    .Command(BindingContext.RefreshCommand)      // typed — no cast
                    .IsEnabled(e => e.Getter(static (ProfileViewModel v) => !v.IsBusy)),

                new ActivityIndicator()
                    .IsRunning(e => e.Getter(static (ProfileViewModel v) => v.IsBusy))
            ));
}
```

`FmgLibContentPage<TViewModel>` does three things worth knowing:

1. Assigns the view model to `BindingContext` **before** the first `Build()`, so `Build()` may read it.
2. Re-types the `BindingContext` property to `TViewModel` — `BindingContext.RefreshCommand` needs no
   cast, and a renamed command becomes a compile error.
3. Wires hot reload, so `Build()` re-runs on edits with the view-model state preserved.

Use `FmgLibContentPage` (non-generic) for pages with no view model, and plain
`ContentPage, IFmgLibHotReload` + `InitializeHotReload()` when you need a different base class.

## Dependency injection

```csharp
// MauiProgram.cs
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddTransient<ProfileViewModel>();
builder.Services.AddTransient<ProfilePage>();
```

Pages resolved from the container get their view model injected. For Shell routes:

```csharp
Routing.RegisterRoute("orders/detail", typeof(OrderDetailPage));
```

Shell resolves registered routes through the service provider, so constructor injection works there too.

## Two ways to reach a command

```csharp
// 1. Direct reference — preferred with a typed BindingContext. Compile-checked, no reflection.
new Button().Command(BindingContext.SaveCommand)

// 2. Binding — needed when the command lives on the *item* inside a template, or the VM is late-bound
new Button().Command(e => e.Path("SaveCommand")).CommandParameter(e => e.Path("."))
```

Inside item templates the binding context is the item. To invoke a **page-level** command from a row,
capture the view model in a local and reference it directly:

```csharp
public override void Build()
{
    var vm = BindingContext;                       // typed

    this.Content(new CollectionView()
        .ItemsSource(vm.Products)
        .ItemTemplate(() => new HorizontalStackLayout().Children(
            new Label().Text(e => e.Getter(static (ProductVM p) => p.Name)),
            new Button()
                .Text("Add")
                .Command(vm.AddToCartCommand)                 // page command
                .Bind(Button.CommandParameterProperty, ".")   // this row's item
        )));
}
```

## CommunityToolkit.Mvvm

Fully supported, and the recommended pairing:

```csharp
public partial class ProfileViewModel : ObservableObject
{
    private readonly IProfileService service;

    public ProfileViewModel(IProfileService service) => this.service = service;

    [ObservableProperty] private string userName = string.Empty;
    [ObservableProperty] private bool isBusy;

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsBusy = true;
        try { UserName = await service.GetNameAsync(); }
        finally { IsBusy = false; }
    }
}
```

The generated `UserName` property and `RefreshCommand` bind normally:
`.Text(e => e.Getter(static (ProfileViewModel v) => v.UserName))` and
`.Command(BindingContext.RefreshCommand)`.

> Source-generated members exist only after a build. If `Getter` can't see `UserName` or
> `RefreshCommand`, build once and re-check before assuming a naming problem.

## Compiled bindings — the rules that matter

Valid getter expressions are **simple property access** (plus null-conditionals, indexers and casts).
Method calls, interpolation, concatenation and arithmetic are not:

```csharp
e.Getter(static (VM v) => v.Address?.City)      // ✔
e.Getter(static (VM v) => v.Items[0])           // ✔
e.Getter(static (VM v) => $"Hi {v.Name}")       // ✘ — use .Convert(...) or a computed VM property
```

Push transformations into the view model wherever the result is meaningful domain state:

```csharp
// VM
public string DisplayName => $"{FirstName} {LastName}";
public Color StatusColor => IsError ? Colors.Red : Colors.Green;

// View — one binding, no converter, unit-testable logic
new Label()
    .Text(e => e.Getter(static (VM v) => v.DisplayName))
    .TextColor(e => e.Getter(static (VM v) => v.StatusColor))
```

Remember to raise `PropertyChanged` for computed properties when their inputs change
(`[NotifyPropertyChangedFor(nameof(DisplayName))]` with the toolkit).

Two-way bindings need a `Setter`; `OneTime` is right for values that never change after load.

## State and `Build()`

`Build()` re-runs on every hot reload, so it must never own state:

```csharp
// ✔ view model injected, survives every rebuild
public ProfilePage(ProfileViewModel vm) : base(vm) { }

// ✘ new view model on every reload — the app loses its state as you type
public override void Build() => this.BindingContext(new ProfileViewModel());
```

Subscribe to view-model events (`PropertyChanged`, messenger registrations) in the constructor, not in
`Build()`, or subscriptions accumulate with each rebuild.

## Navigation

Nothing FmgLib-specific:

```csharp
await Shell.Current.GoToAsync($"orders/detail?id={order.Id}");
await Shell.Current.GoToAsync("..");
```

Keep navigation in the view model behind an `INavigationService` if you want it unit-testable; the view
stays a pure description of the tree either way.

## Testing

Because pages are plain classes, a view model is testable without any MAUI host, and a page's tree can
be constructed in a unit test to assert structure:

```csharp
var vm = new ProfileViewModel(new FakeProfileService());
await vm.RefreshCommand.ExecuteAsync(null);
Assert.Equal("fmg", vm.UserName);
```

## Checklist

- [ ] Pages derive from `FmgLibContentPage<TViewModel>` where a view model exists.
- [ ] Every view-model binding uses `Getter`, not a `Path` string.
- [ ] Two-way bindings have a matching `Setter`.
- [ ] Commands referenced directly through the typed `BindingContext` where possible.
- [ ] View models registered in DI; no `new` view model inside `Build()`.
- [ ] Display formatting lives in computed VM properties, not converters, where it is domain logic.
- [ ] Event subscriptions in the constructor, never in `Build()`.
