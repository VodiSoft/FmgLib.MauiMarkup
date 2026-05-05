using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FmgLib.MauiMarkup;

/// <summary>
/// Describes property metadata needed to generate fluent extension methods.
/// </summary>
public class PropInfo
{
    public string MainSymbolName { get; set; } = null!;
    public IPropertySymbol PropertySymbol { get; set; } = null!;
    public bool IsBindableProperty { get; set; }
    public string BindablePropertyName { get; set; } = null!;

    public string propertyName = null!;
    public string methodName = null!;
    public string propertyTypeName = null!;
    public string accessedWith = null!;
    public string camelCaseName = null!;
    public string valueAssignmentString = null!;
    public string dataTemplateAssignmentString = null!;

    /// <summary>
    /// Builds the configuration for the <c>Build</c> operation.
    /// </summary>
    /// <param name="redefinedProperties">The value used for <paramref name="redefinedProperties"/>.</param>
    public void Build(List<string>? redefinedProperties = null)
    {
        propertyName = PropertySymbol.Name.Split(new[] { "." }, StringSplitOptions.None).Last();
        propertyName = propertyName.Equals("class", StringComparison.Ordinal) ? "@class" : propertyName;
        accessedWith = PropertySymbol.IsStatic ? MainSymbolName : "self";

        if (IsBindableProperty && BindablePropertyName == null)
        {
            BindablePropertyName = $"{MainSymbolName}.{propertyName}";
        }

        camelCaseName = Helpers.ToCamelCase(propertyName);
        propertyTypeName = PropertySymbol.Type.ToDisplayString();

        valueAssignmentString = IsBindableProperty ?
            $@"self.SetValue({BindablePropertyName}, {camelCaseName});" :
            $"{accessedWith}.{propertyName} = {camelCaseName};";

        dataTemplateAssignmentString = IsBindableProperty ?
            $@"self.SetValue({BindablePropertyName}, new DataTemplate(loadTemplate));" :
            $@"{accessedWith}.{propertyName} = new DataTemplate(loadTemplate);";

        methodName = redefinedProperties == null || redefinedProperties.Count == 0 || !redefinedProperties.Any(e => e == propertyName) ? propertyName : propertyName + "New";
    }
}
