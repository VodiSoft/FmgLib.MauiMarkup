#nullable enable

using System.ComponentModel;
using System.Globalization;

namespace FmgLib.MauiMarkup.Localization;

/// <summary>
/// Shared state and change notification for the JSON (<see cref="Translator"/>) and RESX
/// (<see cref="TranslatorResx"/>) translators.
/// </summary>
/// <remarks>
/// Live language switching works because every translated property is bound to this object's indexer
/// and <see cref="ChangeCulture(CultureInfo)"/> raises <see cref="INotifyPropertyChanged.PropertyChanged"/>
/// with a <see langword="null"/> property name, which MAUI treats as "every property changed" and
/// re-reads each binding. Nothing is cached per binding, so the refresh cost is one dictionary probe per
/// bound string.
/// </remarks>
public abstract class BaseTranslator : INotifyPropertyChanged
{
    /// <summary>
    /// Replaces the main-thread dispatch used to raise change notification. Set by tests, which run
    /// without a platform <c>MainThread</c>; production code leaves it <see langword="null"/>.
    /// </summary>
    public static Action<Action>? DispatchOverride { get; set; }

    private CultureInfo currentCulture = CultureInfo.CurrentUICulture;

    /// <summary>The culture translations are currently resolved in.</summary>
    public CultureInfo CurrentCulture
    {
        get => currentCulture;
        private set => currentCulture = value;
    }

    /// <summary>
    /// Culture consulted after <see cref="CurrentCulture"/> and its parents yield nothing — the
    /// equivalent of the neutral <c>.resx</c>.
    /// </summary>
    public CultureInfo? FallbackCulture
    {
        get => fallbackCulture;
        set
        {
            fallbackCulture = value;
            OnCultureChanged();
        }
    }

    private CultureInfo? fallbackCulture;

    /// <summary>What to return for a key that has no translation.</summary>
    public MissingTranslationBehavior MissingTranslation { get; set; } = MissingTranslationBehavior.ReturnKey;

    /// <summary>How far <see cref="ChangeCulture(CultureInfo)"/> propagates beyond this translator.</summary>
    public CultureSyncMode CultureSync { get; set; } = CultureSyncMode.Full;

    /// <summary><see langword="true"/> when <see cref="CurrentCulture"/> is written right to left.</summary>
    public bool IsRightToLeft => CurrentCulture.TextInfo.IsRightToLeft;

    /// <summary>
    /// <see cref="CurrentCulture"/> expressed as a MAUI <see cref="Microsoft.Maui.FlowDirection"/>, so a
    /// page can mirror itself for Arabic or Hebrew by binding to it.
    /// </summary>
    public FlowDirection FlowDirection => IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

    /// <summary>
    /// Switches the active culture and refreshes every bound translation.
    /// </summary>
    /// <param name="culture">The culture to switch to.</param>
    public void ChangeCulture(CultureInfo culture)
    {
        if (culture is null)
            throw new ArgumentNullException(nameof(culture));

        CurrentCulture = culture;
        ApplyCultureSync(culture);
        OnCultureChanged();
        OnPropertyChanged();
    }

    /// <summary>
    /// Switches the active culture by name.
    /// </summary>
    /// <param name="cultureName">A culture name such as <c>en-US</c>.</param>
    public void ChangeCulture(string cultureName)
        => ChangeCulture(CultureResolver.Parse(cultureName, nameof(cultureName)));

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises change notification for every bound translation, on the main thread.
    /// </summary>
    /// <remarks>
    /// Marshalling matters: a language switch triggered from a background thread (a settings sync, a
    /// downloaded language pack) would otherwise update bindings off the UI thread and crash on the
    /// platform. When the caller is already on the main thread the notification is raised inline, so
    /// <see cref="ChangeCulture(CultureInfo)"/> stays observably synchronous.
    /// </remarks>
    public void OnPropertyChanged()
    {
        Dispatch(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)));
    }

    /// <summary>
    /// Called whenever the resolved culture chain changes, so a derived translator can rebuild any
    /// per-culture lookup it keeps.
    /// </summary>
    protected virtual void OnCultureChanged()
    {
    }

    /// <summary>
    /// Applies <see cref="MissingTranslation"/> to a key that produced no translation.
    /// </summary>
    /// <param name="key">The key that was not found.</param>
    /// <returns>The value to hand back to the binding.</returns>
    protected string ResolveMissing(string key)
    {
        return MissingTranslation switch
        {
            MissingTranslationBehavior.ReturnEmpty => string.Empty,
            MissingTranslationBehavior.Marker => $"⟦{key}⟧",
            MissingTranslationBehavior.Throw => throw new KeyNotFoundException(
                $"No translation for '{key}' in '{CurrentCulture.Name}'" +
                (FallbackCulture is null ? "." : $" or fallback '{FallbackCulture.Name}'.")),
            _ => key
        };
    }

    /// <summary>
    /// Culture names to probe for one lookup, most specific first.
    /// </summary>
    protected IReadOnlyList<string> CultureChain() => CultureResolver.BuildChain(CurrentCulture, FallbackCulture);

    private void ApplyCultureSync(CultureInfo culture)
    {
        switch (CultureSync)
        {
            case CultureSyncMode.UICultureOnly:
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.CurrentUICulture = culture;
                break;

            case CultureSyncMode.Full:
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                break;
        }
    }

    private static void Dispatch(Action action)
    {
        var dispatch = DispatchOverride;
        if (dispatch != null)
        {
            dispatch(action);
            return;
        }

        try
        {
            if (MainThread.IsMainThread)
                action();
            else
                MainThread.BeginInvokeOnMainThread(action);
        }
        catch (NotImplementedException)
        {
            // No platform behind Essentials (unit tests, a plain net9.0/net10.0 host). Raising inline is
            // the only option, and is correct there because there is no UI thread to marshal to.
            action();
        }
    }
}
