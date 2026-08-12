namespace FmgLib.MauiMarkup.Gallery.Theme;

/// <summary>
/// The gallery's colour palette.
///
/// Every semantic role is a light/dark PAIR rather than a single colour, because the fluent
/// <c>e.OnLight(...).OnDark(...)</c> builder produces a real <c>AppThemeBinding</c> — the UI repaints
/// itself when the theme changes, with no page rebuild. <see cref="Ui"/> wraps the pairs into
/// one-word helpers so pages never repeat the theme plumbing.
/// </summary>
public static class AppColors
{
    // ---- brand ---------------------------------------------------------------------------------

    public static readonly Color Accent = "#6366F1".ToColor();
    public static readonly Color AccentDark = "#818CF8".ToColor();
    public static readonly Color AccentDeep = "#4338CA".ToColor();
    public static readonly Color Magenta = "#EC4899".ToColor();
    public static readonly Color Violet = "#8B5CF6".ToColor();
    public static readonly Color Cyan = "#06B6D4".ToColor();

    // ---- status --------------------------------------------------------------------------------

    public static readonly Color Success = "#10B981".ToColor();
    public static readonly Color Warning = "#F59E0B".ToColor();
    public static readonly Color Danger = "#EF4444".ToColor();
    public static readonly Color Info = "#0EA5E9".ToColor();

    // ---- light theme ---------------------------------------------------------------------------

    public static readonly Color PageLight = "#F5F6FB".ToColor();
    public static readonly Color SurfaceLight = "#FFFFFF".ToColor();
    public static readonly Color SurfaceAltLight = "#EEF0F8".ToColor();
    public static readonly Color BorderLight = "#E3E6F0".ToColor();
    public static readonly Color TextLight = "#0F172A".ToColor();
    public static readonly Color MutedLight = "#64748B".ToColor();
    public static readonly Color CodeLight = "#1E2537".ToColor();

    // ---- dark theme ----------------------------------------------------------------------------

    public static readonly Color PageDark = "#0B1020".ToColor();
    public static readonly Color SurfaceDark = "#151B2E".ToColor();
    public static readonly Color SurfaceAltDark = "#1D2540".ToColor();
    public static readonly Color BorderDark = "#28314D".ToColor();
    public static readonly Color TextDark = "#F1F5F9".ToColor();
    public static readonly Color MutedDark = "#94A3B8".ToColor();
    public static readonly Color CodeDark = "#0D1220".ToColor();

    /// <summary>Accent colours used to tint the demo cards, cycled by index.</summary>
    public static readonly Color[] CategoryTints =
    [
        "#6366F1".ToColor(),
        "#8B5CF6".ToColor(),
        "#EC4899".ToColor(),
        "#F59E0B".ToColor(),
        "#10B981".ToColor(),
        "#0EA5E9".ToColor()
    ];
}
