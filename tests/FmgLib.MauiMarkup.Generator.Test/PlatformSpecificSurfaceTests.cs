using System.Collections.Immutable;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FmgLib.MauiMarkup.Generator.Test;

/// <summary>
/// Locks down generation against real-world "platform-specific API surface" bugs.
///
/// Multi-targeted third-party libraries frequently define a member only for one platform
/// (e.g. a control's Android-only partial class declares a member that its iOS partial does
/// not). .NET Hot Reload/source generators run once PER target-framework compilation, so a
/// member that exists only on Android is only ever seen — and only ever generates an
/// extension — for the Android compilation; this isolation is already structurally correct
/// and is not what these tests exercise.
///
/// What actually broke in the field (see the Nalu.NaluTabBar.DefaultBlurRadius incident) is
/// that the GENERATOR ITSELF produced invalid code for certain property/event *shapes* —
/// static members chief among them — and that bug only surfaced on the one platform where a
/// matching member happened to exist. These tests pin the shapes that must never regress,
/// regardless of which platform's compilation encounters them.
/// </summary>
[TestFixture]
public class PlatformSpecificSurfaceTests
{
    private static (GeneratorDriverRunResult RunResult, ImmutableArray<Diagnostic> Errors) RunGenerator(string source)
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

        var references = referencePaths.Values
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToList();

        const string implicitUsings = """
            global using System;
            global using System.Collections.Generic;
            global using System.Linq;
            global using System.Threading;
            global using System.Threading.Tasks;
            global using Microsoft.Maui;
            global using Microsoft.Maui.Controls;
            global using Microsoft.Maui.Graphics;
            """;

