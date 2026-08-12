using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentAssertions;
using FmgLib.MauiMarkup;
using Microsoft.Maui.Controls;

namespace FmgLib.MauiMarkup.Generator.Test;

/// <summary>
/// Runtime behaviour of the fluent binding builder: a single sub binding still produces a plain binding,
/// several sub bindings produce a multi binding, and both flavours work with string paths as well as with
/// compiled getters.
/// </summary>
[TestFixture]
public class MultiBindingTests
{
    // Outside an app there is no dispatcher, and MAUI marshals every source change through one before it
    // reaches the target. Running the callbacks inline is enough to exercise the bindings.
    private sealed class InlineDispatcher : Microsoft.Maui.Dispatching.IDispatcher
    {
        public bool IsDispatchRequired => false;

        public Microsoft.Maui.Dispatching.IDispatcherTimer CreateTimer() => throw new NotSupportedException();

        public bool Dispatch(Action action)
        {
            action();
            return true;
        }

        public bool DispatchDelayed(TimeSpan delay, Action action)
        {
            action();
            return true;
        }
    }

    private sealed class InlineDispatcherProvider : Microsoft.Maui.Dispatching.IDispatcherProvider
    {
        private readonly InlineDispatcher _dispatcher = new();

        public Microsoft.Maui.Dispatching.IDispatcher? GetForCurrentThread() => _dispatcher;
    }

    [OneTimeSetUp]
    public void UseInlineDispatcher()
        => Microsoft.Maui.Dispatching.DispatcherProvider.SetCurrent(new InlineDispatcherProvider());

    [OneTimeTearDown]
    public void ResetDispatcher()
        => Microsoft.Maui.Dispatching.DispatcherProvider.SetCurrent(null);

