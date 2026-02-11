using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using FmgLib.MauiMarkup.Generator.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

#pragma warning disable RS2008, RS1032

namespace FmgLib.MauiMarkup;

/// <summary>
/// Generates fluent extension helpers for types marked with <c>MauiMarkup</c> or <c>MauiMarkupAttachedProp</c> attributes.
/// Uses the incremental generator pipeline for significantly improved performance in large solutions.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class SourceGenerator : IIncrementalGenerator
{
    private const string AutoGeneratePropertyName = "build_property.MauiMarkupSourceGenerator";
    private const string MauiMarkupAttributeName = "MauiMarkupAttribute";
    private const string MauiMarkupShortName = "MauiMarkup";
    private const string MauiMarkupAttachedPropAttributeName = "MauiMarkupAttachedPropAttribute";
    private const string MauiMarkupAttachedPropShortName = "MauiMarkupAttachedProp";
    private static readonly IEqualityComparer<INamedTypeSymbol> TypeSymbolComparer = SymbolEqualityComparer.Default;

    private static readonly DiagnosticDescriptor InvalidAttributeDescriptor = new(
        id: "FMMG001",
        title: "Invalid MauiMarkup attribute usage",
        messageFormat: "The attribute '{0}' has invalid arguments and has been ignored",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Executes the <c>Initialize</c> operation.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static output => AddAttributeDefinitions(output));

        var candidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                static (syntaxContext, cancellationToken) => GetCandidateType(syntaxContext, cancellationToken))
            .Where(static symbol => symbol is not null)
            .Select(static (symbol, _) => symbol!);

        var compilationAndTypes = context.CompilationProvider.Combine(candidates.Collect());
        var autoGenerationEnabled = context.AnalyzerConfigOptionsProvider
            .Select(static (optionsProvider, _) => IsAutoGenerationEnabled(optionsProvider));

        var generationInput = compilationAndTypes.Combine(autoGenerationEnabled);

        context.RegisterSourceOutput(generationInput, static (productionContext, source) =>
        {
            ExecuteGeneration(source.Left.Left, source.Left.Right, source.Right, productionContext);
        });
    }

    /// <summary>
    /// Gets the value produced by the <c>GetCandidateType</c> operation.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    /// <param name="cancellationToken">The value used for <paramref name="cancellationToken"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    private static INamedTypeSymbol? GetCandidateType(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        if (!ContainsMauiMarkupAttribute(classDeclaration))
        {
            return null;
        }

        return context.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken) as INamedTypeSymbol;
    }

    /// <summary>
    /// Executes the <c>ContainsMauiMarkupAttribute</c> operation.
    /// </summary>
    /// <param name="classDeclaration">The value used for <paramref name="classDeclaration"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool ContainsMauiMarkupAttribute(ClassDeclarationSyntax classDeclaration)
    {
        foreach (var attributeList in classDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (IsGeneratorAttribute(attribute.Name))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Executes the <c>ExecuteGeneration</c> operation.
    /// </summary>
    /// <param name="compilation">The value used for <paramref name="compilation"/>.</param>
    /// <param name="candidateTypes">The value used for <paramref name="candidateTypes"/>.</param>
    /// <param name="autoGenerateForThirdPartyControls">The value used for <paramref name="autoGenerateForThirdPartyControls"/>.</param>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    private static void ExecuteGeneration(Compilation compilation, ImmutableArray<INamedTypeSymbol> candidateTypes, bool autoGenerateForThirdPartyControls, SourceProductionContext context)
    {
        if (candidateTypes.IsDefaultOrEmpty && !autoGenerateForThirdPartyControls)
        {
            return;
        }

        var generatedTargets = new HashSet<INamedTypeSymbol>(TypeSymbolComparer);

        foreach (var generatorTypeSymbol in candidateTypes.Distinct(TypeSymbolComparer))
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            GenerateForMauiMarkupAttributes(compilation, generatorTypeSymbol, generatedTargets, context);
            GenerateForAttachedAttributes(compilation, generatorTypeSymbol, context);
        }

        if (autoGenerateForThirdPartyControls)
        {
            GenerateForThirdPartyControls(compilation, generatedTargets, context);
        }
    }

    /// <summary>
    /// Generates output for the <c>GenerateForMauiMarkupAttributes</c> operation.
    /// </summary>
    /// <param name="compilation">The value used for <paramref name="compilation"/>.</param>
    /// <param name="generatorTypeSymbol">The value used for <paramref name="generatorTypeSymbol"/>.</param>
    /// <param name="generatedTargets">The value used for <paramref name="generatedTargets"/>.</param>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    private static void GenerateForMauiMarkupAttributes(Compilation compilation, INamedTypeSymbol generatorTypeSymbol, ISet<INamedTypeSymbol> generatedTargets, SourceProductionContext context)
    {
        var suffixes = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var attribute in generatorTypeSymbol.GetAttributes().Where(IsMauiMarkupAttribute))
        {
            foreach (var targetType in ExtractTypesFromAttribute(attribute).Distinct(TypeSymbolComparer))
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                if (!generatedTargets.Add(targetType))
                {
                    continue;
                }

                var (hintName, sourceText, generated) = new ExtensionGenerator(compilation, targetType, context.CancellationToken).Build();
                if (!generated)
                {
                    continue;
                }

                var uniqueName = AllocateHintName(suffixes, hintName);
                context.AddSource($"{uniqueName}.g.cs", sourceText);
            }
        }
    }

    /// <summary>
    /// Generates output for the <c>GenerateForThirdPartyControls</c> operation.
    /// </summary>
    /// <param name="compilation">The value used for <paramref name="compilation"/>.</param>
    /// <param name="generatedTargets">The value used for <paramref name="generatedTargets"/>.</param>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    private static void GenerateForThirdPartyControls(Compilation compilation, ISet<INamedTypeSymbol> generatedTargets, SourceProductionContext context)
    {
        var bindableObjectType = compilation.FindNamedType("Microsoft.Maui.Controls.BindableObject");
        if (bindableObjectType is null)
        {
            return;
        }

        var suffixes = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var reference in compilation.References)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (compilation.GetAssemblyOrModuleSymbol(reference) is not IAssemblySymbol assemblySymbol)
            {
                continue;
            }

            if (IsExcludedAssembly(assemblySymbol))
            {
                continue;
            }

            foreach (var targetType in EnumerateAllTypes(assemblySymbol.GlobalNamespace))
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                if (!IsEligibleAutoGenerationType(targetType, bindableObjectType))
                {
                    continue;
                }

                if (!generatedTargets.Add(targetType))
                {
                    continue;
                }

                var (hintName, sourceText, generated) = new ExtensionGenerator(compilation, targetType, context.CancellationToken).Build();
                if (!generated)
                {
                    continue;
                }

                var uniqueName = AllocateHintName(suffixes, hintName);
                context.AddSource($"{uniqueName}.g.cs", sourceText);
            }
        }
    }

    /// <summary>
    /// Generates output for the <c>GenerateForAttachedAttributes</c> operation.
    /// </summary>
    /// <param name="compilation">The value used for <paramref name="compilation"/>.</param>
    /// <param name="generatorTypeSymbol">The value used for <paramref name="generatorTypeSymbol"/>.</param>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    private static void GenerateForAttachedAttributes(Compilation compilation, INamedTypeSymbol generatorTypeSymbol, SourceProductionContext context)
    {
        var suffixes = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var attribute in generatorTypeSymbol.GetAttributes().Where(IsMauiMarkupAttachedPropAttribute))
        {
            if (!TryCreateAttachedModel(attribute, out var attachedModel))
            {
                var location = attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken)?.GetLocation();
                context.ReportDiagnostic(Diagnostic.Create(InvalidAttributeDescriptor, location, MauiMarkupAttachedPropAttributeName));
                continue;
            }

            var (hintName, sourceText, generated) = new ExtensionGenerator(compilation, attachedModel, context.CancellationToken).Build();
            if (!generated)
            {
                continue;
            }

            var uniqueName = AllocateHintName(suffixes, hintName);
            context.AddSource($"{uniqueName}.g.cs", sourceText);
        }
    }

    /// <summary>
    /// Executes the <c>ExtractTypesFromAttribute</c> operation.
    /// </summary>
    /// <param name="attributeData">The value used for <paramref name="attributeData"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    private static IEnumerable<INamedTypeSymbol> ExtractTypesFromAttribute(AttributeData attributeData)
    {
        if (attributeData.ConstructorArguments.Length == 0)
        {
            return Enumerable.Empty<INamedTypeSymbol>();
        }

        var typedArgument = attributeData.ConstructorArguments[0];
        if (typedArgument.Kind != TypedConstantKind.Array || typedArgument.Values.IsDefaultOrEmpty)
        {
            return Enumerable.Empty<INamedTypeSymbol>();
        }

        var symbols = new List<INamedTypeSymbol>(typedArgument.Values.Length);
        foreach (var typedConstant in typedArgument.Values)
        {
            if (typedConstant.Value is INamedTypeSymbol namedTypeSymbol)
            {
                symbols.Add(namedTypeSymbol);
            }
            else if (typedConstant.Value is ITypeSymbol typeSymbol && typeSymbol is INamedTypeSymbol namedType)
            {
                symbols.Add(namedType);
            }
        }

        return symbols;
    }

    /// <summary>
    /// Attempts to execute the <c>TryCreateAttachedModel</c> operation.
    /// </summary>
    /// <param name="attributeData">The value used for <paramref name="attributeData"/>.</param>
    /// <param name="attachedModel">The value used for <paramref name="attachedModel"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool TryCreateAttachedModel(AttributeData attributeData, out AttachedModel attachedModel)
    {
        attachedModel = default!;
        if (attributeData.ConstructorArguments.Length < 4)
        {
            return false;
        }

        var mainSymbol = attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;
        var propertyName = attributeData.ConstructorArguments[1].Value?.ToString();
        var returnType = attributeData.ConstructorArguments[2].Value as INamedTypeSymbol;
        var declaringType = attributeData.ConstructorArguments[3].Value as INamedTypeSymbol;

        if (mainSymbol is null || string.IsNullOrWhiteSpace(propertyName) || returnType is null || declaringType is null)
        {
            return false;
        }

        attachedModel = new AttachedModel
        {
            MainSymbol = mainSymbol,
            ReturnTypeName = returnType.GetFullyQualifiedName(),
            DeclaringTypeName = declaringType.GetFullyQualifiedName(),
            PropertyName = propertyName!
        };

        return true;
    }

    /// <summary>
    /// Evaluates whether the <c>IsMauiMarkupAttribute</c> condition is satisfied.
    /// </summary>
    /// <param name="attributeData">The value used for <paramref name="attributeData"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool IsMauiMarkupAttribute(AttributeData attributeData)
    {
        var name = attributeData.AttributeClass?.Name;
        return name is MauiMarkupAttributeName or MauiMarkupShortName;
    }

    /// <summary>
    /// Evaluates whether the <c>IsMauiMarkupAttachedPropAttribute</c> condition is satisfied.
    /// </summary>
    /// <param name="attributeData">The value used for <paramref name="attributeData"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool IsMauiMarkupAttachedPropAttribute(AttributeData attributeData)
    {
        var name = attributeData.AttributeClass?.Name;
        return name is MauiMarkupAttachedPropAttributeName or MauiMarkupAttachedPropShortName;
    }

    /// <summary>
    /// Evaluates whether the <c>IsGeneratorAttribute</c> condition is satisfied.
    /// </summary>
    /// <param name="nameSyntax">The value used for <paramref name="nameSyntax"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool IsGeneratorAttribute(NameSyntax nameSyntax)
    {
        var identifier = nameSyntax switch
        {
            IdentifierNameSyntax identifierName => identifierName.Identifier.ValueText,
            QualifiedNameSyntax qualifiedName => qualifiedName.Right.Identifier.ValueText,
            AliasQualifiedNameSyntax aliasName => aliasName.Name.Identifier.ValueText,
            _ => nameSyntax.ToString()
        };

        return identifier is MauiMarkupAttributeName or MauiMarkupShortName or MauiMarkupAttachedPropAttributeName or MauiMarkupAttachedPropShortName;
    }

    /// <summary>
    /// Evaluates whether the <c>IsAutoGenerationEnabled</c> condition is satisfied.
    /// </summary>
    /// <param name="optionsProvider">The value used for <paramref name="optionsProvider"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool IsAutoGenerationEnabled(AnalyzerConfigOptionsProvider optionsProvider)
    {
        if (!TryGetAutoGenerationPropertyValue(optionsProvider.GlobalOptions, out var rawValue))
        {
            return false;
        }

        return IsTrue(rawValue);
    }

    /// <summary>
    /// Attempts to execute the <c>TryGetAutoGenerationPropertyValue</c> operation.
    /// </summary>
    /// <param name="options">The value used for <paramref name="options"/>.</param>
    /// <param name="rawValue">The value used for <paramref name="rawValue"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool TryGetAutoGenerationPropertyValue(AnalyzerConfigOptions options, out string? rawValue)
    {
        if (options.TryGetValue(AutoGeneratePropertyName, out rawValue))
        {
            return true;
        }

        if (options.TryGetValue("build_property.mauimarkupsourcegenerator", out rawValue))
        {
            return true;
        }

        if (options.TryGetValue("MauiMarkupSourceGenerator", out rawValue))
        {
            return true;
        }

        if (options.TryGetValue("mauimarkupsourcegenerator", out rawValue))
        {
            return true;
        }

        rawValue = null;
        return false;
    }

    /// <summary>
    /// Evaluates whether the <c>IsTrue</c> condition is satisfied.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool IsTrue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return value!.Equals("true", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("on", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("enable", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("enabled", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Evaluates whether the <c>IsExcludedAssembly</c> condition is satisfied.
    /// </summary>
    /// <param name="assemblySymbol">The value used for <paramref name="assemblySymbol"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool IsExcludedAssembly(IAssemblySymbol assemblySymbol)
    {
        var name = assemblySymbol.Name;

        return name.StartsWith("Microsoft.Maui", StringComparison.Ordinal) ||
               name.StartsWith("FmgLib.MauiMarkup", StringComparison.Ordinal) ||
               name.StartsWith("System", StringComparison.Ordinal) ||
               name.Equals("mscorlib", StringComparison.Ordinal) ||
               name.Equals("netstandard", StringComparison.Ordinal);
    }

    /// <summary>
    /// Evaluates whether the <c>IsEligibleAutoGenerationType</c> condition is satisfied.
    /// </summary>
    /// <param name="typeSymbol">The value used for <paramref name="typeSymbol"/>.</param>
    /// <param name="bindableObjectType">The value used for <paramref name="bindableObjectType"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    private static bool IsEligibleAutoGenerationType(INamedTypeSymbol typeSymbol, INamedTypeSymbol bindableObjectType)
    {
        if (typeSymbol.TypeKind != TypeKind.Class ||
            typeSymbol.IsGenericType ||
            typeSymbol.DeclaredAccessibility != Accessibility.Public)
        {
            return false;
        }

        if (SymbolEqualityComparer.Default.Equals(typeSymbol, bindableObjectType))
        {
            return false;
        }

        return Helpers.IsBindableObject(typeSymbol);
    }

    /// <summary>
    /// Executes the <c>EnumerateAllTypes</c> operation.
    /// </summary>
    /// <param name="namespaceSymbol">The value used for <paramref name="namespaceSymbol"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    private static IEnumerable<INamedTypeSymbol> EnumerateAllTypes(INamespaceSymbol namespaceSymbol)
    {
        foreach (var namespaceMember in namespaceSymbol.GetNamespaceMembers())
        {
            foreach (var type in EnumerateAllTypes(namespaceMember))
            {
                yield return type;
            }
        }

        foreach (var typeMember in namespaceSymbol.GetTypeMembers())
        {
            foreach (var type in EnumerateNestedTypes(typeMember))
            {
                yield return type;
            }
        }
    }

    /// <summary>
    /// Executes the <c>EnumerateNestedTypes</c> operation.
    /// </summary>
    /// <param name="typeSymbol">The value used for <paramref name="typeSymbol"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    private static IEnumerable<INamedTypeSymbol> EnumerateNestedTypes(INamedTypeSymbol typeSymbol)
    {
        yield return typeSymbol;

        foreach (var nestedType in typeSymbol.GetTypeMembers())
        {
            foreach (var type in EnumerateNestedTypes(nestedType))
            {
                yield return type;
            }
        }
    }

    /// <summary>
    /// Executes the <c>AllocateHintName</c> operation.
    /// </summary>
    /// <param name="suffixes">The value used for <paramref name="suffixes"/>.</param>
    /// <param name="baseName">The value used for <paramref name="baseName"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    private static string AllocateHintName(IDictionary<string, int> suffixes, string baseName)
    {
        if (!suffixes.TryGetValue(baseName, out var index))
        {
            suffixes[baseName] = 0;
            return baseName;
        }

        index++;
        suffixes[baseName] = index;
        return $"{baseName}{index}";
    }

    /// <summary>
    /// Executes the <c>AddAttributeDefinitions</c> operation.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    private static void AddAttributeDefinitions(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("MauiMarkupAttribute.g.cs", @"//
// <auto-generated-fmglib-mauimarkup-generator />
//

using System;

namespace FmgLib.MauiMarkup;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class MauiMarkupAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the MauiMarkupAttribute class.
    /// </summary>
    /// <param name=""nativeControlTypes"">The target control types.</param>
    public MauiMarkupAttribute(params Type[] nativeControlTypes) { }
}

");

        context.AddSource("MauiMarkupAttachedPropAttribute.g.cs", @"//
// <auto-generated-fmglib-mauimarkup-generator />
//

using System;

namespace FmgLib.MauiMarkup;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class MauiMarkupAttachedPropAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the MauiMarkupAttachedPropAttribute class.
    /// </summary>
    /// <param name=""controlType"">The target control type.</param>
    /// <param name=""propertyName"">The attached property name.</param>
    /// <param name=""returnType"">The attached property value type.</param>
    /// <param name=""declaringType"">The type that declares the attached property.</param>
    public MauiMarkupAttachedPropAttribute(Type controlType, string propertyName, Type returnType, Type declaringType) { }
}

");
    }
}
