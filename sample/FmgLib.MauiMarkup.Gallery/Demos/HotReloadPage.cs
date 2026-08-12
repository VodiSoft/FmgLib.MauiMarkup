using FmgLib.MauiMarkup.Gallery.Controls;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// How <c>Build()</c> turns .NET hot reload into live UI editing.
/// </summary>
public partial class HotReloadPage : DemoPage
{
    private int buildCount;

    public HotReloadPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Hot Reload";

    protected override string DemoSummary =>
        "Because the UI is plain C#, .NET hot reload already applies to it. The library adds one thing: re-running your construction method so the edit appears on the running app.";

    protected override IView[] BuildSections() =>
    [
        Counter(),
        ThePattern(),
        BaseClasses(),
        Safety()
    ];

    private IView Counter()
    {
        buildCount++;

        return Demo.Section(
            "Build() has run " + buildCount + (buildCount == 1 ? " time" : " times"),
            "Every page in this gallery follows the same pattern. Edit any file while debugging, save, and this number goes up as the page redraws itself.",
            Demo.Stage(
                new Label()
                    .Text($"{buildCount}")
                    .FontSize(46)
                    .FontAttributes(Bold)
                    .TextColor(AppColors.Danger)
                    .TextCenterHorizontal(),

                new Label()
                    .Text("Try it: change a colour in this file and save.")
                    .Muted()
                    .TextCenterHorizontal()
            ));
    }

    private static IView ThePattern()
        => Demo.Section(
            "The pattern",
            "Implement IFmgLibHotReload, call InitializeHotReload() in the constructor, and put all UI construction in Build().",
            Demo.Code("""
                public partial class ExamplePage : ContentPage, IFmgLibHotReload
                {
                    public ExamplePage()
                    {
                        this.InitializeHotReload();     // calls Build() once, then registers the page
                    }

                    public void Build()
                    {
                        this.Content(
                            new Label().Text("FmgLib.MauiMarkup").FontSize(30).TextCenter()
                        );
                    }
                }
                """),
            Demo.Note("Call InitializeHotReload() LAST in the constructor: it invokes Build() immediately, and a base constructor runs before the derived class's field initializers.", "⚠️"));

    private static IView BaseClasses()
        => Demo.Section(
            "Ready-made base classes",
            "FmgLibContentPage wires the pattern up for you, and its generic form re-types BindingContext so the view model needs no cast.",
            Demo.Code("""
                public class HomePage : FmgLibContentPage
                {
                    public override void Build() =>
                        this.Content(new Label().Text("Hello!").Center());
                }

                public class ProfilePage : FmgLibContentPage<ProfileViewModel>
                {
                    public ProfilePage(ProfileViewModel vm) : base(vm) { }

                    public override void Build() =>
                        this.Content(new Label().Text(BindingContext.Name));   // typed, no cast
                }
                """),
            Demo.Note("This gallery uses its own DemoPage base for the same reason — shared chrome, one Build() per demo."));

    private static IView Safety()
        => Demo.Section(
            "What it does not break",
            "The registration is deliberately conservative, because a debugging aid that leaks pages or crashes the app is worse than none.",
            new VerticalStackLayout()
            .Spacing(Ui.GapSm)
            .Children(
                Demo.Note("Registration uses weak references — a hot-reloaded page is still collected when it is popped, and leak detectors stay quiet.", "🧠"),
                Demo.Note("A Build() that throws during a reload is logged and surfaced through ReloadFailed; it never takes the app down.", "🛟"),
                Demo.Note("Every update writes a diagnostic line to the debug output — if you see it, the pipeline is working end to end.", "🔎"),
                Demo.Note("Do not start animations inside Build(): it re-runs on every reload. Use OnLoaded or OnAppearing.", "🎬")
            ));
}
