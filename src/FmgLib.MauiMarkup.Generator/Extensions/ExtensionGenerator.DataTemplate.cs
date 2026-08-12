namespace FmgLib.MauiMarkup.Generator.Extensions;

public partial class ExtensionGenerator
{
    void GenerateExtensionMethod_DataTemplate(PropInfo info)
    {
        if (mainSymbol.IsSealed)
            GenerateExtensionMethod_DataTemplate_Sealed(info);
        else
            GenerateExtensionMethod_DataTemplate_Normal(info);
    }

    void GenerateExtensionMethod_DataTemplate_Sealed(PropInfo info)
    {
        builder.Append($@"
    public static {info.MainSymbolName} {info.methodName}<T>(this {info.MainSymbolName} self, global::System.Func<object> loadTemplate)
    {{
        {info.dataTemplateAssignmentString}
        return self;
    }}
    ");
    }

    void GenerateExtensionMethod_DataTemplate_Normal(PropInfo info)
    {
        builder.Append($@"
    public static T {info.methodName}<T>(this T self, global::System.Func<object> loadTemplate)
        where T : {info.MainSymbolName}
    {{
        {info.dataTemplateAssignmentString}
        return self;
    }}
    ");
    }
}