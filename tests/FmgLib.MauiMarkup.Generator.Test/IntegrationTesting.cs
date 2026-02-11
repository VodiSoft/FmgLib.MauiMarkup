using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace FmgLib.MauiMarkup.Generator.Test;

[TestFixture]
public class IntegrationTesting
{
    private static (Compilation, ImmutableArray<Diagnostic>) CreateCompilation(string source, params string[] additionalReferences)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();

        foreach (var reference in additionalReferences)
        {
            references.Add(MetadataReference.CreateFromFile(reference));
        }

        var compilation = CSharpCompilation.Create("SourceGeneratorTests",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return (compilation, compilation.GetDiagnostics());
    }

    [Test]
    public void TestSourceGenerator()
    {
        var rootPath = AppDomain.CurrentDomain.BaseDirectory;
        var source = @"
using InputKit.Shared.Controls;
using UraniumUI.Material.Controls;
using Microsoft.Maui.Controls;
using FmgLib.MauiMarkup;
using DevExpress.Maui.Core.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;
using DXImage = DevExpress.Maui.Core.DXImage;
using SwipeItem = DevExpress.Maui.CollectionView.SwipeItem;
using CommunityToolkit.Maui.Behaviors;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Maui;
using DevExpress.Maui.Controls;
using DevExpress.Maui.Editors;
using DevExpress.Maui.CollectionView;
using DevExpress.Maui.Core;
using DevExpress.Maui;
using PanCardView;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SkiaSharp.Extended.UI.Controls;

namespace ConsoleApp1;

[MauiMarkup(typeof(Syncfusion.Maui.Core.SfAvatarView))]
public class FileName
{ }";

        var additionalReferences = CreateSharedReferences(rootPath)
            .Concat(new[]
            {
                @$"{rootPath}\DLLs\InputKit.Maui.dll",
                @$"{rootPath}\DLLs\Plainer.Maui.dll",
                @$"{rootPath}\DLLs\UraniumUI.dll",
                @$"{rootPath}\DLLs\UraniumUI.Material.dll",
                @$"{rootPath}\DLLs\DevExpress.Data.v23.2.dll",
                @$"{rootPath}\DLLs\DevExpress.Maui.CollectionView.dll",
                @$"{rootPath}\DLLs\DevExpress.Maui.Controls.dll",
                @$"{rootPath}\DLLs\DevExpress.Maui.Core.dll",
                @$"{rootPath}\DLLs\DevExpress.Maui.Editors.dll",
                @$"{rootPath}\DLLs\LiveChartsCore.Behaviours.dll",
                @$"{rootPath}\DLLs\LiveChartsCore.dll",
                @$"{rootPath}\DLLs\LiveChartsCore.SkiaSharpView.dll",
                @$"{rootPath}\DLLs\LiveChartsCore.SkiaSharpView.Maui.dll",
                @$"{rootPath}\DLLs\SkiaSharp.dll",
                @$"{rootPath}\DLLs\SkiaSharp.HarfBuzz.dll",
                @$"{rootPath}\DLLs\SkiaSharp.Views.Maui.Controls.Compatibility.dll",
                @$"{rootPath}\DLLs\SkiaSharp.Views.Maui.Controls.dll",
                @$"{rootPath}\DLLs\SkiaSharp.Views.Maui.Core.dll",
                @$"{rootPath}\DLLs\Microsoft.Maui.Controls.Compatibility.dll",
                @$"{rootPath}\DLLs\CommunityToolkit.Maui.Core.dll",
                @$"{rootPath}\DLLs\CommunityToolkit.Maui.dll",
                @$"{rootPath}\DLLs\PanCardView.dll",
                @$"{rootPath}\DLLs\SkiaSharp.Extended.dll",
                @$"{rootPath}\DLLs\SkiaSharp.Extended.UI.dll",
                @$"{rootPath}\DLLs\SkiaSharp.SceneGraph.dll",
                @$"{rootPath}\DLLs\SkiaSharp.Skottie.dll"
            })
            .ToArray();

        var (compilation, _) = CreateCompilation(source, additionalReferences);

        var generator = new SourceGenerator();
        var driver = CreateDriver(generator);

        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics2);

