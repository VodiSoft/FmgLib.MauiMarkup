using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup;

public static partial class VisualElementExtension
{
    public static T SizeRequest<T>(this T self, double widthAndHeightRequest) where T : VisualElement
    {
        self.SetValue(VisualElement.WidthRequestProperty, widthAndHeightRequest);
        self.SetValue(VisualElement.HeightRequestProperty, widthAndHeightRequest);
        return self;
    }

    public static T SizeRequest<T>(this T self, double widthRequest, double heightRequest) where T : VisualElement
    {
        self.SetValue(VisualElement.WidthRequestProperty, widthRequest);
        self.SetValue(VisualElement.HeightRequestProperty, heightRequest);
        return self;
    }

    public static SettersContext<T> SizeRequest<T>(this SettersContext<T> self, double widthAndHeightRequest) where T : VisualElement
    {
        self.XamlSetters.Add(new Setter
        {
            Property = VisualElement.WidthRequestProperty,
            Value = widthAndHeightRequest
        });
        self.XamlSetters.Add(new Setter
        {
            Property = VisualElement.HeightRequestProperty,
            Value = widthAndHeightRequest
        });
        return self;
    }

    public static SettersContext<T> SizeRequest<T>(this SettersContext<T> self, double widthRequest, double heightRequest) where T : VisualElement
    {
        self.XamlSetters.Add(new Setter
        {
            Property = VisualElement.WidthRequestProperty,
            Value = widthRequest
        });
        self.XamlSetters.Add(new Setter
        {
            Property = VisualElement.HeightRequestProperty,
            Value = heightRequest
        });
        return self;
    }

    public static Task<bool> AnimateSizeRequestTo<T>(this T self, double width, double height, uint length = 250u, Easing easing = null) where T : VisualElement
    {
        Size from = new Size(self.WidthRequest, self.HeightRequest);
        Size to = new Size(width, height);
        Func<double, Size> transform = (double t) => Transformations.SizeTransform(from, to, t);
        Action<Size> callback = delegate (Size value)
        {
            self.SizeRequest(value.Width, value.Height);
        };
        return Transformations.AnimateAsync(self, "AnimateSizeRequestTo", transform, callback, length, easing);
    }

    public static T Behaviors<T>(this T self, params Behavior[] behaviors) where T : VisualElement
    {
        foreach (Behavior item in behaviors)
        {
            self.Behaviors.Add(item);
        }

        return self;
    }

    public static T Triggers<T>(this T self, params TriggerBase[] triggers) where T : VisualElement
    {
        foreach (TriggerBase item in triggers)
        {
            self.Triggers.Add(item);
        }

        return self;
    }

    public static T OnBatchCommitted<T>(this T self, EventHandler<Microsoft.Maui.Controls.Internals.EventArg<VisualElement>> handler) where T : VisualElement
    {
        self.BatchCommitted += handler;
        return self;
    }

    public static T OnBatchCommitted<T>(this T self, Action<T> action) where T : VisualElement
    {
        Action<T> action2 = action;
        T self2 = self;
        self2.BatchCommitted += delegate
        {
            action2(self2);
        };
        return self2;
    }

    public static T OnFocusChangeRequested<T>(this T self, EventHandler<VisualElement.FocusRequestArgs> handler) where T : VisualElement
    {
        self.FocusChangeRequested += handler;
        return self;
    }

    public static T OnFocusChangeRequested<T>(this T self, Action<T> action) where T : VisualElement
    {
        Action<T> action2 = action;
        T self2 = self;
        self2.FocusChangeRequested += delegate
        {
            action2(self2);
        };
        return self2;
    }

    public static T VisualStateGroups<T>(this T self, VisualStateGroupList visualStateGroups) where T : VisualElement
    {
        self.SetValue(VisualStateManager.VisualStateGroupsProperty, visualStateGroups);
        return self;
    }

