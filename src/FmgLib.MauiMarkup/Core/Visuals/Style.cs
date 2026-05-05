using System.Collections;

namespace FmgLib.MauiMarkup;

public class Style<T> : IEnumerable where T : BindableObject
{
    private static readonly BindableProperty AttachedStyleInvokeProperty = BindableProperty.CreateAttached("Style.AttachedInvokeProperty", typeof(Action<T>), typeof(Style<T>), null, BindingMode.OneWay, null, OnAttachedInvokeChanged);

    private Style mauiStyle;

    /// <summary>
    /// Executes the <c>OnAttachedInvokeChanged</c> operation.
    /// </summary>
    /// <param name="obj">The value used for <paramref name="obj"/>.</param>
    /// <param name="oldValue">The value used for <paramref name="oldValue"/>.</param>
    /// <param name="newValue">The value used for <paramref name="newValue"/>.</param>
    private static void OnAttachedInvokeChanged(BindableObject obj, object oldValue, object newValue)
    {
        Action<T> action = newValue as Action<T>;
        if (obj is VisualElement visualElement && visualElement.Handler != null)
        {
            action?.Invoke(obj as T);
        }
    }

    /// <summary>
    /// Executes the <c>Style</c> operation.
    /// </summary>
    /// <param name="style">The value used for <paramref name="style"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public static implicit operator Style(Style<T> style)
    {
        return style.mauiStyle;
    }

    /// <summary>
    /// Initializes a new instance of the <c>Style</c> class.
    /// </summary>
    public Style()
    {
        mauiStyle = new Style(typeof(T));
    }

    /// <summary>
    /// Initializes a new instance of the <c>Style</c> class.
    /// </summary>
    /// <param name="basedOn">The value used for <paramref name="basedOn"/>.</param>
    public Style(Style basedOn)
        : this()
    {
        foreach (Setter setter in basedOn.Setters)
        {
            mauiStyle.Setters.Add(setter);
        }

        foreach (TriggerBase trigger in basedOn.Triggers)
        {
            mauiStyle.Triggers.Add(trigger);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <c>Style</c> class.
    /// </summary>
    /// <param name="applyToDerivedTypes">The value used for <paramref name="applyToDerivedTypes"/>.</param>
    public Style(bool applyToDerivedTypes)
        : this()
    {
        mauiStyle.ApplyToDerivedTypes = applyToDerivedTypes;
    }

    /// <summary>
    /// Initializes a new instance of the <c>Style</c> class.
    /// </summary>
    /// <param name="buildSetters">The value used for <paramref name="buildSetters"/>.</param>
    public Style(Func<SettersContext<T>, SettersContext<T>> buildSetters)
        : this()
    {
        BuildSetters(buildSetters);
    }

    /// <summary>
    /// Initializes a new instance of the <c>Style</c> class.
    /// </summary>
    /// <param name="applyToDerivedTypes">The value used for <paramref name="applyToDerivedTypes"/>.</param>
    /// <param name="buildSetters">The value used for <paramref name="buildSetters"/>.</param>
    public Style(bool applyToDerivedTypes, Func<SettersContext<T>, SettersContext<T>> buildSetters)
        : this()
    {
        mauiStyle.ApplyToDerivedTypes = applyToDerivedTypes;
        BuildSetters(buildSetters);
    }

    /// <summary>
    /// Initializes a new instance of the <c>Style</c> class.
    /// </summary>
    /// <param name="basedOn">The value used for <paramref name="basedOn"/>.</param>
    /// <param name="applyToDerivedTypes">The value used for <paramref name="applyToDerivedTypes"/>.</param>
    public Style(Style<T> basedOn, bool applyToDerivedTypes)
        : this((Style)basedOn)
    {
        mauiStyle.ApplyToDerivedTypes = applyToDerivedTypes;
    }

    /// <summary>
    /// Initializes a new instance of the <c>Style</c> class.
    /// </summary>
    /// <param name="basedOn">The value used for <paramref name="basedOn"/>.</param>
    /// <param name="buildSetters">The value used for <paramref name="buildSetters"/>.</param>
    public Style(Style<T> basedOn, Func<SettersContext<T>, SettersContext<T>> buildSetters)
        : this((Style)basedOn)
    {
        BuildSetters(buildSetters);
    }

    /// <summary>
    /// Initializes a new instance of the <c>Style</c> class.
    /// </summary>
    /// <param name="basedOn">The value used for <paramref name="basedOn"/>.</param>
    /// <param name="applyToDerivedTypes">The value used for <paramref name="applyToDerivedTypes"/>.</param>
    /// <param name="buildSetters">The value used for <paramref name="buildSetters"/>.</param>
    public Style(Style<T> basedOn, bool applyToDerivedTypes, Func<SettersContext<T>, SettersContext<T>> buildSetters)
        : this((Style)basedOn)
    {
        mauiStyle.ApplyToDerivedTypes = applyToDerivedTypes;
        BuildSetters(buildSetters);
    }

    /// <summary>
    /// Builds the configuration for the <c>BuildSetters</c> operation.
    /// </summary>
    /// <param name="buildSetters">The value used for <paramref name="buildSetters"/>.</param>
    private void BuildSetters(Func<SettersContext<T>, SettersContext<T>> buildSetters)
    {
        SettersContext<T> settersContext = new SettersContext<T>();
        buildSetters(settersContext);
        foreach (Setter xamlSetter in settersContext.XamlSetters)
        {
            mauiStyle.Setters.Add(xamlSetter);
        }
    }

    /// <summary>
    /// Executes the <c>Add</c> operation.
    /// </summary>
    /// <param name="invokeOnElement">The value used for <paramref name="invokeOnElement"/>.</param>
    public void Add(Action<T> invokeOnElement)
    {
        mauiStyle.Setters.Add(new Setter
        {
            Property = AttachedStyleInvokeProperty,
            Value = invokeOnElement
        });
    }

    /// <summary>
    /// Executes the <c>Add</c> operation.
    /// </summary>
    /// <param name="setter">The value used for <paramref name="setter"/>.</param>
    public void Add(Setter setter)
    {
        mauiStyle.Setters.Add(setter);
    }

    /// <summary>
    /// Executes the <c>Add</c> operation.
    /// </summary>
    /// <param name="trigger">The value used for <paramref name="trigger"/>.</param>
    public void Add(TriggerBase trigger)
    {
        mauiStyle.Triggers.Add(trigger);
    }

    /// <summary>
    /// Executes the <c>Add</c> operation.
    /// </summary>
    /// <param name="group">The value used for <paramref name="group"/>.</param>
    public void Add(VisualStateGroup group)
    {
        mauiStyle.GetVisualStateGroupList().Add(group);
    }

    /// <summary>
    /// Executes the <c>Add</c> operation.
    /// </summary>
    /// <param name="visualState">The value used for <paramref name="visualState"/>.</param>
    public void Add(VisualState visualState)
    {
        mauiStyle.GetVisualStateGroupList().GetCommonStatesVisualStateGroup().States.Add(visualState);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return mauiStyle.Setters.GetEnumerator();
    }
}