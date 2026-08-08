using FluentAssertions;
using FmgLib.MauiMarkup;
using Microsoft.Maui.Controls;

namespace FmgLib.MauiMarkup.Generator.Test;

/// <summary>
/// OnLight/OnDark has to produce a live binding rather than a value resolved once, so that a control keeps
/// following the theme after it has been built.
/// <para>
/// The final hop — .NET MAUI pushing the theme change down the element tree — cannot be exercised here: it
/// travels from <see cref="Application"/> through its windows, and a window only attaches to the application
/// once a platform handler exists. What is asserted below is everything up to that hop: the values resolve
/// through the theme, and the style setter carries a binding instead of a snapshot.
/// </para>
/// </summary>
[TestFixture]
public class ThemeBindingTests
{
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

    private Application _app = null!;

    [SetUp]
    public void SetUp()
    {
        Microsoft.Maui.Dispatching.DispatcherProvider.SetCurrent(new InlineDispatcherProvider());
        _app = new Application();
        Application.Current = _app;
    }

    [TearDown]
    public void TearDown()
    {
        Application.Current = null;
        Microsoft.Maui.Dispatching.DispatcherProvider.SetCurrent(null);
    }

    private Label BuildLabel(AppTheme theme)
    {
        _app.UserAppTheme = theme;

        var label = new Label();

        new PropertyContext<Color>(label, Label.TextColorProperty)
            .OnLight(Colors.Black)
            .OnDark(Colors.White)
            .Build()
            .Should().BeTrue();

        return label;
    }

    [Test]
    public void TheLightValueIsUsedUnderTheLightTheme()
        => BuildLabel(AppTheme.Light).TextColor.Should().Be(Colors.Black);

    [Test]
    public void TheDarkValueIsUsedUnderTheDarkTheme()
        => BuildLabel(AppTheme.Dark).TextColor.Should().Be(Colors.White);

    [Test]
    public void ADeclaredSideAloneDoesNotPushNull()
    {
        _app.UserAppTheme = AppTheme.Dark;

        var label = new Label();

        new PropertyContext<Color>(label, Label.TextColorProperty)
            .OnLight(Colors.Red)
            .Build();

        // Only the light side was declared, so the dark theme falls back to it instead of clearing the property.
        label.TextColor.Should().Be(Colors.Red);
    }

    [Test]
    public void DefaultWinsWhenNeitherSideMatches()
    {
        _app.UserAppTheme = AppTheme.Dark;

        var label = new Label();

        new PropertyContext<Color>(label, Label.TextColorProperty)
            .OnLight(Colors.Red)
            .Default(Colors.Green)
            .Build();

        label.TextColor.Should().Be(Colors.Green);
    }

    [Test]
    public void AStyleSetterCarriesABindingRatherThanASnapshot()
    {
        _app.UserAppTheme = AppTheme.Light;

        var setters = new List<Setter>();

        new PropertySettersContext<Color>(setters, Label.TextColorProperty)
            .OnLight(Colors.Black)
            .OnDark(Colors.White)
            .Build()
            .Should().BeTrue();

        setters.Should().ContainSingle();

        // The crux of the fix: a plain value here would freeze the style at the theme that was in effect
        // when the style was created.
        setters[0].Value.Should().BeAssignableTo<BindingBase>();

        var style = new Microsoft.Maui.Controls.Style(typeof(Label));
        style.Setters.Add(setters[0]);

        new Label { Style = style }.TextColor.Should().Be(Colors.Black);
    }

}
