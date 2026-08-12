using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FmgLib.MauiMarkup.Gallery.Models;

/// <summary>A product row used by the binding and collection demos.</summary>
/// <param name="Name">Display name.</param>
/// <param name="Category">Category label.</param>
/// <param name="Price">Unit price.</param>
/// <param name="Rating">Rating out of five.</param>
/// <param name="Glyph">Emoji stand-in for a product image.</param>
/// <param name="InStock">Whether the product can be ordered.</param>
public sealed record Product(string Name, string Category, decimal Price, double Rating, string Glyph, bool InStock);

/// <summary>
/// A small view model shared by the data-binding demos. Deliberately hand-written and dependency-free
/// so the gallery shows the library rather than an MVVM toolkit.
/// </summary>
public sealed class DemoViewModel : INotifyPropertyChanged
{
    private string firstName = "Ada";
    private string lastName = "Lovelace";
    private int age = 36;
    private bool acceptedTerms;
    private bool confirmedEmail;
    private double budget = 120;
    private string search = string.Empty;

    public DemoViewModel()
    {
        AddToCartCommand = new RelayCommand<Product>(product =>
        {
            if (product is not null)
                LastAction = $"Added {product.Name} to the cart";
        });

        ClearCommand = new RelayCommand(() =>
        {
            Search = string.Empty;
            LastAction = "Cleared the search";
        });
    }

    public string FirstName
    {
        get => firstName;
        set { if (Set(ref firstName, value)) RaiseComputedName(); }
    }

    public string LastName
    {
        get => lastName;
        set { if (Set(ref lastName, value)) RaiseComputedName(); }
    }

    public int Age
    {
        get => age;
        set => Set(ref age, value);
    }

    public bool AcceptedTerms
    {
        get => acceptedTerms;
        set { if (Set(ref acceptedTerms, value)) Raise(nameof(CanSubmit)); }
    }

    public bool ConfirmedEmail
    {
        get => confirmedEmail;
        set { if (Set(ref confirmedEmail, value)) Raise(nameof(CanSubmit)); }
    }

    public double Budget
    {
        get => budget;
        set { if (Set(ref budget, value)) Raise(nameof(Affordable)); }
    }

    public string Search
    {
        get => search;
        set
        {
            if (Set(ref search, value))
                ApplySearch();
        }
    }

    private string lastAction = "Nothing yet.";

    public string LastAction
    {
        get => lastAction;
        private set => Set(ref lastAction, value);
    }

    /// <summary>Computed for the compiled-binding demo.</summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>Computed for the compiled-binding demo.</summary>
    public string Initials =>
        $"{(FirstName.Length > 0 ? FirstName[0] : ' ')}{(LastName.Length > 0 ? LastName[0] : ' ')}".Trim();

    /// <summary>Both checkboxes have to be ticked — the multi-binding demo does the same thing declaratively.</summary>
    public bool CanSubmit => AcceptedTerms && ConfirmedEmail;

    /// <summary>Products under the current budget.</summary>
    public int Affordable => Catalogue.Count(p => (double)p.Price <= Budget);

    /// <summary>Fed to TranslateFormat, so the date is formatted in the selected culture.</summary>
    public DateTime Today { get; } = DateTime.Now;

    public ICommand AddToCartCommand { get; }

    public ICommand ClearCommand { get; }

    /// <summary>The full product list.</summary>
    public static IReadOnlyList<Product> Catalogue { get; } =
    [
        new("Sourdough Loaf", "Bakery", 4.90m, 4.8, "🥖", true),
        new("Cold Brew", "Drinks", 3.40m, 4.5, "🧋", true),
        new("Olive Oil 1L", "Pantry", 12.50m, 4.9, "🫒", true),
        new("Sea Salt Flakes", "Pantry", 6.20m, 4.2, "🧂", false),
        new("Dark Chocolate", "Snacks", 5.75m, 4.7, "🍫", true),
        new("Espresso Beans", "Drinks", 18.00m, 4.9, "☕", true),
        new("Sun-dried Tomatoes", "Pantry", 8.10m, 4.1, "🍅", true),
        new("Aged Cheddar", "Dairy", 14.30m, 4.6, "🧀", false)
    ];

    /// <summary>The filtered list bound by the collection demos.</summary>
    public ObservableCollection<Product> Products { get; } = [.. Catalogue];

    /// <summary>Tags rendered by the BindableLayout demo.</summary>
    public IReadOnlyList<string> Tags { get; } =
        ["fluent", "no-xaml", "hot reload", "generated", "typed", "themable", "responsive"];

    public event PropertyChangedEventHandler? PropertyChanged;

    private void ApplySearch()
    {
        var matches = Catalogue.Where(p =>
            Search.Length == 0 ||
            p.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
            p.Category.Contains(Search, StringComparison.OrdinalIgnoreCase));

        Products.Clear();

        foreach (var product in matches)
            Products.Add(product);
    }

    private void RaiseComputedName()
    {
        Raise(nameof(FullName));
        Raise(nameof(Initials));
    }

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        Raise(propertyName);
        return true;
    }

    private void Raise(string? propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

/// <summary>Minimal ICommand so the gallery needs no MVVM package.</summary>
public sealed class RelayCommand(Action execute, Func<bool>? canExecute = null) : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => execute();

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <inheritdoc cref="RelayCommand"/>
/// <typeparam name="T">Command parameter type.</typeparam>
public sealed class RelayCommand<T>(Action<T?> execute, Func<T?, bool>? canExecute = null) : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => canExecute?.Invoke((T?)parameter) ?? true;

    public void Execute(object? parameter) => execute((T?)parameter);

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
