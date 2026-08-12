using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup.Gallery.Theme;

/// <summary>
/// The gallery's own fluent vocabulary, built entirely from the library's own methods.
///
/// This is the "composition shorthand" pattern from the Custom Extension Methods guide: because every
/// fluent method is generic over <c>T</c> and returns <c>T</c>, a helper that chains a few of them
/// stays fully composable — <c>new Label().Title().TextCenter()</c> still has a <c>Label</c> at every
/// step. Keeping design decisions here means a page reads as intent ("this is a card, this is a
/// caption") instead of a wall of colours and paddings.
/// </summary>
public static class Ui
{
    // ---- spacing scale -------------------------------------------------------------------------

    public const double GapXs = 4;
    public const double GapSm = 8;
    public const double Gap = 12;
    public const double GapMd = 16;
    public const double GapLg = 24;
    public const double GapXl = 36;

    /// <summary>Content wider than this is centred instead of stretched — the single rule that makes
    /// every page look deliberate on a desktop window rather than on a phone stretched to 2000px.</summary>
    public const double ReadableWidth = 940;

    // ---- surfaces ------------------------------------------------------------------------------

    /// <summary>Page background.</summary>
    public static T PageSurface<T>(this T self) where T : VisualElement
        => self.BackgroundColor(e => e.OnLight(AppColors.PageLight).OnDark(AppColors.PageDark));

    /// <summary>Raised surface: white on light, near-navy on dark.</summary>
    public static T Surface<T>(this T self) where T : VisualElement
        => self.BackgroundColor(e => e.OnLight(AppColors.SurfaceLight).OnDark(AppColors.SurfaceDark));

    /// <summary>Recessed surface, for wells and inline demo stages.</summary>
    public static T SurfaceAlt<T>(this T self) where T : VisualElement
        => self.BackgroundColor(e => e.OnLight(AppColors.SurfaceAltLight).OnDark(AppColors.SurfaceAltDark));

    /// <summary>A rounded, hairline-bordered card.</summary>
    public static Border Card(this Border self, double radius = 18)
        => self
            .Surface()
            .Stroke(new SolidColorBrush(AppColors.BorderLight))
            .Stroke(e => e.OnLight(new SolidColorBrush(AppColors.BorderLight)).OnDark(new SolidColorBrush(AppColors.BorderDark)))
            .StrokeThickness(1)
            .StrokeShape(new RoundRectangle().CornerRadius(radius))
            .Padding(GapLg);

    /// <summary>The stage a demo is presented on: recessed, rounded, generously padded.</summary>
    public static Border Stage(this Border self, double radius = 14)
        => self
            .SurfaceAlt()
            .StrokeThickness(0)
            .StrokeShape(new RoundRectangle().CornerRadius(radius))
            .Padding(GapMd);

    // ---- typography ----------------------------------------------------------------------------

    /// <summary>Page-level heading.</summary>
    public static T Display<T>(this T self) where T : Label
        => self
            .FontSize(e => e.OnPhone(28.0).Default(34.0))
            .FontAttributes(Bold)
            .TextColor(e => e.OnLight(AppColors.TextLight).OnDark(AppColors.TextDark));

    /// <summary>Section heading inside a page.</summary>
    public static T Heading<T>(this T self) where T : Label
        => self
            .FontSize(19)
            .FontAttributes(Bold)
            .TextColor(e => e.OnLight(AppColors.TextLight).OnDark(AppColors.TextDark));

    /// <summary>Regular reading text.</summary>
    public static T Body<T>(this T self) where T : Label
        => self
            .FontSize(15)
            .LineHeight(1.35)
            .TextColor(e => e.OnLight(AppColors.TextLight).OnDark(AppColors.TextDark));

    /// <summary>Secondary text: descriptions, captions, hints.</summary>
    public static T Muted<T>(this T self) where T : Label
        => self
            .FontSize(13.5)
            .LineHeight(1.35)
            .TextColor(e => e.OnLight(AppColors.MutedLight).OnDark(AppColors.MutedDark));

    /// <summary>Small all-caps label used above groups.</summary>
    public static T Overline<T>(this T self) where T : Label
        => self
            .FontSize(11)
            .FontAttributes(Bold)
            .CharacterSpacing(1.6)
            .TextTransform(TextTransform.Uppercase)
            .TextColor(e => e.OnLight(AppColors.MutedLight).OnDark(AppColors.MutedDark));

    /// <summary>Monospace, for code and values.</summary>
    public static T Mono<T>(this T self) where T : Label
        => self
            .FontSize(13)
            .FontFamily(DeviceInfo.Platform == DevicePlatform.WinUI ? "Consolas" : "Menlo")
            .TextColor(e => e.OnLight(AppColors.TextLight).OnDark(AppColors.TextDark));

    // ---- pieces --------------------------------------------------------------------------------

    /// <summary>A pill-shaped tag.</summary>
    public static Border Pill(this Border self, Color tint)
        => self
            .BackgroundColor(tint.WithAlpha(0.14f))
            .StrokeThickness(0)
            .StrokeShape(new RoundRectangle().CornerRadius(999))
            .Padding(10, 5);

    /// <summary>Hairline separator.</summary>
    public static BoxView Divider(this BoxView self)
        => self
            .HeightRequest(1)
            .Color(e => e.OnLight(AppColors.BorderLight).OnDark(AppColors.BorderDark));

    /// <summary>Soft drop shadow, tuned to stay subtle in dark mode.</summary>
    public static T SoftShadow<T>(this T self) where T : VisualElement
        => self.Shadow(new Shadow()
            .Brush(new SolidColorBrush(Colors.Black))
            .Offset(new Point(0, 6))
            .Radius(18)
            .Opacity(0.10f));

    /// <summary>The gallery's signature gradient, used for hero surfaces and accents.</summary>
    public static LinearGradientBrush BrandGradient(double angleX = 1, double angleY = 1)
        => new LinearGradientBrush()
            .StartPoint(new Point(0, 0))
            .EndPoint(new Point(angleX, angleY))
            .GradientStops(
                new GradientStop(AppColors.AccentDeep, 0f),
                new GradientStop(AppColors.Accent, 0.55f),
                new GradientStop(AppColors.Magenta, 1f));
}
