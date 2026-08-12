using System.Collections.Immutable;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FmgLib.MauiMarkup.Generator.Test;

/// <summary>
/// Locks down generation against member shapes that are only reachable — or only BROKEN — when the
/// control lives in a REFERENCED assembly rather than in the consuming project.
///
/// This distinction is the whole point of the fixture. A <c>protected internal</c> setter is
/// perfectly assignable from code in the declaring assembly, so a control declared inside the test
/// source itself can never reproduce the failure; it only breaks once the control ships as a NuGet
/// package. Every test here therefore compiles a real "third-party library" assembly first and
/// references it, exactly mirroring how a consumer sees DrawnUi.Maui, Syncfusion, DevExpress, etc.
///
/// The concrete regressions pinned here were all reported against DrawnUi.Maui 1.10.5.6 with
/// <c>MauiMarkupSourceGenerator=true</c>:
///   * <c>SkiaControl.IsMeasuring</c> — <c>{ get; protected internal set; }</c> produced
///     <c>self.IsMeasuring = value;</c> (CS0272).
///   * <c>DrawnView.ExecuteAfterCreated</c> (<c>Queue&lt;Action&gt;</c>) and
///     <c>SkiaShell.NavigationStackScreens</c> (<c>LinkedList&lt;PageInStack&gt;</c>) — collection
///     shaped, but with no callable <c>Add</c>, produced <c>self.Prop.Add(item);</c> (CS1929).
/// </summary>
[TestFixture]
public class ThirdPartyControlSurfaceTests
{
    /// <summary>
    /// A stand-in NuGet control library. Each member reproduces one real-world shape; the ones that
    /// must KEEP generating sit next to the ones that must be skipped, so an overbroad filter fails
    /// the fixture just as loudly as a too-narrow one.
    /// </summary>
    private const string ThirdPartyLibrarySource = """
        using System;
        using System.Collections.Generic;
        using System.Collections.ObjectModel;
        using Microsoft.Maui.Controls;

        namespace ThirdParty.Controls;

        public class ThirdPartyCanvas : ContentView
        {
            // DrawnUi's SkiaControl.IsMeasuring. `protected internal` is Accessibility
            // .ProtectedOrInternal — neither Protected, Private nor Internal, which is exactly how
            // it slipped past the old accessibility filter.
            public bool IsMeasuring { get; protected internal set; }

            // `private protected` (Accessibility.ProtectedAndInternal) — the other value the old
            // filter did not name.
            public bool IsAttached { get; private protected set; }

            // The two accessibilities the old filter DID name; kept so they stay covered.
            public bool IsDisposed { get; protected set; }

            public bool IsInternallyTracked { get; internal set; }

            // Assignable only inside an object initializer (CS8852 anywhere else).
            public string? Culture { get; init; }

            // DrawnUi's DrawnView.ExecuteAfterCreated. Queue<T> is IEnumerable + ICollection and has
            // no Add at all — it has Enqueue.
            public Queue<Action> ExecuteAfterCreated { get; } = new();

            // DrawnUi's SkiaShell.NavigationStackScreens. LinkedList<T> implements ICollection<T>
            // .Add EXPLICITLY, so `list.Add(item)` does not bind without a cast.
            public LinkedList<string> NavigationStackScreens { get; } = new();

            // ---- everything below must still generate ----

            public double BlurRadius { get; set; }

            public IList<View> Items { get; } = new List<View>();

            public ObservableCollection<string> Tags { get; } = new();

            public event EventHandler? Rendered;
        }
        """;

    /// <summary>
    /// Note what is NOT here: no <c>using System;</c>, no <c>using System.Threading.Tasks;</c>, no
    /// implicit-usings tree anywhere in <see cref="RunCore"/>. A consuming project is free to set
    /// <c>&lt;ImplicitUsings&gt;disable&lt;/ImplicitUsings&gt;</c> (the default for anything ported
    /// from an older SDK), and generated code must still compile there — so every type it names is
    /// emitted <c>global::</c>-rooted. Compiling this file with the bare minimum of imports is what
    /// proves it: the <c>Animate…To</c> overloads alone reference <c>Task&lt;bool&gt;</c> and
    /// <c>Easing</c>, which used to be emitted unqualified (CS0246).
    /// </summary>
    private const string ConsumerSource = """
        using FmgLib.MauiMarkup;
        using ThirdParty.Controls;

        namespace ConsoleApp1;

        [MauiMarkup(typeof(ThirdPartyCanvas))]
        public class Markup { }
        """;

    [Test]
    public void InaccessibleSetters_AreSkipped_AndSettableOnesStillGenerate()
    {
        var (generated, errors) = Run(ConsumerSource);

        errors.Should().BeEmpty(string.Join(Environment.NewLine, errors));

        generated.Should().NotContain("IsMeasuring", "a protected internal setter is unreachable from the consuming assembly (CS0272)");
        generated.Should().NotContain("IsAttached", "a private protected setter is unreachable from the consuming assembly (CS0272)");
        generated.Should().NotContain("IsDisposed");
        generated.Should().NotContain("IsInternallyTracked");
        generated.Should().NotContain("Culture", "an init-only setter cannot be assigned outside an object initializer (CS8852)");

        generated.Should().Contain("BlurRadius");
        generated.Should().Contain("OnRendered");
    }

