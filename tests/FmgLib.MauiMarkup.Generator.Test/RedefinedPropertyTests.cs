using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FmgLib.MauiMarkup.Generator.Test;

/// <summary>
/// Locks down the redefined-property rules using Syncfusion.Maui.Toolkit's SfButton, whose
/// surface triggered a real-world regression:
///
/// - SfButton redeclares TextColor/StrokeThickness/CornerRadius/Background with the SAME type as
///   its base ButtonBase — these must NOT get a "New" suffix and must NOT be duplicated (the
///   base's generic extension serves SfButton), otherwise callers hit missing methods or CS0121.
/// - Command/FontSize/FontAttributes live only on ButtonBase — annotating just SfButton must
///   still produce them, i.e. eligible third-party base classes are generated automatically.
/// </summary>
[TestFixture]
public class RedefinedPropertyTests
{
    private static (GeneratorDriverRunResult RunResult, Compilation Output, IEnumerable<Diagnostic> Diagnostics) RunGenerator(string source, params string[] extraReferences)
    {
        var rootPath = AppDomain.CurrentDomain.BaseDirectory;

        // Deduplicate by simple assembly name: the test host already loads the net10 MAUI +
        // FmgLib assemblies via the project reference, and importing a second copy of any of
        // them would fail the compilation with CS1704.
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

        Add(typeof(Microsoft.Maui.Controls.BindableObject).Assembly.Location);   // Microsoft.Maui.Controls
        Add(typeof(Microsoft.Maui.Thickness).Assembly.Location);                 // Microsoft.Maui (Core)
        Add(typeof(Microsoft.Maui.Graphics.Color).Assembly.Location);            // Microsoft.Maui.Graphics
        Add(typeof(FmgLib.MauiMarkup.IFmgLibHotReload).Assembly.Location);       // FmgLib.MauiMarkup
        Add($"{rootPath}/DLLs/Syncfusion.Maui.Toolkit.dll");

        foreach (var extra in extraReferences)
        {
            Add(extra);
        }

        var references = referencePaths.Values
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToList();

        // MAUI app projects always compile with ImplicitUsings=enable; the generated code
        // relies on those SDK-provided global usings (System, System.Collections.Generic, …).
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

        var compilation = CSharpCompilation.Create("RedefinedPropertyTests",
            new[] { CSharpSyntaxTree.ParseText(source), CSharpSyntaxTree.ParseText(implicitUsings) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver = (CSharpGeneratorDriver)CSharpGeneratorDriver
            .Create(new SourceGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        return (driver.GetRunResult(), outputCompilation, outputCompilation.GetDiagnostics());
    }

    private const string SfButtonUsageSource = """
        using System.Windows.Input;
        using FmgLib.MauiMarkup;
        using Microsoft.Maui;
        using Microsoft.Maui.Controls;
        using Microsoft.Maui.Graphics;
        using Syncfusion.Maui.Toolkit.Buttons;
        using Syncfusion.Maui.Toolkit.Carousel;
        using Syncfusion.Maui.Toolkit.OtpInput;
        using Syncfusion.Maui.Toolkit.TextInputLayout;

        namespace ConsoleApp1;

        [MauiMarkup(typeof(SfButton), typeof(SfTextInputLayout), typeof(SfOtpInput), typeof(SfCarousel))]
        public class Markup { }

        public static class AuthPageUi
        {
            // Mirrors the exact call patterns that regressed in the field.
            public static SfButton PrimaryButton(ICommand command)
                => new SfButton()
                    .Command(command)
                    .HeightRequest(50)
                    .Background(new SolidColorBrush(Colors.Green))
                    .TextColor(Colors.White)
                    .CornerRadius(new CornerRadius(14))
                    .Stroke(new SolidColorBrush(Colors.DarkGreen))
                    .StrokeThickness(1)
                    .FontAttributes(FontAttributes.Bold)
                    .FontSize(15);

            // SfTextInputLayout is NOT a Microsoft.Maui.Controls.ContentView — its Content
            // property comes from Syncfusion's SfContentView base, which must be generated
            // automatically and win over the MAUI ContentViewExtension for this receiver.
            public static SfTextInputLayout Field(Entry entry)
                => new SfTextInputLayout()
                    .Hint("E-mail")
                    .Stroke(Colors.Gray)
                    .FocusedStrokeThickness(2)
                    .UnfocusedStrokeThickness(1)
                    .InputViewPadding(new Thickness(12, 0))
                    .HeightRequest(58)
                    .Content(entry.Placeholder(string.Empty));

            public static SfOtpInput Otp()
                => new SfOtpInput().Length(6);

            public static SfCarousel Carousel()
                => new SfCarousel()
                    .HeightRequest(200)
                    .ItemHeight(148)
                    .ItemWidth(320)
                    .ItemSpacing(12);
        }
        """;

    [Test]
    public void SfButton_UsageWithBaseAndRedefinedProperties_CompilesWithoutErrors()
    {
        var (_, _, diagnostics) = RunGenerator(SfButtonUsageSource);

        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        errors.Should().BeEmpty(string.Join(Environment.NewLine, errors));
    }

    [Test]
    public void SfButton_AnnotatingLeafType_AlsoGeneratesEligibleBaseClasses()
    {
        var (runResult, _, _) = RunGenerator(SfButtonUsageSource);

        runResult.GeneratedTrees
            .Any(tree => tree.FilePath.Contains("ButtonBase", StringComparison.OrdinalIgnoreCase))
            .Should().BeTrue("ButtonBase declares Command/FontSize/FontAttributes and must be generated automatically");
    }

    [Test]
    public void PoisonedReference_DoesNotKillGenerationForOtherTypes()
    {
        // Syncfusion.Maui.DataGrid.dll is referenced WITHOUT its dependencies (GridCommon etc.),
        // so SfDataGrid's base chain contains unresolvable types. One broken type must be
        // reported and skipped — never crash the generator, which would silently erase every
        // generated extension and flood the project with CS1955/CS0311.
        var source = """
            using FmgLib.MauiMarkup;
            using Syncfusion.Maui.DataGrid;
            using Syncfusion.Maui.Toolkit.Buttons;

            namespace ConsoleApp1;

            [MauiMarkup(typeof(SfButton), typeof(SfDataGrid))]
            public class Markup { }
            """;

        var rootPath = AppDomain.CurrentDomain.BaseDirectory;
        var (runResult, _, _) = RunGenerator(source, $"{rootPath}/DLLs/Syncfusion.Maui.DataGrid.dll");

        runResult.Results.Should().OnlyContain(result => result.Exception == null,
            "a generator exception wipes all generated output");

        runResult.GeneratedTrees
            .Any(tree => tree.FilePath.Contains("SfButton", StringComparison.OrdinalIgnoreCase))
            .Should().BeTrue("healthy types must still be generated when another type is broken");

        runResult.GeneratedTrees
            .Any(tree => tree.FilePath.Contains("ButtonBase", StringComparison.OrdinalIgnoreCase))
            .Should().BeTrue();
    }

    [Test]
    public void SfButton_IdenticallyTypedRedefinitions_AreSkippedWithoutNewSuffix()
    {
        var (runResult, _, _) = RunGenerator(SfButtonUsageSource);

        var sfButtonSource = runResult.GeneratedTrees
            .First(tree => tree.FilePath.Contains("SfButton", StringComparison.OrdinalIgnoreCase))
            .ToString();

        sfButtonSource.Should().NotContain("TextColorNew");
        sfButtonSource.Should().NotContain("StrokeThicknessNew");
        sfButtonSource.Should().NotContain("CornerRadiusNew");
        sfButtonSource.Should().NotContain("BackgroundNew");

        // The redefinitions are identical to ButtonBase's — SfButton must not duplicate them.
        sfButtonSource.Should().NotContain("public static T TextColor<T>");

        var buttonBaseSource = runResult.GeneratedTrees
            .First(tree => tree.FilePath.Contains("ButtonBase", StringComparison.OrdinalIgnoreCase))
            .ToString();

        buttonBaseSource.Should().Contain("public static T TextColor<T>");
        buttonBaseSource.Should().Contain("public static T Command<T>");
        buttonBaseSource.Should().Contain("public static T FontSize<T>");
    }
}
