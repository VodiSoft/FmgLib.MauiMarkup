using Microsoft.CodeAnalysis;

namespace FmgLib.MauiMarkup.Generator.Extensions;

public partial class ExtensionGenerator
{
    void GenerateExtensionMethods_ITextAlignment(ISymbol symbol)
    {
        builder.Append($@"

    public static T AlignText<T>(this T self, global::Microsoft.Maui.TextAlignment vertical, global::Microsoft.Maui.TextAlignment horizontal)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, vertical);
        self.SetValue({symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, horizontal);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> AlignText<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self, global::Microsoft.Maui.TextAlignment vertical, global::Microsoft.Maui.TextAlignment horizontal)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, Value = vertical }});
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, Value = horizontal }});
        return self;
    }}

    public static T TextCenterHorizontal<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Center);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextCenterHorizontal<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Center }});
        return self;
    }}

    public static T TextCenterVertical<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Center);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextCenterVertical<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Center }});
        return self;
    }}

    public static T TextCenter<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Center);
        self.SetValue({symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Center);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextCenter<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Center }});
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Center }});
        return self;
    }}

    public static T TextTop<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Start);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextTop<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Start }});
        return self;
    }}

    public static T TextBottom<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.End);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextBottom<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.End }});
        return self;
    }}

    public static T TextTopLeft<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Start);
        self.SetValue({symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Start);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextTopLeft<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Start }});
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Start }});
        return self;
    }}

    public static T TextBottomLeft<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.End);
        self.SetValue({symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Start);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextBottomLeft<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.End }});
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Start }});
        return self;
    }}

    public static T TextTopCenter<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Start);
        self.SetValue({symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Center);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextTopCenter<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Start }});
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Center }});
        return self;
    }}

    public static T TextBottomCenter<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.End);
        self.SetValue({symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Center);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextBottomCenter<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.End }});
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Center }});
        return self;
    }}

    public static T TextCenterRight<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Center);
        self.SetValue({symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.End);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextCenterRight<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Center }});
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.End }});
        return self;
    }}

    public static T TextCenterLeft<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Center);
        self.SetValue({symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Start);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextCenterLeft<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Center }});
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Start }});
        return self;
    }}

    public static T TextTopRight<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Start);
        self.SetValue({symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.End);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextTopRight<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Start }});
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.End }});
        return self;
    }}

    public static T TextBottomRight<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.End);
        self.SetValue({symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.End);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextBottomRight<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.VerticalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.End }});
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.End }});
        return self;
    }}

    public static T TextLeft<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.Start);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextLeft<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.Start }});
        return self;
    }}

    public static T TextRight<T>(this T self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.SetValue({symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, global::Microsoft.Maui.TextAlignment.End);
        return self;
    }}

    public static global::FmgLib.MauiMarkup.SettersContext<T> TextRight<T>(this global::FmgLib.MauiMarkup.SettersContext<T> self)
        where T : {symbol.ToQualifiedName()}
    {{
        self.XamlSetters.Add(new global::Microsoft.Maui.Controls.Setter {{ Property = {symbol.ToQualifiedName()}.HorizontalTextAlignmentProperty, Value = global::Microsoft.Maui.TextAlignment.End }});
        return self;
    }}

    ");
    }
}

