using System.ComponentModel;
using System.Globalization;
using System.Resources;
using FluentAssertions;

namespace FmgLib.MauiMarkup.Generator.Test;

/// <summary>
/// Behavioural guards for the localization system.
///
/// Every test here corresponds to something that was actually wrong or missing: silent culture
/// mismatches, a JSON backend with no fallback while the RESX backend had one, a loader whose
/// exceptions were discarded, and an app builder whose positional overload could not tell a file name
/// from a culture name.
/// </summary>
[TestFixture]
public class LocalizationTests
{
    // MAUI marshals every source change through a dispatcher before it reaches the target; outside an
    // app there is none, so bindings would never update. Running callbacks inline is enough.
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
        private readonly InlineDispatcher dispatcher = new();

        public Microsoft.Maui.Dispatching.IDispatcher? GetForCurrentThread() => dispatcher;
    }

    [OneTimeSetUp]
    public void UseInlineDispatcher()
        => Microsoft.Maui.Dispatching.DispatcherProvider.SetCurrent(new InlineDispatcherProvider());

    [OneTimeTearDown]
    public void ResetDispatcher()
        => Microsoft.Maui.Dispatching.DispatcherProvider.SetCurrent(null);

    private const string Sample = """
        {
          "Hello":       { "en-US": "Hello!",       "tr": "Merhaba!" },
          "OnlyEnglish": { "en-US": "Only English" },
          "WelcomeUser": { "en-US": "Welcome, {0}!", "tr-TR": "Hoş geldin, {0}!" }
        }
        """;

    [SetUp]
    public void SetUp()
    {
        // The translators are singletons; give every test a clean, platform-free starting point.
        FmgLib.MauiMarkup.Localization.BaseTranslator.DispatchOverride = action => action();

        Translator.Instance = new Translator();
        TranslatorResx.Instance = new TranslatorResx();

        Sample.LoadLocalizationData();
    }

    [TearDown]
    public void TearDown()
    {
        FmgLib.MauiMarkup.Localization.BaseTranslator.DispatchOverride = null;
        CultureInfo.DefaultThreadCurrentCulture = null;
        CultureInfo.DefaultThreadCurrentUICulture = null;
    }

    // ---- culture fallback ----------------------------------------------------------------------

    /// <summary>
    /// The JSON backend used to require an exact culture match, so a file written with a neutral "tr"
    /// key produced nothing at all on a tr-TR device — while the RESX backend resolved it fine through
    /// ResourceManager. The two backends must agree.
    /// </summary>
    [Test]
    public void SpecificCulture_FallsBackToItsNeutralParent()
    {
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));

        Translator.Instance["Hello"].Should().Be("Merhaba!");
    }

    [Test]
    public void FallbackCulture_IsUsedWhenTheCultureChainYieldsNothing()
    {
        Translator.Instance.FallbackCulture = CultureInfo.GetCultureInfo("en-US");
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));

        // "OnlyEnglish" exists in en-US only: tr-TR → tr → (fallback) en-US.
        Translator.Instance["OnlyEnglish"].Should().Be("Only English");
    }

    [Test]
    public void CultureNamesAreMatchedCaseInsensitively()
    {
        """{ "Hi": { "EN-us": "Hi!" } }""".LoadLocalizationData();
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));

        Translator.Instance["Hi"].Should().Be("Hi!");
    }

    // ---- missing keys --------------------------------------------------------------------------

    [Test]
    public void MissingKey_DefaultsToReturningTheKey()
    {
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));

        Translator.Instance["NoSuchKey"].Should().Be("NoSuchKey");
    }

    [Test]
    public void MissingKey_HonoursTheConfiguredBehaviour()
    {
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));

        Translator.Instance.MissingTranslation = MissingTranslationBehavior.ReturnEmpty;
        Translator.Instance["NoSuchKey"].Should().BeEmpty();

        Translator.Instance.MissingTranslation = MissingTranslationBehavior.Marker;
        Translator.Instance["NoSuchKey"].Should().Be("⟦NoSuchKey⟧");

        Translator.Instance.MissingTranslation = MissingTranslationBehavior.Throw;
        Translator.Instance.Invoking(t => _ = t["NoSuchKey"]).Should().Throw<KeyNotFoundException>();
    }

    /// <summary>
    /// ResourceManager.GetString returns null for an unknown key, which reached the binding as null and
    /// rendered an empty label — silently different from the JSON backend returning the key.
    /// </summary>
    [Test]
    public void ResxMissingKey_BehavesLikeTheJsonBackend()
    {
        UseEmptyResourceManager();

        TranslatorResx.Instance["NoSuchKey"].Should().Be("NoSuchKey");

        TranslatorResx.Instance.MissingTranslation = MissingTranslationBehavior.Marker;
        TranslatorResx.Instance["NoSuchKey"].Should().Be("⟦NoSuchKey⟧");
    }

    [Test]
    public void ResxWithoutRegisteredResourceManager_ThrowsAnActionableError()
    {
        TranslatorResx.ResourceManager = null;

        TranslatorResx.Instance
            .Invoking(t => _ = t["Hello"])
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*UseMauiMarkupLocalizationWithResx*");
    }

    // ---- live switching ------------------------------------------------------------------------

    [Test]
    public void ChangeCulture_RaisesPropertyChangedForEveryBoundProperty()
    {
        var raised = new List<string?>();
        ((INotifyPropertyChanged)Translator.Instance).PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));

        // A null property name is the contract MAUI reads as "re-evaluate every binding on this source".
        raised.Should().ContainSingle().Which.Should().BeNull();
    }

    [Test]
    public void ChangeCulture_ReresolvesTranslations()
    {
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
        Translator.Instance["Hello"].Should().Be("Hello!");

        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));
        Translator.Instance["Hello"].Should().Be("Merhaba!");
    }

    /// <summary>
    /// Translations loaded after the UI is already up (a later load, a downloaded pack) used to leave
    /// every bound label showing its raw key, because nothing notified the bindings.
    /// </summary>
    [Test]
    public void LoadingDataLater_RefreshesBoundTranslations()
    {
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));

        var raised = 0;
        ((INotifyPropertyChanged)Translator.Instance).PropertyChanged += (_, _) => raised++;

        """{ "Hello": { "en-US": "Reloaded!" } }""".LoadLocalizationData();

        raised.Should().Be(1);
        Translator.Instance["Hello"].Should().Be("Reloaded!");
    }

    // ---- culture sync --------------------------------------------------------------------------

    /// <summary>
    /// Switching language used to leave dates, numbers and currency on the device culture — an app with
    /// English labels and Turkish dates. CultureSyncMode.Full is the default for that reason.
    /// </summary>
    [Test]
    public void ChangeCulture_AppliesAmbientCultureByDefault()
    {
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));

        CultureInfo.DefaultThreadCurrentCulture!.Name.Should().Be("tr-TR");
        CultureInfo.DefaultThreadCurrentUICulture!.Name.Should().Be("tr-TR");
        1.5.ToString(CultureInfo.CurrentCulture).Should().Be("1,5");
    }

    [Test]
    public void CultureSyncMode_None_LeavesAmbientCultureAlone()
    {
        Translator.Instance.CultureSync = CultureSyncMode.None;

        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));

        CultureInfo.DefaultThreadCurrentCulture.Should().BeNull();
        Translator.Instance["Hello"].Should().Be("Merhaba!");
    }

    [Test]
    public void CultureSyncMode_UICultureOnly_LeavesFormattingAlone()
    {
        Translator.Instance.CultureSync = CultureSyncMode.UICultureOnly;

        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));

        CultureInfo.DefaultThreadCurrentUICulture!.Name.Should().Be("tr-TR");
        CultureInfo.DefaultThreadCurrentCulture.Should().BeNull();
    }

    // ---- right to left -------------------------------------------------------------------------

    [Test]
    public void FlowDirection_FollowsTheCulture()
    {
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
        Translator.Instance.IsRightToLeft.Should().BeFalse();
        Translator.Instance.FlowDirection.Should().Be(FlowDirection.LeftToRight);

        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("ar-SA"));
        Translator.Instance.IsRightToLeft.Should().BeTrue();
        Translator.Instance.FlowDirection.Should().Be(FlowDirection.RightToLeft);
    }

    // ---- loading -------------------------------------------------------------------------------

    /// <summary>
    /// The builder discarded the loader's Task (`_ = LoadLocalizationDataAsync(...)`), so a malformed
    /// file left the app silently untranslated even though the documentation promised an exception.
    /// </summary>
    [Test]
    public void MalformedJson_Throws()
    {
        FluentActions.Invoking(() => "{ this is not json }".LoadLocalizationData())
            .Should().Throw<FileLoadException>()
            .WithMessage("*language file*");
    }

    [Test]
    public void EmptyJson_Throws()
    {
        FluentActions.Invoking(() => "   ".LoadLocalizationData())
            .Should().Throw<FileLoadException>();
    }

    // ---- app builder argument validation --------------------------------------------------------

    /// <summary>
    /// `UseMauiMarkupLocalization("Common.json", "Checkout.json")` binds the FIRST argument to
    /// `defaultLang`, because the culture parameter precedes `params string[] filePaths` — the exact
    /// call the documentation used to recommend. It threw a bare CultureNotFoundException naming a file;
    /// it now explains the fix.
    /// </summary>
    [Test]
    public void FileNameInTheCultureParameter_IsRejectedWithAnActionableMessage()
    {
        FluentActions.Invoking(() => CultureResolverProbe("Common.json"))
            .Should().Throw<ArgumentException>()
            .WithMessage("*looks like a file name*");

        FluentActions.Invoking(() => CultureResolverProbe("Languages/tr.json"))
            .Should().Throw<ArgumentException>()
            .WithMessage("*looks like a file name*");
    }

    [Test]
    public void InvalidCultureName_IsRejectedWithAnActionableMessage()
    {
        FluentActions.Invoking(() => CultureResolverProbe("not a culture"))
            .Should().Throw<ArgumentException>()
            .WithMessage("*not a valid culture name*");
    }

    [Test]
    public void ValidCultureName_IsAccepted()
    {
        Translator.Instance.ChangeCulture("tr-TR");

        Translator.Instance.CurrentCulture.Name.Should().Be("tr-TR");
    }

    // ---- explicit culture lookups ---------------------------------------------------------------

    [Test]
    public void TranslateString_ResolvesAnExplicitCultureIndependentlyOfTheActiveOne()
    {
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));

        Translator.Instance.TranslateString("Hello", CultureInfo.GetCultureInfo("tr-TR")).Should().Be("Merhaba!");
        Translator.Instance["Hello"].Should().Be("Hello!");
    }

    [Test]
    public void GetTranslationExtension_WalksTheCultureChain()
    {
        LocalizationData.Data!.GetTranslation("Hello", "tr-TR").Should().Be("Merhaba!");
        LocalizationData.Data!.GetTranslation("NoSuchKey", "tr-TR").Should().Be("NoSuchKey");
    }

    // ---- bindings ------------------------------------------------------------------------------

    /// <summary>
    /// The end-to-end promise of the whole feature: a bound property re-reads itself on a language
    /// switch, with no page reload and no manual refresh.
    /// </summary>
    [Test]
    public void TranslatedBinding_UpdatesOnLanguageSwitch()
    {
        var target = new BindingTarget();
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));

        TextOf(target).Translate("Hello").Build().Should().BeTrue();
        target.Text.Should().Be("Hello!");

        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));
        target.Text.Should().Be("Merhaba!");
    }

    /// <summary>
    /// A translated sentence with a runtime value in it has to react to BOTH inputs — the language and
    /// the value — which is why TranslateFormat builds a MultiBinding rather than formatting eagerly.
    /// </summary>
    [Test]
    public void TranslateFormat_ReactsToBothLanguageAndArgumentChanges()
    {
        var user = new UserViewModel { UserName = "Ada" };
        var target = new BindingTarget { BindingContext = user };

        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));

        TextOf(target).TranslateFormat("WelcomeUser", nameof(UserViewModel.UserName)).Build().Should().BeTrue();
        target.Text.Should().Be("Welcome, Ada!");

        user.UserName = "Grace";
        target.Text.Should().Be("Welcome, Grace!");

        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));
        target.Text.Should().Be("Hoş geldin, Grace!");
    }

    /// <summary>
    /// Numeric and date placeholders must follow the SELECTED language, not the device's — the whole
    /// point of defaulting CultureSync to Full and of formatting with the translator's culture.
    /// </summary>
    [Test]
    public void TranslateFormat_FormatsArgumentsWithTheSelectedCulture()
    {
        """{ "Total": { "en-US": "Total: {0:N2}", "tr-TR": "Toplam: {0:N2}" } }""".LoadLocalizationData();

        var order = new UserViewModel { Amount = 1234.5m };
        var target = new BindingTarget { BindingContext = order };

        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
        TextOf(target).TranslateFormat("Total", nameof(UserViewModel.Amount)).Build();
        target.Text.Should().Be("Total: 1,234.50");

        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));
        target.Text.Should().Be("Toplam: 1.234,50");
    }

    /// <summary>
    /// A translator dropping "{0}" from one language must not take the app down in that language.
    /// </summary>
    [Test]
    public void TranslateFormat_SurvivesABrokenFormatString()
    {
        """{ "Broken": { "en-US": "Hello {0} {1}" } }""".LoadLocalizationData();

        var target = new BindingTarget { BindingContext = new UserViewModel { UserName = "Ada" } };
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));

        FluentActions.Invoking(() => TextOf(target).TranslateFormat("Broken", nameof(UserViewModel.UserName)).Build())
            .Should().NotThrow();

        target.Text.Should().Be("Hello {0} {1}");
    }

    [Test]
    public void FlowDirectionBinding_FollowsTheCulture()
    {
        var target = new BindingTarget();

        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
        new PropertyContext<FlowDirection>(target, BindingTarget.DirectionProperty).FromCulture().Build().Should().BeTrue();
        target.Direction.Should().Be(FlowDirection.LeftToRight);

        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("ar-SA"));
        target.Direction.Should().Be(FlowDirection.RightToLeft);
    }

    private static PropertyContext<string> TextOf(BindingTarget target) => new(target, BindingTarget.TextProperty);

    private sealed class BindingTarget : BindableObject
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(BindingTarget), default(string));

        public static readonly BindableProperty DirectionProperty =
            BindableProperty.Create(nameof(Direction), typeof(FlowDirection), typeof(BindingTarget), FlowDirection.MatchParent);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public FlowDirection Direction
        {
            get => (FlowDirection)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }
    }

    private sealed class UserViewModel : INotifyPropertyChanged
    {
        private string userName = string.Empty;
        private decimal amount;

        public string UserName
        {
            get => userName;
            set { userName = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserName))); }
        }

        public decimal Amount
        {
            get => amount;
            set { amount = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Amount))); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private static void CultureResolverProbe(string cultureName)
        => Translator.Instance.ChangeCulture(cultureName);

    private static void UseEmptyResourceManager()
        => TranslatorResx.ResourceManager = new ResourceManager("FmgLib.MauiMarkup.Generator.Test.NoSuchResources", typeof(LocalizationTests).Assembly);
}
