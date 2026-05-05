using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace FmgLib.MauiMarkup.Generator.Extensions;

/// <summary>
/// Stores attached property metadata required during code generation.
/// </summary>
public sealed class AttachedFieldInfo
{
    public INamedTypeSymbol MainSymbol { get; set; } = null!;
    public IFieldSymbol FieldSymbol { get; set; } = null!;
    public string BindablePropertyName { get; set; } = null!;
    public string PropertyName { get; set; } = null!;
    public string ReturnTypeName { get; set; } = null!;
    public string DeclaringTypeName { get; set; } = null!;

    public string propertyName = null!;
    public string propertyTypeName = null!;
    public string camelCaseName = null!;
    public string symbolTypeName = null!;
    public string valueAssignmentString = null!;
    public string dataTemplateAssignmentString = null!;

    /// <summary>
    /// Builds the configuration for the <c>Build</c> operation.
    /// </summary>
    public void Build()
    {
        FieldSymbol = MainSymbol.GetMembers(PropertyName).OfType<IFieldSymbol>().FirstOrDefault();
        symbolTypeName = $"{MainSymbol.ToDisplayString()}";
        propertyName = BindablePropertyName.Split(new[] { "." }, StringSplitOptions.None)[^2] + PropertyName.Replace("Property", "");
        propertyTypeName = ReturnTypeName;
        camelCaseName = Helpers.ToCamelCase(propertyName);
        valueAssignmentString = $@"self.SetValue({BindablePropertyName}, {camelCaseName});";
        dataTemplateAssignmentString = $@"self.SetValue({BindablePropertyName}, new DataTemplate(loadTemplate));";
    }
}