    [Test]
    public void CollectionsWithoutCallableAdd_AreSkipped_AndRealCollectionsStillGenerate()
    {
        var (generated, errors) = Run(ConsumerSource);

        errors.Should().BeEmpty(string.Join(Environment.NewLine, errors));

        generated.Should().NotContain("ExecuteAfterCreated", "Queue<T> exposes Enqueue, not Add (CS1929)");
        generated.Should().NotContain("NavigationStackScreens", "LinkedList<T> implements ICollection<T>.Add explicitly (CS1929)");

        generated.Should().Contain("Items");
        generated.Should().Contain("Tags");
    }

    [Test]
    public void ConsumerCode_UsingTheGeneratedSurface_CompilesCleanly()
    {
        var source = ConsumerSource + """


            public static class Usage
            {
                public static ThirdPartyCanvas Build() => new ThirdPartyCanvas()
                    .BlurRadius(8)
                    .Items(new global::Microsoft.Maui.Controls.Label())
                    .Tags("first", "second")
                    .OnRendered((sender, args) => { });
            }
            """;

        var (_, errors) = Run(source);

        errors.Should().BeEmpty(string.Join(Environment.NewLine, errors));
    }

    /// <summary>
    /// The generated files must not add diagnostics to a consumer's build. Nullable annotations
    /// (<c>string?</c>) are emitted verbatim from the property types, which is CS8669 in any project
    /// that leaves nullable disabled — the default for a project created before .NET 6 and the
    /// state that produced 271 warnings in the reported DrawnUi build.
    /// </summary>
    [Test]
    public void GeneratedFiles_ProduceNoWarnings_WhenNullableContextIsDisabled()
    {
        var (_, _, allDiagnostics) = RunCore(ConsumerSource, NullableContextOptions.Disable);

        var generatedDiagnostics = allDiagnostics
            .Where(diagnostic => diagnostic.Severity >= DiagnosticSeverity.Warning)
            .Where(diagnostic => diagnostic.Location.SourceTree?.FilePath.Contains("FmgLib.MauiMarkup.SourceGenerator", StringComparison.Ordinal) == true)
            .ToList();

        generatedDiagnostics.Should().BeEmpty(string.Join(Environment.NewLine, generatedDiagnostics.Select(d => d.ToString())));
    }

    private static (string Generated, ImmutableArray<Diagnostic> Errors) Run(string consumerSource)
    {
        var (generated, errors, _) = RunCore(consumerSource, NullableContextOptions.Annotations);
        return (generated, errors);
    }

    private static (string Generated, ImmutableArray<Diagnostic> Errors, ImmutableArray<Diagnostic> All) RunCore(string consumerSource, NullableContextOptions nullableContext)
    {
        var references = CreateBaseReferences();
        references.Add(CompileThirdPartyLibrary(references));

        // Deliberately no implicit-usings tree — see the remarks on ConsumerSource.
        var compilation = CSharpCompilation.Create("ThirdPartyControlSurfaceTests_" + Guid.NewGuid().ToString("N"),
            new[] { CSharpSyntaxTree.ParseText(consumerSource) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: nullableContext));

        CSharpGeneratorDriver
            .Create(new SourceGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generated = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees
            .Where(tree => tree.FilePath.Contains("ThirdPartyCanvas", StringComparison.OrdinalIgnoreCase))
            .Select(tree => tree.ToString()));

        var diagnostics = outputCompilation.GetDiagnostics();
        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToImmutableArray();

        return (generated, errors, diagnostics);
    }

    /// <summary>
    /// Emits <see cref="ThirdPartyLibrarySource"/> to a real assembly image, so the generator sees
    /// the control through metadata — the only way <c>protected internal</c> is genuinely
    /// out of reach — instead of as source in the same compilation.
    /// </summary>
    /// <param name="references">Reference set to compile the stand-in library against.</param>
    /// <returns>A metadata reference to the emitted library.</returns>
    private static MetadataReference CompileThirdPartyLibrary(IEnumerable<MetadataReference> references)
    {
        var compilation = CSharpCompilation.Create("ThirdParty.Controls",
            new[] { CSharpSyntaxTree.ParseText(ThirdPartyLibrarySource) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);

        result.Success.Should().BeTrue(string.Join(Environment.NewLine,
            result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.ToString())));

        stream.Position = 0;
        return MetadataReference.CreateFromStream(stream);
    }

    private static List<MetadataReference> CreateBaseReferences()
    {
        var referencePaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        void Add(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                referencePaths[Path.GetFileNameWithoutExtension(path)] = path;
            }
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            {
                Add(assembly.Location);
            }
        }

        Add(typeof(Microsoft.Maui.Controls.BindableObject).Assembly.Location);
        Add(typeof(Microsoft.Maui.Thickness).Assembly.Location);
        Add(typeof(Microsoft.Maui.Graphics.Color).Assembly.Location);
        Add(typeof(FmgLib.MauiMarkup.IFmgLibHotReload).Assembly.Location);

        return referencePaths.Values
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToList();
    }
}
