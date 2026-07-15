namespace FmgLib.MauiMarkup;

public abstract class FmgLibContentPage : ContentPage, IFmgLibHotReload
{
    /// <summary>
    /// Initializes a new instance of the <c>FmgLibContentPage</c> class.
    /// </summary>
    protected FmgLibContentPage()
        : this(initialize: true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <c>FmgLibContentPage</c> class, optionally deferring
    /// the first <see cref="Build"/> so derived classes can complete their own setup
    /// (e.g. assigning the binding context) before the UI is constructed.
    /// </summary>
    /// <param name="initialize">Whether to register for hot reload and build immediately.</param>
    private protected FmgLibContentPage(bool initialize)
    {
        if (initialize)
        {
            Initialize();
        }
    }

    /// <summary>
    /// Registers the page for hot reload (weakly — the page stays collectible; costs nothing in
    /// production where no update ever arrives) and runs the first build.
    /// </summary>
    private protected void Initialize()
    {
        FmgLibHotReloadHandler.Register(this);
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

public abstract class FmgLibContentPage<TViewModel> : FmgLibContentPage
{
    protected new TViewModel BindingContext => (TViewModel)base.BindingContext;

    /// <summary>
    /// Initializes a new instance of the <c>FmgLibContentPage</c> class.
    /// </summary>
    /// <param name="viewModel">The value used for <paramref name="viewModel"/>.</param>
    protected FmgLibContentPage(TViewModel viewModel)
        : base(initialize: false)
    {
        base.BindingContext = viewModel;
        Initialize();
    }
}