    private sealed class Target : BindableObject
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(Target), default(string));

        public static readonly BindableProperty FlagProperty =
            BindableProperty.Create(nameof(Flag), typeof(bool), typeof(Target), false);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool Flag
        {
            get => (bool)GetValue(FlagProperty);
            set => SetValue(FlagProperty, value);
        }
    }

    private sealed class Person : INotifyPropertyChanged
    {
        private string _firstName = "Ada";
        private string _lastName = "Lovelace";
        private int _age = 36;
        private bool _isActive;
        private bool _hasLicence;

        public string FirstName { get => _firstName; set => Set(ref _firstName, value); }

        public string LastName { get => _lastName; set => Set(ref _lastName, value); }

        public int Age { get => _age; set => Set(ref _age, value); }

        public bool IsActive { get => _isActive; set => Set(ref _isActive, value); }

        public bool HasLicence { get => _hasLicence; set => Set(ref _hasLicence, value); }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    private static PropertyContext<string> TextOf(Target target) => new(target, Target.TextProperty);

    private static PropertyContext<bool> FlagOf(Target target) => new(target, Target.FlagProperty);

    [Test]
    public void SinglePathStillProducesAPlainBinding()
    {
        var target = new Target { BindingContext = new Person() };

        TextOf(target).Path(nameof(Person.FirstName)).Build().Should().BeTrue();

        target.Text.Should().Be("Ada");
    }

    [Test]
    public void SeveralPathsAreCombinedByMultiConvert()
    {
        var person = new Person();
        var target = new Target { BindingContext = person };

        TextOf(target)
            .Path(nameof(Person.FirstName))
            .Path(nameof(Person.LastName))
            .Path(nameof(Person.Age))
            .MultiConvert((string first, string last, int age) => $"{first} {last} ({age})")
            .Build();

        target.Text.Should().Be("Ada Lovelace (36)");

        person.LastName = "Byron";
        target.Text.Should().Be("Ada Byron (36)");
    }

    [Test]
    public void PerPathConvertRunsBeforeMultiConvert()
    {
        var target = new Target { BindingContext = new Person() };

        TextOf(target)
            .Path(nameof(Person.Age)).Convert((int age) => age >= 18)
            .Path(nameof(Person.FirstName))
            .MultiConvert((bool adult, string first) => $"{first}:{adult}")
            .Build();

        target.Text.Should().Be("Ada:True");
    }

    [Test]
    public void CompiledGettersCanBeCombined()
    {
        var person = new Person();
        var target = new Target { BindingContext = person };

        TextOf(target)
            .Getter(static (Person p) => p.FirstName)
            .Getter(static (Person p) => p.Age)
            .MultiConvert((string first, int age) => $"{first}-{age}")
            .Build();

        target.Text.Should().Be("Ada-36");

        person.Age = 37;
        target.Text.Should().Be("Ada-37");
    }

    [Test]
    public void CompiledGetterAndSetterStillRoundTripOnASingleBinding()
    {
        var person = new Person();
        var target = new Target { BindingContext = person };

        TextOf(target)
            .Getter(static (Person p) => p.FirstName)
            .Setter(static (Person p, string value) => p.FirstName = value)
            .BindingMode(BindingMode.TwoWay)
            .Build();

        target.Text.Should().Be("Ada");

        target.Text = "Grace";
        person.FirstName.Should().Be("Grace");
    }

    [Test]
    public void MultiConvertBackWritesEveryPathInOrder()
    {
        var person = new Person();
        var target = new Target { BindingContext = person };

        TextOf(target)
            .Path(nameof(Person.FirstName))
            .Path(nameof(Person.LastName))
            .MultiMode(BindingMode.TwoWay)
            .MultiConvert((string first, string last) => $"{first} {last}")
            .MultiConvertBack((string full) =>
            {
                var parts = full.Split(' ');
                return (parts[0], parts[1]);
            })
            .Build();

        target.Text.Should().Be("Ada Lovelace");

        target.Text = "Grace Hopper";
        person.FirstName.Should().Be("Grace");
        person.LastName.Should().Be("Hopper");
    }

    [Test]
    public void BooleanAggregatesNeedNoDelegate()
    {
        var person = new Person();
        var target = new Target { BindingContext = person };

        FlagOf(target)
            .Path(nameof(Person.IsActive))
            .Path(nameof(Person.HasLicence))
            .MultiAll()
            .Build();

        target.Flag.Should().BeFalse();

        person.IsActive = true;
        target.Flag.Should().BeFalse();

        person.HasLicence = true;
        target.Flag.Should().BeTrue();
    }

    [Test]
    public void MultiStringFormatCombinesWithoutAConverter()
    {
        var target = new Target { BindingContext = new Person() };

        TextOf(target)
            .Path(nameof(Person.FirstName))
            .Path(nameof(Person.LastName))
            .MultiStringFormat("{0} {1}")
            .Build();

        target.Text.Should().Be("Ada Lovelace");
    }

    [Test]
    public void AMissingSourceValueLeavesTheTargetUntouched()
    {
        var person = new Person { FirstName = null! };
        var target = new Target { BindingContext = person };

        TextOf(target)
            .Path(nameof(Person.FirstName))
            .Path(nameof(Person.Age))
            .MultiConvert((int missing, int age) => $"{missing}/{age}")
            .Build();

        // The first path cannot produce an int, but a null source is a "not yet" rather than an error.
        target.Text.Should().BeNull();
    }

    [Test]
    public void ArityMismatchIsReportedWhenBuilding()
    {
        var target = new Target { BindingContext = new Person() };

        var build = () => TextOf(target)
            .Path(nameof(Person.FirstName))
            .Path(nameof(Person.LastName))
            .Path(nameof(Person.Age))
            .MultiConvert((string first, string last) => $"{first} {last}")
            .Build();

        build.Should().Throw<InvalidOperationException>().WithMessage("*3 sub bindings*2 parameters*");
    }

    [Test]
    public void SeveralPathsWithoutACombinerAreReported()
    {
        var target = new Target { BindingContext = new Person() };

        var build = () => TextOf(target)
            .Path(nameof(Person.FirstName))
            .Path(nameof(Person.LastName))
            .Build();

        build.Should().Throw<InvalidOperationException>().WithMessage("*nothing combines them*");
    }

    [Test]
    public void AWronglyDeclaredDelegateParameterNamesThePath()
    {
        var target = new Target { BindingContext = new Person() };

        var apply = () => TextOf(target)
            .Path(nameof(Person.FirstName))
            .Path(nameof(Person.LastName))
            .MultiConvert((int first, string last) => $"{first} {last}")
            .Build();

        apply.Should().Throw<MauiMarkupConverterException>()
            .Which.BindingPath.Should().Be(nameof(Person.FirstName));
    }

    [Test]
    public void ModifiersDeclaredBeforeThePathStillApply()
    {
        var source = new Person { FirstName = "Edsger" };
        var target = new Target();

        new PropertyBindingBuilder<string>(TextOf(target))
            .Source(source)
            .Path(nameof(Person.FirstName))
            .Build();

        target.Text.Should().Be("Edsger");
    }

    [Test]
    public void TheBindingsEntryPointAlsoTakesAFluentCombiner()
    {
        var target = new Target { BindingContext = new Person() };

        TextOf(target)
            .Bindings(
                new Binding(nameof(Person.FirstName)),
                new Binding(nameof(Person.Age)))
            .MultiConvert((string first, int age) => $"{first}/{age}")
            .Build();

        target.Text.Should().Be("Ada/36");
    }

    [Test]
    public void TheBindingsEntryPointStillTakesAConverterInstance()
    {
        var target = new Target { BindingContext = new Person { IsActive = true, HasLicence = true } };

        FlagOf(target)
            .Bindings(
                new Binding(nameof(Person.IsActive)),
                new Binding(nameof(Person.HasLicence)))
            .Converter(new AllTrue())
            .FallbackValue(false)
            .Build();

        target.Flag.Should().BeTrue();
    }

    private sealed class AllTrue : IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
            => values is not null && values.All(v => v is true);

        public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, System.Globalization.CultureInfo culture)
            => throw new NotSupportedException();
    }

    [Test]
    public void StyleSettersProduceTheSameMultiBinding()
    {
        var setters = new List<Setter>();
        var context = new PropertySettersContext<string>(setters, Label.TextProperty);

        new PropertySettersBindingBuilder<string>(context)
            .Path(nameof(Person.FirstName))
            .Path(nameof(Person.LastName))
            .MultiConvert((string first, string last) => $"{first} {last}")
            .Build()
            .Should().BeTrue();

        setters.Should().ContainSingle();
        setters[0].Property.Should().Be(Label.TextProperty);
        setters[0].Value.Should().BeOfType<MultiBinding>();

        var style = new Style(typeof(Label));
        style.Setters.Add(setters[0]);

        var label = new Label { BindingContext = new Person(), Style = style };
        label.Text.Should().Be("Ada Lovelace");
    }

    [Test]
    public void RawBindingsCanBeMixedIntoTheMultiBinding()
    {
        var person = new Person();
        var target = new Target { BindingContext = person };

        TextOf(target)
            .Path(nameof(Person.FirstName))
            .Bindings(new Binding(nameof(Person.LastName)))
            .MultiConvert((string first, string last) => $"{last}, {first}")
            .Build();

        target.Text.Should().Be("Lovelace, Ada");
    }

    /// <summary>
    /// Pins the contract a fluent converter delegate is called under before its sources resolve.
    ///
    /// MAUI applies a binding immediately in SetBinding, so the converter runs once while the target
    /// still has no BindingContext. For a NON-NULLABLE VALUE TYPE parameter the library holds the
    /// property at its current value (BindingValues.IsMissing → Binding.DoNothing), because handing a
    /// delegate a default(int) it never asked for would be worse than waiting. For a REFERENCE TYPE
    /// parameter there is no such distinction available — null is a legitimate source value — so the
    /// delegate is invoked with null.
    ///
    /// The practical consequence, and the reason this test exists: a MultiConvert delegate that
    /// dereferences a reference-typed parameter must be null-safe, or it throws during page
    /// construction. Changing this would mean suppressing legitimately null values, so the behaviour
    /// is deliberate and pinned here rather than left to drift.
    /// </summary>
    [Test]
    public void ConverterReceivesNullForUnresolvedReferenceTypedSubBindings()
    {
        var target = new Target();   // deliberately no BindingContext
        var invocations = new List<(string? First, string? Last)>();

        TextOf(target)
            .Path(nameof(Person.FirstName))
            .Path(nameof(Person.LastName))
            .MultiConvert((string first, string last) =>
            {
                invocations.Add((first, last));
                return $"{first}|{last}";
            })
            .Build();

        invocations.Should().ContainSingle("the binding is applied once while the target is still unbound");
        invocations[0].Should().Be((null, null), "nothing has resolved yet");

        target.BindingContext = new Person();

        target.Text.Should().Be("Ada|Lovelace");
    }

    /// <summary>
    /// The value-type half of the contract above: an unresolved sub binding whose delegate parameter
    /// is a non-nullable value type holds the target instead of pushing a default through.
    /// </summary>
    [Test]
    public void ConverterIsNotCalledForUnresolvedValueTypedSubBindings()
    {
        var target = new Target { Text = "untouched" };
        var invoked = false;

        TextOf(target)
            .Path(nameof(Person.Age))
            .Path(nameof(Person.IsActive))
            .MultiConvert((int age, bool active) =>
            {
                invoked = true;
                return $"{age}|{active}";
            })
            .Build();

        invoked.Should().BeFalse("a non-nullable value type has nothing sensible to receive yet");
        target.Text.Should().Be("untouched", "the property is held at its current value");
    }
}
