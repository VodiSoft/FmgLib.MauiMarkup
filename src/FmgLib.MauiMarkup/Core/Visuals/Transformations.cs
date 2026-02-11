namespace FmgLib.MauiMarkup;

public static class Transformations
{
    /// <summary>
    /// Executes the <c>ColorTransform</c> operation.
    /// </summary>
    /// <param name="fromColor">The value used for <paramref name="fromColor"/>.</param>
    /// <param name="toColor">The value used for <paramref name="toColor"/>.</param>
    /// <param name="t">The value used for <paramref name="t"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public static Color ColorTransform(Color fromColor, Color toColor, double t)
    {
        return Color.FromRgba((double)fromColor.Red + t * (double)(toColor.Red - fromColor.Red), (double)fromColor.Green + t * (double)(toColor.Green - fromColor.Green), (double)fromColor.Blue + t * (double)(toColor.Blue - fromColor.Blue), (double)fromColor.Alpha + t * (double)(toColor.Alpha - fromColor.Alpha));
    }

    /// <summary>
    /// Executes the <c>DoubleTransform</c> operation.
    /// </summary>
    /// <param name="from">The value used for <paramref name="from"/>.</param>
    /// <param name="to">The value used for <paramref name="to"/>.</param>
    /// <param name="t">The value used for <paramref name="t"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public static double DoubleTransform(double from, double to, double t)
    {
        return from + t * (to - from);
    }

    /// <summary>
    /// Executes the <c>SizeTransform</c> operation.
    /// </summary>
    /// <param name="from">The value used for <paramref name="from"/>.</param>
    /// <param name="to">The value used for <paramref name="to"/>.</param>
    /// <param name="t">The value used for <paramref name="t"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public static Size SizeTransform(Size from, Size to, double t)
    {
        return new Size(from.Width + t * (to.Width - from.Width), from.Height + t * (to.Height - from.Height));
    }

    /// <summary>
    /// Executes the <c>AnimateAsync</c> operation.
    /// </summary>
    /// <param name="element">The value used for <paramref name="element"/>.</param>
    /// <param name="name">The value used for <paramref name="name"/>.</param>
    /// <param name="transform">The value used for <paramref name="transform"/>.</param>
    /// <param name="callback">The value used for <paramref name="callback"/>.</param>
    /// <param name="length">The value used for <paramref name="length"/>.</param>
    /// <param name="easing">The value used for <paramref name="easing"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the operation succeeded.</returns>
    public static Task<bool> AnimateAsync<T>(VisualElement element, string name, Func<double, T> transform, Action<T> callback, uint length, Easing easing)
    {
        easing = easing ?? Easing.Linear;
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        element.Animate(name, transform, callback, 16u, length, easing, delegate (T value, bool c)
        {
            taskCompletionSource.SetResult(c);
        });

        return taskCompletionSource.Task;
    }
}
