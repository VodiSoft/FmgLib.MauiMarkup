namespace FmgLib.MauiMarkup.Generator.Extensions;

public partial class ExtensionGenerator
{
    void GenerateExtensionMethod_BindablePropertyBuilder(PropInfo info)
    {
        if (mainSymbol.IsSealed)
            GenerateExtensionMethod_BindablePropertyBuilder_Sealed(info);
        else
            GenerateExtensionMethod_BindablePropertyBuilder_Normal(info);
    }

    void GenerateExtensionMethod_BindablePropertyBuilder_Sealed(PropInfo info)
    {
        builder.Append($@"
    public static {info.MainSymbolName} {info.methodName}(this {info.MainSymbolName} self, global::System.Func<global::FmgLib.MauiMarkup.PropertyContext<{info.propertyTypeName}>, global::FmgLib.MauiMarkup.IPropertyBuilder<{info.propertyTypeName}>> configure)
    {{
        var context = new global::FmgLib.MauiMarkup.PropertyContext<{info.propertyTypeName}>(self, {info.BindablePropertyName});
        configure(context).Build();
        return self;
    }}
    ");
    }

    void GenerateExtensionMethod_BindablePropertyBuilder_Normal(PropInfo info)
    {
        builder.Append($@"
    public static T {info.methodName}<T>(this T self, global::System.Func<global::FmgLib.MauiMarkup.PropertyContext<{info.propertyTypeName}>, global::FmgLib.MauiMarkup.IPropertyBuilder<{info.propertyTypeName}>> configure)
        where T : {info.MainSymbolName}
    {{
        var context = new global::FmgLib.MauiMarkup.PropertyContext<{info.propertyTypeName}>(self, {info.BindablePropertyName});
        configure(context).Build();
        return self;
    }}
    ");
    }


    void GenerateExtensionMethod_BindablePropertyBuilder(AttachedFieldInfo info)
    {
        if (mainSymbol.IsSealed)
            GenerateExtensionMethod_BindablePropertyBuilder_Sealed(info);
        else
            GenerateExtensionMethod_BindablePropertyBuilder_Normal(info);
    }

    void GenerateExtensionMethod_BindablePropertyBuilder_Sealed(AttachedFieldInfo info)
    {
        builder.Append($@"
    public static {info.DeclaringTypeName} {info.propertyName}(this {info.DeclaringTypeName} self, global::System.Func<global::FmgLib.MauiMarkup.PropertyContext<{info.ReturnTypeName}>, global::FmgLib.MauiMarkup.IPropertyBuilder<{info.ReturnTypeName}>> configure)
    {{
        var context = new global::FmgLib.MauiMarkup.PropertyContext<{info.ReturnTypeName}>(self, {info.BindablePropertyName});
        configure(context).Build();
        return self;
    }}
    ");
    }

    void GenerateExtensionMethod_BindablePropertyBuilder_Normal(AttachedFieldInfo info)
    {
        builder.Append($@"
    public static T {info.propertyName}<T>(this T self, global::System.Func<global::FmgLib.MauiMarkup.PropertyContext<{info.ReturnTypeName}>, global::FmgLib.MauiMarkup.IPropertyBuilder<{info.ReturnTypeName}>> configure)
        where T : {info.DeclaringTypeName}
    {{
        var context = new global::FmgLib.MauiMarkup.PropertyContext<{info.ReturnTypeName}>(self, {info.BindablePropertyName});
        configure(context).Build();
        return self;
    }}
    ");
    }
}
