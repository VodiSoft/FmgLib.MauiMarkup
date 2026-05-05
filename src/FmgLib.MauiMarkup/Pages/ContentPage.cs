using System.Diagnostics;

namespace FmgLib.MauiMarkup;

public abstract class FmgLibContentPage : ContentPage, IFmgLibHotReload
{
    /// <summary>
    /// Initializes a new instance of the <c>FmgLibContentPage</c> class.
    /// </summary>
    protected FmgLibContentPage()
    {
        if (Debugger.IsAttached)
        {
            FmgLibHotReloadHandler.UpdateApplicationEvent += ReloadUI;
        }

        Build();
    }

    public abstract void Build();

    /// <summary>
    /// Executes the <c>ReloadUI</c> operation.
    /// </summary>
    /// <param name="obj">The value used for <paramref name="obj"/>.</param>
    protected void ReloadUI(Type[]? obj)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Build();
        });
    }
}

public abstract class FmgLibContentPage<TViewModel> : FmgLibContentPage, IFmgLibHotReload
{
    protected new TViewModel BindingContext => (TViewModel)base.BindingContext;

    /// <summary>
    /// Initializes a new instance of the <c>FmgLibContentPage</c> class.
    /// </summary>
    /// <param name="viewModel">The value used for <paramref name="viewModel"/>.</param>
    protected FmgLibContentPage(TViewModel viewModel)
    {
        base.BindingContext = viewModel;

        if (Debugger.IsAttached)
        {
            FmgLibHotReloadHandler.UpdateApplicationEvent += ReloadUI;
        }

        Build();
    }
}
