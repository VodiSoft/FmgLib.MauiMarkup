namespace FmgLib.MauiMarkup.Gallery.Controls;

/// <summary>
/// Base page for every demo: hot-reload wiring, the page header, and the responsive frame.
/// </summary>
/// <remarks>
/// <b>Responsiveness lives here, once.</b> The body is capped at <see cref="Ui.ReadableWidth"/> and
/// centred, so the same page is comfortable on a 360pt phone and on a maximised desktop window
/// instead of stretching a paragraph across 2000px. Padding tightens on phones through the idiom
/// builder. Individual demos then only have to worry about their own content.
///
/// <b>Why the derived page calls <c>InitializeHotReload()</c> and not this constructor:</b> it invokes
/// <c>Build()</c> immediately, and a base constructor runs BEFORE the derived class's field
/// initializers — a page with <c>readonly SomeViewModel _vm = new()</c> would build against a null
/// view model. Calling it as the last statement of the derived constructor is the safe order.
/// </remarks>
public abstract class DemoPage : ContentPage, IFmgLibHotReload
{
    /// <summary>Title shown in the navigation bar and as the page heading.</summary>
    protected abstract string DemoTitle { get; }

    /// <summary>One sentence describing what the page demonstrates.</summary>
    protected abstract string DemoSummary { get; }

    /// <summary>The demo sections, top to bottom.</summary>
    /// <returns>Sections to render.</returns>
    protected abstract IView[] BuildSections();

    /// <inheritdoc/>
    public void Build()
    {
        var body = new VerticalStackLayout()
            .Spacing(Ui.GapLg)
            .MaximumWidthRequest(Ui.ReadableWidth)
            .CenterHorizontal();

        body.Children.Add(Header());

        foreach (var section in BuildSections())
            body.Children.Add(section);

        this
        .PageSurface()
        .Title(DemoTitle)
        .Content(
            new ScrollView()
            .Content(
                new ContentView()
                .Padding(e => e
                    .OnPhone(new Thickness(Ui.GapMd, Ui.GapMd, Ui.GapMd, Ui.GapXl))
                    .Default(new Thickness(Ui.GapLg, Ui.GapLg, Ui.GapLg, Ui.GapXl)))
                .Content(body)
            )
        );
    }

    private View Header()
        => new VerticalStackLayout()
            .Spacing(Ui.GapSm)
            .Children(
                new Label().Text(DemoTitle).Display(),
                new Label().Text(DemoSummary).Muted().FontSize(15)
            );
}