    public static T VisualStateGroups<T>(this T self, Func<PropertyContext<VisualStateGroupList>, IPropertyBuilder<VisualStateGroupList>> configure) where T : VisualElement
    {
        PropertyContext<VisualStateGroupList> arg = new PropertyContext<VisualStateGroupList>(self, VisualStateManager.VisualStateGroupsProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> VisualStateGroups<T>(this SettersContext<T> self, VisualStateGroupList visualStateGroups) where T : VisualElement
    {
        self.XamlSetters.Add(new Setter
        {
            Property = VisualStateManager.VisualStateGroupsProperty,
            Value = visualStateGroups
        });
        return self;
    }

    public static SettersContext<T> VisualStateGroups<T>(this SettersContext<T> self, Func<PropertySettersContext<VisualStateGroupList>, IPropertySettersBuilder<VisualStateGroupList>> configure) where T : VisualElement
    {
        PropertySettersContext<VisualStateGroupList> arg = new PropertySettersContext<VisualStateGroupList>(self.XamlSetters, VisualStateManager.VisualStateGroupsProperty);
        configure(arg).Build();
        return self;
    }

    public static VisualStateGroupList GetVisualStateGroupsValue<T>(this T self) where T : VisualElement
    {
        return (VisualStateGroupList)self.GetValue(VisualStateManager.VisualStateGroupsProperty);
    }

    public static T AutomationExcludedWithChildren<T>(this T self, bool? automationExcludedWithChildren) where T : VisualElement
    {
        self.SetValue(AutomationProperties.ExcludedWithChildrenProperty, automationExcludedWithChildren);
        return self;
    }

    public static T AutomationExcludedWithChildren<T>(this T self, Func<PropertyContext<bool?>, IPropertyBuilder<bool?>> configure) where T : VisualElement
    {
        PropertyContext<bool?> arg = new PropertyContext<bool?>(self, AutomationProperties.ExcludedWithChildrenProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> AutomationExcludedWithChildren<T>(this SettersContext<T> self, bool? automationExcludedWithChildren) where T : VisualElement
    {
        self.XamlSetters.Add(new Setter
        {
            Property = AutomationProperties.ExcludedWithChildrenProperty,
            Value = automationExcludedWithChildren
        });
        return self;
    }

    public static SettersContext<T> AutomationExcludedWithChildren<T>(this SettersContext<T> self, Func<PropertySettersContext<bool?>, IPropertySettersBuilder<bool?>> configure) where T : VisualElement
    {
        PropertySettersContext<bool?> arg = new PropertySettersContext<bool?>(self.XamlSetters, AutomationProperties.ExcludedWithChildrenProperty);
        configure(arg).Build();
        return self;
    }

    public static bool? GetAutomationExcludedWithChildrenValue<T>(this T self) where T : VisualElement
    {
        return (bool?)self.GetValue(AutomationProperties.ExcludedWithChildrenProperty);
    }

    public static T AutomationIsInAccessibleTree<T>(this T self, bool? automationIsInAccessibleTree) where T : VisualElement
    {
        self.SetValue(AutomationProperties.IsInAccessibleTreeProperty, automationIsInAccessibleTree);
        return self;
    }

    public static T AutomationIsInAccessibleTree<T>(this T self, Func<PropertyContext<bool?>, IPropertyBuilder<bool?>> configure) where T : VisualElement
    {
        PropertyContext<bool?> arg = new PropertyContext<bool?>(self, AutomationProperties.IsInAccessibleTreeProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> AutomationIsInAccessibleTree<T>(this SettersContext<T> self, bool? automationIsInAccessibleTree) where T : VisualElement
    {
        self.XamlSetters.Add(new Setter
        {
            Property = AutomationProperties.IsInAccessibleTreeProperty,
            Value = automationIsInAccessibleTree
        });
        return self;
    }

    public static SettersContext<T> AutomationIsInAccessibleTree<T>(this SettersContext<T> self, Func<PropertySettersContext<bool?>, IPropertySettersBuilder<bool?>> configure) where T : VisualElement
    {
        PropertySettersContext<bool?> arg = new PropertySettersContext<bool?>(self.XamlSetters, AutomationProperties.IsInAccessibleTreeProperty);
        configure(arg).Build();
        return self;
    }

    public static bool? GetAutomationIsInAccessibleTreeValue<T>(this T self) where T : VisualElement
    {
        return (bool?)self.GetValue(AutomationProperties.IsInAccessibleTreeProperty);
    }

    public static T AutomationName<T>(this T self, string automationName) where T : VisualElement
    {
        self.SetValue(AutomationProperties.NameProperty, automationName);
        return self;
    }

    public static T AutomationName<T>(this T self, Func<PropertyContext<string>, IPropertyBuilder<string>> configure) where T : VisualElement
    {
        PropertyContext<string> arg = new PropertyContext<string>(self, AutomationProperties.NameProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> AutomationName<T>(this SettersContext<T> self, string automationName) where T : VisualElement
    {
        self.XamlSetters.Add(new Setter
        {
            Property = AutomationProperties.NameProperty,
            Value = automationName
        });
        return self;
    }

    public static SettersContext<T> AutomationName<T>(this SettersContext<T> self, Func<PropertySettersContext<string>, IPropertySettersBuilder<string>> configure) where T : VisualElement
    {
        PropertySettersContext<string> arg = new PropertySettersContext<string>(self.XamlSetters, AutomationProperties.NameProperty);
        configure(arg).Build();
        return self;
    }

    public static string GetAutomationNameValue<T>(this T self) where T : VisualElement
    {
        return (string)self.GetValue(AutomationProperties.NameProperty);
    }

    public static T AutomationHelpText<T>(this T self, string automationHelpText) where T : VisualElement
    {
        self.SetValue(AutomationProperties.HelpTextProperty, automationHelpText);
        return self;
    }

    public static T AutomationHelpText<T>(this T self, Func<PropertyContext<string>, IPropertyBuilder<string>> configure) where T : VisualElement
    {
        PropertyContext<string> arg = new PropertyContext<string>(self, AutomationProperties.HelpTextProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> AutomationHelpText<T>(this SettersContext<T> self, string automationHelpText) where T : VisualElement
    {
        self.XamlSetters.Add(new Setter
        {
            Property = AutomationProperties.HelpTextProperty,
            Value = automationHelpText
        });
        return self;
    }

    public static SettersContext<T> AutomationHelpText<T>(this SettersContext<T> self, Func<PropertySettersContext<string>, IPropertySettersBuilder<string>> configure) where T : VisualElement
    {
        PropertySettersContext<string> arg = new PropertySettersContext<string>(self.XamlSetters, AutomationProperties.HelpTextProperty);
        configure(arg).Build();
        return self;
    }

    public static string GetAutomationHelpTextValue<T>(this T self) where T : VisualElement
    {
        return (string)self.GetValue(AutomationProperties.HelpTextProperty);
    }

    public static T AutomationLabeledBy<T>(this T self, VisualElement automationLabeledBy) where T : VisualElement
    {
        self.SetValue(AutomationProperties.LabeledByProperty, automationLabeledBy);
        return self;
    }

    public static T AutomationLabeledBy<T>(this T self, Func<PropertyContext<VisualElement>, IPropertyBuilder<VisualElement>> configure) where T : VisualElement
    {
        PropertyContext<VisualElement> arg = new PropertyContext<VisualElement>(self, AutomationProperties.LabeledByProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> AutomationLabeledBy<T>(this SettersContext<T> self, VisualElement automationLabeledBy) where T : VisualElement
    {
        self.XamlSetters.Add(new Setter
        {
            Property = AutomationProperties.LabeledByProperty,
            Value = automationLabeledBy
        });
        return self;
    }

    public static SettersContext<T> AutomationLabeledBy<T>(this SettersContext<T> self, Func<PropertySettersContext<VisualElement>, IPropertySettersBuilder<VisualElement>> configure) where T : VisualElement
    {
        PropertySettersContext<VisualElement> arg = new PropertySettersContext<VisualElement>(self.XamlSetters, AutomationProperties.LabeledByProperty);
        configure(arg).Build();
        return self;
    }

    public static VisualElement GetAutomationLabeledByValue<T>(this T self) where T : VisualElement
    {
        return (VisualElement)self.GetValue(AutomationProperties.LabeledByProperty);
    }

    public static T SemanticHint<T>(this T self, string semanticHint) where T : VisualElement
    {
        self.SetValue(SemanticProperties.HintProperty, semanticHint);
        return self;
    }

    public static T SemanticHint<T>(this T self, Func<PropertyContext<string>, IPropertyBuilder<string>> configure) where T : VisualElement
    {
        PropertyContext<string> arg = new PropertyContext<string>(self, SemanticProperties.HintProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> SemanticHint<T>(this SettersContext<T> self, string semanticHint) where T : VisualElement
    {
        self.XamlSetters.Add(new Setter
        {
            Property = SemanticProperties.HintProperty,
            Value = semanticHint
        });
        return self;
    }

    public static SettersContext<T> SemanticHint<T>(this SettersContext<T> self, Func<PropertySettersContext<string>, IPropertySettersBuilder<string>> configure) where T : VisualElement
    {
        PropertySettersContext<string> arg = new PropertySettersContext<string>(self.XamlSetters, SemanticProperties.HintProperty);
        configure(arg).Build();
        return self;
    }

    public static string GetSemanticHintValue<T>(this T self) where T : VisualElement
    {
        return (string)self.GetValue(SemanticProperties.HintProperty);
    }

    public static T SemanticDescription<T>(this T self, string semanticDescription) where T : VisualElement
    {
        self.SetValue(SemanticProperties.DescriptionProperty, semanticDescription);
        return self;
    }

    public static T SemanticDescription<T>(this T self, Func<PropertyContext<string>, IPropertyBuilder<string>> configure) where T : VisualElement
    {
        PropertyContext<string> arg = new PropertyContext<string>(self, SemanticProperties.DescriptionProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> SemanticDescription<T>(this SettersContext<T> self, string semanticDescription) where T : VisualElement
    {
        self.XamlSetters.Add(new Setter
        {
            Property = SemanticProperties.DescriptionProperty,
            Value = semanticDescription
        });
        return self;
    }

    public static SettersContext<T> SemanticDescription<T>(this SettersContext<T> self, Func<PropertySettersContext<string>, IPropertySettersBuilder<string>> configure) where T : VisualElement
    {
        PropertySettersContext<string> arg = new PropertySettersContext<string>(self.XamlSetters, SemanticProperties.DescriptionProperty);
        configure(arg).Build();
        return self;
    }

    public static string GetSemanticDescriptionValue<T>(this T self) where T : VisualElement
    {
        return (string)self.GetValue(SemanticProperties.DescriptionProperty);
    }

    public static T SemanticHeadingLevel<T>(this T self, SemanticHeadingLevel semanticHeadingLevel) where T : VisualElement
    {
        self.SetValue(SemanticProperties.HeadingLevelProperty, semanticHeadingLevel);
        return self;
    }

    public static T SemanticHeadingLevel<T>(this T self, Func<PropertyContext<SemanticHeadingLevel>, IPropertyBuilder<SemanticHeadingLevel>> configure) where T : VisualElement
    {
        PropertyContext<SemanticHeadingLevel> arg = new PropertyContext<SemanticHeadingLevel>(self, SemanticProperties.HeadingLevelProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> SemanticHeadingLevel<T>(this SettersContext<T> self, SemanticHeadingLevel semanticHeadingLevel) where T : VisualElement
    {
        self.XamlSetters.Add(new Setter
        {
            Property = SemanticProperties.HeadingLevelProperty,
            Value = semanticHeadingLevel
        });
        return self;
    }

    public static SettersContext<T> SemanticHeadingLevel<T>(this SettersContext<T> self, Func<PropertySettersContext<SemanticHeadingLevel>, IPropertySettersBuilder<SemanticHeadingLevel>> configure) where T : VisualElement
    {
        PropertySettersContext<SemanticHeadingLevel> arg = new PropertySettersContext<SemanticHeadingLevel>(self.XamlSetters, SemanticProperties.HeadingLevelProperty);
        configure(arg).Build();
        return self;
    }

    public static SemanticHeadingLevel GetSemanticHeadingLevelValue<T>(this T self) where T : VisualElement
    {
        return (SemanticHeadingLevel)self.GetValue(SemanticProperties.HeadingLevelProperty);
    }

}
