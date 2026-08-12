using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup.Gallery.Controls;

/// <summary>
/// The building blocks every demo page is assembled from. Keeping them here means the pages contain
/// only the feature they are demonstrating — no repeated chrome.
/// </summary>
public static class Demo
{
    /// <summary>
    /// A titled card: heading, one line of explanation, then the demo itself.
    /// </summary>
    /// <param name="title">Section heading.</param>
    /// <param name="description">One sentence on what the section shows.</param>
    /// <param name="content">The demo views.</param>
    /// <returns>The section card.</returns>
    public static Border Section(string title, string description, params IView[] content)
    {
        var body = new VerticalStackLayout().Spacing(Ui.Gap);

        body.Children.Add(new Label().Text(title).Heading());
        body.Children.Add(new Label().Text(description).Muted());

        foreach (var view in content)
            body.Children.Add((IView)view);

        return new Border().Card().SoftShadow().Content(body);
    }

    /// <summary>
    /// The recessed area a live demo sits on, so it reads as "this is the running example" rather
    /// than as more page content.
    /// </summary>
    /// <param name="content">The demo views.</param>
    /// <returns>The stage.</returns>
    public static Border Stage(params IView[] content)
    {
        var body = new VerticalStackLayout().Spacing(Ui.Gap);

        foreach (var view in content)
            body.Children.Add((IView)view);

        return new Border().Stage().Content(body);
    }

    /// <summary>Horizontal stage variant, wrapping so it survives a narrow phone.</summary>
    /// <param name="content">The demo views.</param>
    /// <returns>The stage.</returns>
    public static Border WrapStage(params IView[] content)
    {
        var body = new FlexLayout()
            .Wrap(FlexWrap.Wrap)
            .AlignItems(FlexAlignItems.Center)
            .JustifyContent(FlexJustify.Start);

        foreach (var view in content)
            body.Children.Add((IView)view);

        return new Border().Stage().Content(body);
    }

    /// <summary>
    /// The markup that produced the demo above it. Every section shows its own source, because the
    /// point of the gallery is the code, not the rectangle.
    /// </summary>
    /// <param name="snippet">The C# snippet.</param>
    /// <returns>The code block.</returns>
    public static Border Code(string snippet)
        => new Border()
            .BackgroundColor(e => e.OnLight(AppColors.CodeLight).OnDark(AppColors.CodeDark))
            .StrokeThickness(0)
            .StrokeShape(new RoundRectangle().CornerRadius(12))
            .Padding(Ui.GapMd)
            .Content(
                new ScrollView()
                .Orientation(ScrollOrientation.Horizontal)
                .HorizontalScrollBarVisibility(ScrollBarVisibility.Never)
                .Content(
                    new Label()
                    .Text(snippet.Trim())
                    .Mono()
                    .TextColor("#E2E8F0".ToColor())
                )
            );

    /// <summary>A short aside — a tip, a caveat, a "why".</summary>
    /// <param name="text">The note text.</param>
    /// <param name="glyph">Leading emoji.</param>
    /// <returns>The note row.</returns>
    public static View Note(string text, string glyph = "💡")
        => new HorizontalStackLayout()
            .Spacing(Ui.GapSm)
            .Children(
                new Label().Text(glyph).FontSize(14),
                new Label().Text(text).Muted().FillHorizontal()
            );

    /// <summary>A labelled value chip, used to show live state next to a demo.</summary>
    /// <param name="tint">Chip colour.</param>
    /// <param name="configureText">Configures the chip's label (usually a binding).</param>
    /// <returns>The chip.</returns>
    public static Border Chip(Color tint, Action<Label> configureText)
    {
        var label = new Label().FontSize(13).FontAttributes(Bold).TextColor(tint);
        configureText(label);

        return new Border().Pill(tint).Margin(0, 0, Ui.GapSm, Ui.GapSm).Content(label);
    }

    /// <summary>A plain text chip.</summary>
    /// <param name="text">Chip text.</param>
    /// <param name="tint">Chip colour.</param>
    /// <returns>The chip.</returns>
    public static Border Chip(string text, Color tint) => Chip(tint, label => label.Text(text));

    /// <summary>A small square swatch used by the colour/brush demos.</summary>
    /// <param name="caption">Caption under the swatch.</param>
    /// <param name="configure">Configures the swatch surface.</param>
    /// <returns>The swatch.</returns>
    public static View Swatch(string caption, Action<Border> configure)
    {
        var surface = new Border()
            .SizeRequest(96, 72)
            .StrokeThickness(0)
            .StrokeShape(new RoundRectangle().CornerRadius(12));

        configure(surface);

        return new VerticalStackLayout()
            .Spacing(Ui.GapXs)
            .Margin(0, 0, Ui.Gap, Ui.Gap)
            .Children(
                surface,
                new Label().Text(caption).Muted().FontSize(12).TextCenterHorizontal().WidthRequest(96)
            );
    }
}
