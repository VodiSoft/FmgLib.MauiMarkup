namespace FmgLib.MauiMarkup.Generator.Extensions;

public partial class ExtensionGenerator
{
    void GenerateExtensionMethod_SettersBuilder(PropInfo info)
    {
        if (mainSymbol.IsSealed)
            GenerateExtensionMethod_SettersBuilder_Sealed(info);
        else
            GenerateExtensionMethod_SettersBuilder_Normal(info);
    }

    void GenerateExtensionMethod_SettersBuilder_Sealed(PropInfo info)
    {
        builder.Append($@"
    public static global::FmgLib.MauiMarkup.SettersContext<{info.MainSymbolName}> {info.methodName}(this global::FmgLib.MauiMarkup.SettersContext<{info.MainSymbolName}> self, global::System.Func<global::FmgLib.MauiMarkup.PropertySettersContext<{info.propertyTypeName}>, global::FmgLib.MauiMarkup.IPropertySettersBuilder<{info.propertyTypeName}>> configure)
    {{
        var context = new global::FmgLib.MauiMarkup.PropertySettersContext<{info.propertyTypeName}>(self.XamlSetters, {info.BindablePropertyName});
        configure(context).Build();
        return self;
    }}
    ");
    }

    void GenerateExtensionMethod_SettersBuilder_Normal(PropInfo info)
    {
        builder.Append($@"
    public static global::FmgLib.MauiMarkup.SettersContext<T> {info.methodName}<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self, global::System.Func<global::FmgLib.MauiMarkup.PropertySettersContext<{info.propertyTypeName}>, global::FmgLib.MauiMarkup.IPropertySettersBuilder<{info.propertyTypeName}>> configure)
        where T : {info.MainSymbolName}
    {{
        var context = new global::FmgLib.MauiMarkup.PropertySettersContext<{info.propertyTypeName}>(self.XamlSetters, {info.BindablePropertyName});
        configure(context).Build();
        return self;
    }}
    ");
    }


    void GenerateExtensionMethod_SettersBuilder(AttachedFieldInfo info)
    {
        if (mainSymbol.IsSealed)
            GenerateExtensionMethod_SettersBuilder_Sealed(info);
        else
            GenerateExtensionMethod_SettersBuilder_Normal(info);
    }

    void GenerateExtensionMethod_SettersBuilder_Sealed(AttachedFieldInfo info)
    {
        builder.Append($@"
    public static global::FmgLib.MauiMarkup.SettersContext<{info.DeclaringTypeName}> {info.propertyName}(this global::FmgLib.MauiMarkup.SettersContext<{info.DeclaringTypeName}> self, global::System.Func<global::FmgLib.MauiMarkup.PropertySettersContext<{info.ReturnTypeName}>, global::FmgLib.MauiMarkup.IPropertySettersBuilder<{info.ReturnTypeName}>> configure)
    {{
        var context = new global::FmgLib.MauiMarkup.PropertySettersContext<{info.ReturnTypeName}>(self.XamlSetters, {info.BindablePropertyName});
        configure(context).Build();
        return self;
    }}
    ");
    }

    void GenerateExtensionMethod_SettersBuilder_Normal(AttachedFieldInfo info)
    {
        builder.Append($@"
    public static global::FmgLib.MauiMarkup.SettersContext<T> {info.propertyName}<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self, global::System.Func<global::FmgLib.MauiMarkup.PropertySettersContext<{info.ReturnTypeName}>, global::FmgLib.MauiMarkup.IPropertySettersBuilder<{info.ReturnTypeName}>> configure)
        where T : {info.DeclaringTypeName}
    {{
        var context = new global::FmgLib.MauiMarkup.PropertySettersContext<{info.ReturnTypeName}>(self.XamlSetters, {info.BindablePropertyName});
        configure(context).Build();
        return self;
    }}
    ");
    }
}
