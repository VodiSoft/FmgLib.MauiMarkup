namespace FmgLib.MauiMarkup.Generator.Extensions;

public partial class ExtensionGenerator
{
    void GenerateExtensionMethod_AnimateTo(PropInfo info, string transformationName)
    {

        if (mainSymbol.IsSealed)
            builder.Append($@"
    public static Task<bool> Animate{info.methodName}To(this {info.MainSymbolName} self, {info.propertyTypeName} value, uint length = 250, Easing? easing = null)");
        else
            builder.Append($@"
    public static Task<bool> Animate{info.methodName}To<T>(this T self, {info.propertyTypeName} value, uint length = 250, Easing? easing = null)
        where T : {info.MainSymbolName}");

        // info.accessedWith is "self" for instance properties and the declaring type's name for
        // static ones (kept in sync with PropInfo.valueAssignmentString — see its remarks).
        var callBackSetValue = info.IsBindableProperty ? $"self.SetValue({info.BindablePropertyName}, actValue);" : $"{info.accessedWith}.{info.propertyName} = actValue;";

        builder.Append($@"
    {{
        {info.propertyTypeName} fromValue = {info.accessedWith}.{info.propertyName};
        var transform = (double t) => Transformations.{transformationName}(fromValue, value, t);
        var callback = ({info.propertyTypeName} actValue) => {{ {callBackSetValue} }};
        return Transformations.AnimateAsync<{info.propertyTypeName}>(self, ""Animate{info.methodName}To"", transform, callback, length, easing);
    }}
    ");
    }
}