namespace FmgLib.MauiMarkup.Generator.Extensions;

public partial class ExtensionGenerator
{
    void GenerateExtensionMethod_Setters(PropInfo info)
    {
        if (mainSymbol.IsSealed)
            GenerateExtensionMethod_Setters_Sealed(info);
        else
            GenerateExtensionMethod_Setters_Normal(info);
    }

    void GenerateExtensionMethod_Setters_Sealed(PropInfo info)
    {
        builder.Append($@"
    public static global::FmgLib.MauiMarkup.SettersContext<{info.MainSymbolName}> {info.methodName}(this global::FmgLib.MauiMarkup.SettersContext<{info.MainSymbolName}> self,
        {info.propertyTypeName} {info.camelCaseName})
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {info.BindablePropertyName}, Value = {info.camelCaseName} }});
        return self;
    }}
    ");
    }

    void GenerateExtensionMethod_Setters_Normal(PropInfo info)
    {
        builder.Append($@"
    public static global::FmgLib.MauiMarkup.SettersContext<T> {info.methodName}<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self,
        {info.propertyTypeName} {info.camelCaseName})
        where T : {info.MainSymbolName}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {info.BindablePropertyName}, Value = {info.camelCaseName} }});
        return self;
    }}
    ");
    }


    void GenerateExtensionMethod_Setters(AttachedFieldInfo info)
    {
        if (mainSymbol.IsSealed)
            GenerateExtensionMethod_Setters_Sealed(info);
        else
            GenerateExtensionMethod_Setters_Normal(info);
    }

    void GenerateExtensionMethod_Setters_Sealed(AttachedFieldInfo info)
    {
        builder.Append($@"
    public static global::FmgLib.MauiMarkup.SettersContext<{info.DeclaringTypeName}> {info.propertyName}(this global::FmgLib.MauiMarkup.SettersContext<{info.DeclaringTypeName}> self,
        {info.ReturnTypeName} {info.camelCaseName})
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {info.BindablePropertyName}, Value = {info.camelCaseName} }});
        return self;
    }}
    ");
    }

    void GenerateExtensionMethod_Setters_Normal(AttachedFieldInfo info)
    {
        builder.Append($@"
    public static global::FmgLib.MauiMarkup.SettersContext<T> {info.propertyName}<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self,
        {info.ReturnTypeName} {info.camelCaseName})
        where T : {info.DeclaringTypeName}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {info.BindablePropertyName}, Value = {info.camelCaseName} }});
        return self;
    }}
    ");
    }
}