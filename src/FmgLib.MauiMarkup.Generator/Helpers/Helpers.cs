using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FmgLib.MauiMarkup;

/// <summary>
/// Shared helper utilities used by the source generator.
/// </summary>
public static class Helpers
{
    public static readonly HashSet<string> NotGenerateList = new(StringComparer.Ordinal) { "this[]", "Handler", "LogicalChildren" };

    /// <summary>
    /// C# reserved keywords: identifiers generated from property names (e.g. parameter names)
    /// must be escaped with <c>@</c> when they collide with one of these.
    /// </summary>
    private static readonly HashSet<string> CSharpKeywords = new(StringComparer.Ordinal)
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
        "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
        "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
        "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
        "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
        "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
        "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw",
        "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
        "virtual", "void", "volatile", "while"
    };

    /// <summary>
    /// The display format every type name in generated code is printed with.
    ///
    /// Generated files are compiled inside the CONSUMER's project, whose <c>using</c> directives the
    /// generator has no control over: <c>ImplicitUsings</c> may be disabled, and a project is free
    /// to declare its own <c>Controls</c> or <c>Graphics</c> namespace that shadows the intended
    /// one. Emitting every type as a <c>global::</c>-rooted name makes the output independent of
    /// both — the code resolves identically whatever the surrounding file imports.
    ///
    /// This is <see cref="SymbolDisplayFormat.FullyQualifiedFormat"/> plus the nullable reference
    /// modifier, which that format omits: without it a <c>string?</c> property would generate a
    /// non-nullable <c>string</c> parameter and warn at every call site that passes null.
    /// </summary>
    public static readonly SymbolDisplayFormat QualifiedTypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
        .WithMiscellaneousOptions(
            SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
            SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
            SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    /// <summary>
    /// Prints a symbol the way generated code must reference it — see <see cref="QualifiedTypeFormat"/>.
    /// </summary>
    /// <param name="symbol">The symbol to print.</param>
    /// <returns>A <c>global::</c>-rooted, using-independent type name.</returns>
    public static string ToQualifiedName(this ISymbol symbol) => symbol.ToDisplayString(QualifiedTypeFormat);

    /// <summary>
    /// Executes the <c>ToCamelCase</c> operation.
    /// </summary>
    /// <param name="text">The value used for <paramref name="text"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public static string ToCamelCase(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var result = char.ToLowerInvariant(text[0]) + text[1..];
        return CSharpKeywords.Contains(result) ? "@" + result : result;
    }

    /// <summary>
    /// Executes the <c>FindNamedType</c> operation.
    /// </summary>
    /// <param name="compilation">The value used for <paramref name="compilation"/>.</param>
    /// <param name="typeMetadataName">The value used for <paramref name="typeMetadataName"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public static INamedTypeSymbol? FindNamedType(this Compilation compilation, string typeMetadataName)
    {
        var typeToMauiMarkup = compilation.GetTypeByMetadataName(typeMetadataName);

        typeToMauiMarkup ??= compilation.References
            .Select(compilation.GetAssemblyOrModuleSymbol)
            .OfType<IAssemblySymbol>()
            .Select(assemblySymbol => assemblySymbol.GetTypeByMetadataName(typeMetadataName))
            .FirstOrDefault(static symbol => symbol is not null);

        return typeToMauiMarkup;
    }

    /// <summary>
    /// Gets the value produced by the <c>GetFullyQualifiedName</c> operation.
    /// </summary>
    /// <param name="typeSymbol">The value used for <paramref name="typeSymbol"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public static string GetFullyQualifiedName(this ISymbol typeSymbol)
    {
        return typeSymbol.ToString();
    }

    /// <summary>
    /// Executes the <c>EnsureNotNull</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public static T EnsureNotNull<T>(this T? value)
        => value ?? throw new InvalidOperationException();

    /// <summary>
    /// Evaluates whether the <c>IsVisualElement</c> condition is satisfied.
    /// </summary>
    /// <param name="symbol">The value used for <paramref name="symbol"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    public static bool IsVisualElement(INamedTypeSymbol symbol)
    {
        return IsDerivedFrom(symbol, "Microsoft.Maui.Controls.VisualElement");
    }

    /// <summary>
    /// Evaluates whether the <c>IsBindableObject</c> condition is satisfied.
    /// </summary>
    /// <param name="symbol">The value used for <paramref name="symbol"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    public static bool IsBindableObject(INamedTypeSymbol symbol)
    {
        return IsDerivedFrom(symbol, "Microsoft.Maui.Controls.BindableObject");
    }

    /// <summary>
    /// Executes the <c>LoopDownToObject</c> operation.
    /// </summary>
    /// <param name="symbol">The value used for <paramref name="symbol"/>.</param>
    /// <param name="func">The value used for <paramref name="func"/>.</param>
    public static void LoopDownToObject(INamedTypeSymbol symbol, Func<INamedTypeSymbol, bool> func)
    {
        for (var type = symbol; type != null && type.SpecialType != SpecialType.System_Object; type = type.BaseType)
        {
            if (func(type))
            {
                break;
            }
        }
    }

    /// <summary>
    /// Evaluates whether the <c>IsBaseImplementationOfInterface</c> condition is satisfied.
    /// </summary>
    /// <param name="symbol">The value used for <paramref name="symbol"/>.</param>
    /// <param name="name">The value used for <paramref name="name"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    public static bool IsBaseImplementationOfInterface(INamedTypeSymbol symbol, string name)
    {
        var count = 0;
        for (var type = symbol; type != null && type.SpecialType != SpecialType.System_Object; type = type.BaseType)
        {
            if (type.Interfaces.Any(interfaceSymbol => interfaceSymbol.Name.Equals(name, StringComparison.Ordinal)))
            {
                count++;
            }
        }

        return count == 1;
    }

    /// <summary>
    /// Gets the value produced by the <c>GetNormalizedFileName</c> operation.
    /// </summary>
    /// <param name="type">The value used for <paramref name="type"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public static string GetNormalizedFileName(INamedTypeSymbol type)
    {
        var tail = type.IsGenericType ? $".{type.TypeArguments.FirstOrDefault()?.Name}" : string.Empty;
        return $"{type.Name}{tail}";
    }

    /// <summary>
    /// Gets the value produced by the <c>GetNormalizedClassName</c> operation.
    /// </summary>
    /// <param name="type">The value used for <paramref name="type"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public static string GetNormalizedClassName(INamedTypeSymbol type)
    {
        var tail = type.IsGenericType ? $"Of{type.TypeArguments.FirstOrDefault()?.Name}" : string.Empty;
        var fullName = type.ToDisplayString();
        var prefix = fullName.IndexOf(".Shapes.", StringComparison.Ordinal) >= 0 ? "Shapes" : string.Empty;
        prefix += fullName.IndexOf(".Compatibility.", StringComparison.Ordinal) >= 0 ? "Compatibility" : string.Empty;
        return $"{prefix}{type.Name}{tail}";
    }

    /// <summary>
    /// Gets the value produced by the <c>GetFullyQualifiedName</c> operation.
    /// </summary>
    /// <param name="classDeclarationSyntax">The value used for <paramref name="classDeclarationSyntax"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public static string? GetFullyQualifiedName(this ClassDeclarationSyntax classDeclarationSyntax)
    {
        if (!TryGetNamespace(classDeclarationSyntax, out string? namespaceName))
        {
            return null;
        }

        return namespaceName + "." + classDeclarationSyntax.Identifier;
    }

    /// <summary>
    /// Attempts to execute the <c>TryGetNamespace</c> operation.
    /// </summary>
    /// <param name="syntaxNode">The value used for <paramref name="syntaxNode"/>.</param>
    /// <param name="result">The value used for <paramref name="result"/>.</param>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetNamespace(SyntaxNode? syntaxNode, out string? result)
    {
        result = null;

        if (syntaxNode == null)
        {
            return false;
        }

        try
        {
            syntaxNode = syntaxNode.Parent;

            if (syntaxNode == null)
            {
                return false;
            }

            if (syntaxNode is NamespaceDeclarationSyntax namespaceDeclarationSyntax)
            {
                result = namespaceDeclarationSyntax.Name.ToString();
                return true;
            }

            if (syntaxNode is FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDeclarationSyntax)
            {
                result = fileScopedNamespaceDeclarationSyntax.Name.ToString();
                return true;
            }

            return TryGetNamespace(syntaxNode, out result);
        }
        catch
        {
            return false;
        }
    }

    static bool IsDerivedFrom(INamedTypeSymbol symbol, string fullyQualifiedMetadataName)
    {
        for (var type = symbol; type != null && type.SpecialType != SpecialType.System_Object; type = type.BaseType)
        {
            if (type.GetFullyQualifiedName().Equals(fullyQualifiedMetadataName, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Evaluates whether the symbol is marked <c>[Obsolete(message, error: true)]</c>.
    ///
    /// A plain obsolete member only warns (and generated files suppress CS0612/CS0618), but an
    /// error-level one makes every mention of it CS0619 — an error no <c>#pragma</c> can turn off.
    /// Anything generated around such a symbol would break the consuming build, so it is skipped.
    /// </summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns><see langword="true"/> when using the symbol is a compile error.</returns>
    public static bool IsObsoleteAsError(ISymbol symbol)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass?.Name is not ("ObsoleteAttribute" or "Obsolete"))
            {
                continue;
            }

            if (attribute.ConstructorArguments.Length >= 2 &&
                attribute.ConstructorArguments[1].Value is bool isError &&
                isError)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Evaluates whether a type — or any of its constituent types — is obsolete-as-error, so that
    /// it can never appear in a generated signature.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><see langword="true"/> when the type cannot be referenced without a compile error.</returns>
    public static bool IsUnusableType(ITypeSymbol type)
    {
        switch (type)
        {
            case IArrayTypeSymbol array:
                return IsUnusableType(array.ElementType);

            case INamedTypeSymbol named:
                for (var current = named; current != null; current = current.ContainingType)
                {
                    if (IsObsoleteAsError(current))
                    {
                        return true;
                    }
                }

                foreach (var typeArgument in named.TypeArguments)
                {
                    if (IsUnusableType(typeArgument))
                    {
                        return true;
                    }
                }

                return false;

            default:
                return false;
        }
    }

    /// <summary>
    /// Evaluates whether <paramref name="type"/> is visible from any external assembly — i.e.
    /// safe to use as a parameter/return type on a <c>public</c> generated extension method.
    ///
    /// Third-party libraries occasionally expose a <c>public</c> property or event whose TYPE
    /// (or one of its generic type arguments, or its array element type) is <c>internal</c> or a
    /// nested private/protected type — a deliberate "settable internally, readable externally"
    /// pattern, or simply an oversight. Generating a public method with such a type in its
    /// signature would fail to compile for every consumer (CS0053/CS0051-class errors), so those
    /// members are skipped rather than attempted.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><see langword="true"/> when the type (and its constituent types) are all public.</returns>
    public static bool IsEffectivelyPublic(ITypeSymbol type)
    {
        switch (type)
        {
            case IArrayTypeSymbol array:
                return IsEffectivelyPublic(array.ElementType);

            case IPointerTypeSymbol pointer:
                return IsEffectivelyPublic(pointer.PointedAtType);

            case ITypeParameterSymbol:
                // A generic parameter of the containing type/method is always in scope.
                return true;

            case INamedTypeSymbol named:
                for (var current = named; current != null; current = current.ContainingType)
                {
                    if (current.DeclaredAccessibility != Accessibility.Public)
                    {
                        return false;
                    }
                }

                if (named.IsGenericType)
                {
                    foreach (var typeArgument in named.TypeArguments)
                    {
                        if (!IsEffectivelyPublic(typeArgument))
                        {
                            return false;
                        }
                    }
                }

                return true;

            default:
                // Primitives, dynamic, etc. — always accessible.
                return true;
        }
    }
}
