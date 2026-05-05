using System.Collections;

namespace FmgLib.MauiMarkup;

public class VisualState<T> : IEnumerable where T : BindableObject
{
    private static readonly BindableProperty AttachedVisualStateInvokeProperty = BindableProperty.CreateAttached("VisualState.AttachedInvokeProperty", typeof(Action<T>), typeof(VisualState<T>), null, BindingMode.OneWay, null, OnAttachedInvokeChanged);

    private VisualState mauiVisualState;

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
    /// Executes the <c>VisualState</c> operation.
    /// </summary>
    /// <param name="visualState">The value used for <paramref name="visualState"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public static implicit operator VisualState(VisualState<T> visualState)
    {
        return visualState.mauiVisualState;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return mauiVisualState.Setters.GetEnumerator();
    }

    /// <summary>
    /// Executes the <c>Add</c> operation.
    /// </summary>
    /// <param name="setter">The value used for <paramref name="setter"/>.</param>
    public void Add(Setter setter)
    {
        mauiVisualState.Setters.Add(setter);
    }

    /// <summary>
    /// Executes the <c>Add</c> operation.
    /// </summary>
    /// <param name="triggerBase">The value used for <paramref name="triggerBase"/>.</param>
    public void Add(StateTriggerBase triggerBase)
    {
        mauiVisualState.StateTriggers.Add(triggerBase);
    }

    /// <summary>
    /// Executes the <c>Add</c> operation.
    /// </summary>
    /// <param name="invokeOnElement">The value used for <paramref name="invokeOnElement"/>.</param>
    public void Add(Action<T> invokeOnElement)
    {
        mauiVisualState.Setters.Add(new Setter
        {
            Property = AttachedVisualStateInvokeProperty,
            Value = invokeOnElement
        });
    }

    /// <summary>
    /// Initializes a new instance of the <c>VisualState</c> class.
    /// </summary>
    /// <param name="name">The value used for <paramref name="name"/>.</param>
    public VisualState(string name)
    {
        mauiVisualState = new VisualState();
        mauiVisualState.Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <c>VisualState</c> class.
    /// </summary>
    public VisualState()
        : this(Guid.NewGuid().ToString())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <c>VisualState</c> class.
    /// </summary>
    /// <param name="buildSetters">The value used for <paramref name="buildSetters"/>.</param>
    public VisualState(Func<SettersContext<T>, SettersContext<T>> buildSetters)
        : this()
    {
        BuildSetters(buildSetters);
    }

    /// <summary>
    /// Initializes a new instance of the <c>VisualState</c> class.
    /// </summary>
    /// <param name="name">The value used for <paramref name="name"/>.</param>
    /// <param name="buildSetters">The value used for <paramref name="buildSetters"/>.</param>
    public VisualState(string name, Func<SettersContext<T>, SettersContext<T>> buildSetters)
        : this(name)
    {
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
            mauiVisualState.Setters.Add(xamlSetter);
        }
    }
}