        var compilation = CSharpCompilation.Create("PlatformSpecificSurfaceTests_" + Guid.NewGuid().ToString("N"),
            new[] { CSharpSyntaxTree.ParseText(source), CSharpSyntaxTree.ParseText(implicitUsings) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Note: we deliberately do NOT pre-check `compilation.GetDiagnostics()` before running the
        // generator — the [MauiMarkup]/[MauiMarkupAttachedProp] attributes referenced by the input
        // source only exist once the generator's RegisterPostInitializationOutput has run, so the
        // pre-generator compilation is expected to have unresolved-attribute errors. The output
        // compilation below is the only meaningful thing to assert on.
        var driver = (CSharpGeneratorDriver)CSharpGeneratorDriver
            .Create(new SourceGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var errors = outputCompilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToImmutableArray();
        return (driver.GetRunResult(), errors);
    }

    /// <summary>
    /// The exact shape of the field bug: a public STATIC settable <c>double</c> property on a
    /// VisualElement-derived third-party control (this combination is what routes through the
    /// Animate…To generator, which used to hardcode an instance access and emit
    /// <c>self.DefaultBlurRadius = value;</c> for a static member — CS0176).
    /// </summary>
    private const string StaticPropertySource = """
        using FmgLib.MauiMarkup;

        namespace ConsoleApp1;

        public class ThirdPartyTabBar : ContentView
        {
            // Global default, set once at startup — exactly like Nalu.NaluTabBar.DefaultBlurRadius.
            public static double DefaultBlurRadius { get; set; } = 8;

            // A normal instance property must still generate (sanity check the filter isn't overbroad).
            public double InstanceBlurRadius { get; set; } = 8;

            public static event EventHandler? GlobalConfigurationChanged;

            public event EventHandler? Tapped;
        }

        [MauiMarkup(typeof(ThirdPartyTabBar))]
        public class Markup { }
        """;

    [Test]
    public void StaticProperty_And_StaticEvent_CompileCleanly_AndAreNotGenerated()
    {
        var (runResult, errors) = RunGenerator(StaticPropertySource);

        errors.Should().BeEmpty(string.Join(Environment.NewLine, errors));

        var generated = runResult.GeneratedTrees
            .First(t => t.FilePath.Contains("ThirdPartyTabBar", StringComparison.OrdinalIgnoreCase))
            .ToString();

        // The static members must not be turned into instance-fluent extension methods at all.
        generated.Should().NotContain("DefaultBlurRadius");
        generated.Should().NotContain("GlobalConfigurationChanged");

        // The instance members with the same shapes must still be generated normally.
        generated.Should().Contain("InstanceBlurRadius");
        generated.Should().Contain("AnimateInstanceBlurRadiusTo");
        generated.Should().Contain("OnTapped");
    }

    [Test]
    public void ConsumerCode_UsingOnlyGeneratedMembers_CompilesCleanly()
    {
        // End-to-end guard: mirrors the exact reported usage — configuring the static default
        // directly (plain C#, no fluent wrapper needed/possible) alongside fluent instance
        // configuration of the sibling instance members.
        var source = StaticPropertySource + """


        public static class Usage
        {
            public static ThirdPartyTabBar Build()
            {
                ThirdPartyTabBar.DefaultBlurRadius = 12; // plain static assignment — always available
                return new ThirdPartyTabBar()
                    .InstanceBlurRadius(10)
                    .OnTapped((s, e) => { });
            }
        }
        """;

        var (_, errors) = RunGenerator(source);
        errors.Should().BeEmpty(string.Join(Environment.NewLine, errors));
    }

    /// <summary>
    /// <see cref="Helpers.IsEffectivelyPublic"/> guards against a public property/event whose
    /// type (or a generic type argument, or array element type) is not itself public — which
    /// would make the generated <c>public</c> extension method uncompilable for every external
    /// caller (CS0053/CS7025-class errors).
    ///
    /// This can only be unit-tested against the helper directly: valid C# SOURCE can never
    /// actually declare such a member (the C# compiler itself rejects "public member exposes a
    /// less-accessible type" at the declaring site with CS0053/CS7025). The pattern only reaches
    /// consumers as already-compiled METADATA — e.g. a NuGet package post-processed by an
    /// IL-merge/internalize or obfuscation tool that rewrites a type's visibility without
    /// re-verifying the members that reference it, bypassing the C# front-end check entirely.
    /// Since that requires raw IL authoring to reproduce faithfully, the helper is exercised
    /// directly against Roslyn symbols instead, which is the precise unit under test.
    /// </summary>
    [Test]
    public void IsEffectivelyPublic_CorrectlyClassifiesTypeShapes()
    {
        const string source = """
            namespace ConsoleApp1
            {
                public class PublicOuter
                {
                    public class PublicNested { }
                    internal class InternalNested { }
                }

                internal class InternalType { }

                public class PublicGeneric<T> { }
            }
            """;

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => (MetadataReference)MetadataReference.CreateFromFile(a.Location))
            .ToList();

        var compilation = CSharpCompilation.Create("IsEffectivelyPublicTests",
            new[] { CSharpSyntaxTree.ParseText(source) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        INamedTypeSymbol Get(string metadataName) => compilation.GetTypeByMetadataName(metadataName)!;

        var publicOuter = Get("ConsoleApp1.PublicOuter");
        var publicNested = Get("ConsoleApp1.PublicOuter+PublicNested");
        var internalNested = Get("ConsoleApp1.PublicOuter+InternalNested");
        var internalType = Get("ConsoleApp1.InternalType");
        var publicGeneric = Get("ConsoleApp1.PublicGeneric`1");

        publicOuter.Should().NotBeNull();
        publicNested.Should().NotBeNull();
        internalNested.Should().NotBeNull();
        internalType.Should().NotBeNull();
        publicGeneric.Should().NotBeNull();

        // Plain public type: accessible. Plain internal type: not.
        Helpers.IsEffectivelyPublic(publicOuter).Should().BeTrue();
        Helpers.IsEffectivelyPublic(internalType).Should().BeFalse();

        // A public type nested under a public outer type is fine; nested under (or itself)
        // internal is not — this is what the generator actually filters on.
        IsAccessibleForGeneration(publicNested).Should().BeTrue();
        IsAccessibleForGeneration(internalNested).Should().BeFalse();

        // Generic type arguments are walked recursively: List<InternalType> must be rejected
        // even though List<T> itself is a public BCL type.
        var listOfInternal = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")!.Construct(internalType);
        IsAccessibleForGeneration(listOfInternal).Should().BeFalse();

        var genericOfInternal = publicGeneric.Construct(internalType);
        IsAccessibleForGeneration(genericOfInternal).Should().BeFalse();

        var listOfPublic = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")!.Construct(publicOuter);
        IsAccessibleForGeneration(listOfPublic).Should().BeTrue();

        // Array element type is walked too.
        var arrayOfInternal = compilation.CreateArrayTypeSymbol(internalType);
        IsAccessibleForGeneration(arrayOfInternal).Should().BeFalse();

        // A method's own type parameter is always in scope regardless of any constraint.
        var methodTypeParameter = publicGeneric.TypeParameters[0];
        IsAccessibleForGeneration(methodTypeParameter).Should().BeTrue();

        static bool IsAccessibleForGeneration(ITypeSymbol type) => Helpers.IsEffectivelyPublic(type);
    }
}