        var runResult = driver.GetRunResult();
        runResult.GeneratedTrees.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Test]
    public void TestSourceGenerator_AutoModeWithProjectProperty()
    {
        var rootPath = AppDomain.CurrentDomain.BaseDirectory;
        var source = @"
using FmgLib.MauiMarkup;

namespace ConsoleApp1;
public class FileName
{ }";

        var additionalReferences = CreateSharedReferences(rootPath);
        var (compilation, _) = CreateCompilation(source, additionalReferences);

        var generator = new SourceGenerator();
        var driver = CreateDriver(generator, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["build_property.MauiMarkupSourceGenerator"] = "true"
        });

        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics2);

        var runResult = driver.GetRunResult();
        runResult.GeneratedTrees.Any(tree => tree.FilePath.Contains("SfAvatarView", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Test]
    public void TestSourceGenerator_AvatarView_DoesNotGenerateInvalidITextAlignmentPropertyAccess()
    {
        var rootPath = AppDomain.CurrentDomain.BaseDirectory;
        var source = @"
using CommunityToolkit.Maui.Views;
using FmgLib.MauiMarkup;

namespace ConsoleApp1;

[MauiMarkup(typeof(AvatarView))]
public class FileName
{ }";

        var additionalReferences = CreateSharedReferences(rootPath)
            .Concat(new[]
            {
                @$"{rootPath}\DLLs\CommunityToolkit.Maui.Core.dll",
                @$"{rootPath}\DLLs\CommunityToolkit.Maui.dll"
            })
            .ToArray();

        var (compilation, _) = CreateCompilation(source, additionalReferences);

        var generator = new SourceGenerator();
        var driver = CreateDriver(generator);
        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var hasInvalidAlignmentPropertyError = outputCompilation
            .GetDiagnostics()
            .Any(d => d.Id == "CS0117" && d.GetMessage().Contains("TextAlignmentProperty"));

        hasInvalidAlignmentPropertyError.Should().BeFalse();
    }

    private static string[] CreateSharedReferences(string rootPath)
    {
        return
        [
            @$"{rootPath}\DLLs\FmgLib.MauiMarkup.dll",
            @$"{rootPath}\DLLs\Microsoft.Extensions.Configuration.Abstractions.dll",
            @$"{rootPath}\DLLs\Microsoft.Extensions.Configuration.dll",
            @$"{rootPath}\DLLs\Microsoft.Extensions.DependencyInjection.Abstractions.dll",
            @$"{rootPath}\DLLs\Microsoft.Extensions.DependencyInjection.dll",
            @$"{rootPath}\DLLs\Microsoft.Extensions.Logging.Abstractions.dll",
            @$"{rootPath}\DLLs\Microsoft.Extensions.Logging.dll",
            @$"{rootPath}\DLLs\Microsoft.Extensions.Options.dll",
            @$"{rootPath}\DLLs\Microsoft.Extensions.Primitives.dll",
            @$"{rootPath}\DLLs\Microsoft.Maui.Controls.dll",
            @$"{rootPath}\DLLs\Microsoft.Maui.Controls.Xaml.dll",
            @$"{rootPath}\DLLs\Microsoft.Maui.dll",
            @$"{rootPath}\DLLs\Microsoft.Maui.Essentials.dll",
            @$"{rootPath}\DLLs\Microsoft.Maui.Graphics.dll",
            @$"{rootPath}\DLLs\Syncfusion.Licensing.dll",
            @$"{rootPath}\DLLs\Syncfusion.Maui.Core.dll",
            @$"{rootPath}\DLLs\Syncfusion.Maui.Data.dll",
            @$"{rootPath}\DLLs\Syncfusion.Maui.DataGrid.dll",
            @$"{rootPath}\DLLs\Syncfusion.Maui.DataSource.dll",
            @$"{rootPath}\DLLs\Syncfusion.Maui.GridCommon.dll",
            @$"{rootPath}\DLLs\Syncfusion.Maui.Inputs.dll",
            @$"{rootPath}\DLLs\Syncfusion.Maui.ListView.dll",
            @$"{rootPath}\DLLs\Syncfusion.Maui.PullToRefresh.dll"
        ];
    }

    private static CSharpGeneratorDriver CreateDriver(SourceGenerator generator, IDictionary<string, string>? globalOptions = null)
    {
        var driver = CSharpGeneratorDriver.Create(generator);
        if (globalOptions is null)
        {
            return driver;
        }

        return (CSharpGeneratorDriver)driver.WithUpdatedAnalyzerConfigOptions(new TestAnalyzerConfigOptionsProvider(globalOptions));
    }

    private sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly AnalyzerConfigOptions globalOptions;
        private static readonly AnalyzerConfigOptions EmptyOptions = new TestAnalyzerConfigOptions(new Dictionary<string, string>());

        public TestAnalyzerConfigOptionsProvider(IDictionary<string, string> values)
        {
            globalOptions = new TestAnalyzerConfigOptions(values);
        }

        public override AnalyzerConfigOptions GlobalOptions => globalOptions;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => EmptyOptions;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => EmptyOptions;
    }

    private sealed class TestAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly IReadOnlyDictionary<string, string> values;

        public TestAnalyzerConfigOptions(IDictionary<string, string> values)
        {
            this.values = new Dictionary<string, string>(values, StringComparer.OrdinalIgnoreCase);
        }

        public override bool TryGetValue(string key, out string value)
        {
            if (values.TryGetValue(key, out var found))
            {
                value = found;
                return true;
            }

            value = string.Empty;
            return false;
        }
    }
}
