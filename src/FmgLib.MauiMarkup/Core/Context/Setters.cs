namespace FmgLib.MauiMarkup;

public class Setters<T> : List<Setter> where T : BindableObject
	{
    /// <summary>
    /// Initializes a new instance of the <c>Setters</c> class.
    /// </summary>
    /// <param name="buildSetters">The value used for <paramref name="buildSetters"/>.</param>
    public Setters(Func<SettersContext<T>, SettersContext<T>> buildSetters)
    {
        var settersContext = new SettersContext<T>();
        buildSetters(settersContext);
        foreach (var setter in settersContext.XamlSetters)
            this.Add(setter);
    }
}