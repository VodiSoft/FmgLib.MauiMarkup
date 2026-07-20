namespace FmgLib.MauiMarkup.Generator.Extensions;

public partial class ExtensionGenerator
{
    void GenerateExtensionMethod_ContentProp_Single(PropInfo info)
    {
        if (mainSymbol.IsSealed)
            GenerateExtensionMethod_ContentProp_Single_Sealed(info);
        else
            GenerateExtensionMethod_ContentProp_Single_Normal(info);
    }

    void GenerateExtensionMethod_ContentProp_List(PropInfo info, string elementTypeName, string fluentMethodName = "")
    {
        if (mainSymbol.IsSealed)
            GenerateExtensionMethod_ContentProp_List_Sealed(info, elementTypeName, fluentMethodName);
        else
            GenerateExtensionMethod_ContentProp_List_Normal(info, elementTypeName, fluentMethodName);
    }



    void GenerateExtensionMethod_ContentProp_Single_Sealed(PropInfo info)
    {
        builder.Append($@"
    public static {info.MainSymbolName} {info.methodName}_ContentProp(this {info.MainSymbolName} self,
        {info.propertyTypeName} {info.camelCaseName})
    {{
        self.{info.propertyName} = {info.camelCaseName};
        return self;
    }}

    public static {info.MainSymbolName} {info.methodName}_ContentProp(this {info.MainSymbolName} self,
        Func<{info.propertyTypeName}> configure)
    {{
        var {info.camelCaseName} = configure();
        self.{info.propertyName} = {info.camelCaseName};
        return self;
    }}
    ");
    }

    void GenerateExtensionMethod_ContentProp_Single_Normal(PropInfo info)
    {
        builder.Append($@"
    public static T {info.methodName}_ContentProp<T>(this T self,
        {info.propertyTypeName} {info.camelCaseName})
        where T : {info.MainSymbolName}
    {{
        self.{info.propertyName} = {info.camelCaseName};
        return self;
    }}

    public static T {info.methodName}_ContentProp<T>(this T self,
        Func<{info.propertyTypeName}> configure)
        where T : {info.MainSymbolName}
    {{
        var {info.camelCaseName} = configure();
        self.{info.propertyName} = {info.camelCaseName};
        return self;
    }}
    ");
    }



    void GenerateExtensionMethod_ContentProp_List_Sealed(PropInfo info, string elementTypeName, string fluentMethodName = "")
    {
        var tail = info.propertyTypeName.EndsWith("?") ? "?" : "";
        builder.Append($@"
    public static {info.MainSymbolName} {(string.IsNullOrEmpty(fluentMethodName) ? info.methodName : fluentMethodName)}(this {info.MainSymbolName} self,
        IList<{elementTypeName}> {info.camelCaseName})
    {{
        foreach (var item in {info.camelCaseName})
            {info.accessedWith}.{info.propertyName}{tail}.Add(item);
        return self;
    }}

    public static {info.MainSymbolName} {(string.IsNullOrEmpty(fluentMethodName) ? info.methodName : fluentMethodName)}(this {info.MainSymbolName} self,
        params {elementTypeName}[] {info.camelCaseName})
    {{
        foreach (var item in {info.camelCaseName})
            {info.accessedWith}.{info.propertyName}{tail}.Add(item);
        return self;
    }}

    public static {info.MainSymbolName} {(string.IsNullOrEmpty(fluentMethodName) ? info.methodName : fluentMethodName)}(this {info.MainSymbolName} self,
        Func<{elementTypeName}[]> configure)
    {{
        var {info.camelCaseName} = configure();
        foreach (var item in {info.camelCaseName})
            {info.accessedWith}.{info.propertyName}{tail}.Add(item);
        return self;
    }}
    ");
    }

    void GenerateExtensionMethod_ContentProp_List_Normal(PropInfo info, string elementTypeName, string fluentMethodName = "")
    {
        var tail = info.propertyTypeName.EndsWith("?") ? "?" : "";
        builder.Append($@"
    public static T {(string.IsNullOrEmpty(fluentMethodName) ? info.methodName : fluentMethodName)}<T>(this T self,
        IList<{elementTypeName}> {info.camelCaseName})
        where T : {info.MainSymbolName}
    {{
        foreach (var item in {info.camelCaseName})
            {info.accessedWith}.{info.propertyName}{tail}.Add(item);
        return self;
    }}

    public static T {(string.IsNullOrEmpty(fluentMethodName) ? info.methodName : fluentMethodName)}<T>(this T self,
        params {elementTypeName}[] {info.camelCaseName})
        where T : {info.MainSymbolName}
    {{
        foreach (var item in {info.camelCaseName})
            {info.accessedWith}.{info.propertyName}{tail}.Add(item);
        return self;
    }}

    public static T {(string.IsNullOrEmpty(fluentMethodName) ? info.methodName : fluentMethodName)}<T>(this T self,
        Func<{elementTypeName}[]> configure)
        where T : {info.MainSymbolName}
    {{
        var {info.camelCaseName} = configure();
        foreach (var item in {info.camelCaseName})
            {info.accessedWith}.{info.propertyName}{tail}.Add(item);
        return self;
    }}
    ");
    }
}
