namespace FmgLib.MauiMarkup.Generator.Extensions;

public partial class ExtensionGenerator
{
    void GenerateExtensionMethod_List(PropInfo info, string elementTypeName)
    {
        if (mainSymbol.IsSealed)
            GenerateExtensionMethod_List_Sealed(info, elementTypeName);
        else
            GenerateExtensionMethod_List_Normal(info, elementTypeName);
    }

    void GenerateExtensionMethod_List_Sealed(PropInfo info, string elementTypeName)
    {
        var tail = info.propertyTypeName.EndsWith("?") ? "?" : "";
        builder.Append($@"
    public static {info.MainSymbolName} {info.methodName}(this {info.MainSymbolName} self,
        global::System.Collections.Generic.IList<{elementTypeName}> {info.camelCaseName})
    {{
        foreach (var item in {info.camelCaseName})
            {info.accessedWith}.{info.propertyName}{tail}.Add(item);
        return self;
    }}

    public static {info.MainSymbolName} {info.methodName}(this {info.MainSymbolName} self,
        params {elementTypeName}[] {info.camelCaseName})
    {{
        foreach (var item in {info.camelCaseName})
            {info.accessedWith}.{info.propertyName}{tail}.Add(item);
        return self;
    }}
    ");
    }

    void GenerateExtensionMethod_List_Normal(PropInfo info, string elementTypeName)
    {
        var tail = info.propertyTypeName.EndsWith("?") ? "?" : "";
        builder.Append($@"
    public static T {info.propertyName}<T>(this T self,
        global::System.Collections.Generic.IList<{elementTypeName}> {info.camelCaseName})
        where T : {info.MainSymbolName}
    {{
        foreach (var item in {info.camelCaseName})
            {info.accessedWith}.{info.propertyName}{tail}.Add(item);
        return self;
    }}

    public static T {info.propertyName}<T>(this T self,
        params {elementTypeName}[] {info.camelCaseName})
        where T : {info.MainSymbolName}
    {{
        foreach (var item in {info.camelCaseName})
            {info.accessedWith}.{info.propertyName}{tail}.Add(item);
        return self;
    }}
    ");
    }

}