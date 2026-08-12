using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup.App;

public class AppStyles
{
    public static ResourceDictionary Default => new ResourceDictionary {

        // "ActivityIndicator"
        
        new Style<ActivityIndicator>(e => e
            .Color(e => e.OnLight(AppColors.Primary).OnDark(White))),
        
        // "IndicatorView"

        new Style<IndicatorView>(e => e
            .IndicatorColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray500))
            .SelectedIndicatorColor(e => e.OnLight(AppColors.Gray950).OnDark(AppColors.Gray100))),
        
        // "Border"

        new Style<Border>(e => e
            .Stroke(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray500))
            .StrokeShape(new Rectangle())
            .StrokeThickness(1)),

        // "BoxView"

        new Style<BoxView>(e => e
            .Color(e => e.OnLight(AppColors.Gray950).OnDark(AppColors.Gray200))),
        

        // "Button"

        new Style<Button>(e => e
            .TextColor(e => e.OnLight(White).OnDark(AppColors.Primary))
            .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(White))
            .FontFamily("OpenSansRegular")
            .FontSize(14)
            .CornerRadius(8)
            .Padding(new Thickness(14,10))
            .MinimumHeightRequest(44)
            .MinimumWidthRequest(44))
        {
             new VisualState<Button> (VisualStates.VisualElement.Normal, e => e
                    .TextColor(e => e.OnLight(White).OnDark(AppColors.Primary))
                    .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(White))),

                new VisualState<Button> (VisualStates.VisualElement.Disabled, e => e
                    .TextColor(e => e.OnLight(AppColors.Gray950).OnDark(AppColors.Gray200))
                    .BackgroundColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray600)))
        },

        // "CheckBox"

        new Style<CheckBox>(e => e
            .Color(e => e.OnLight(AppColors.Primary).OnDark(White))
            .MinimumHeightRequest(44)
            .MinimumWidthRequest(44))
        {
            new VisualState<CheckBox>(VisualStates.VisualElement.Normal),
            new VisualState<CheckBox>(VisualStates.VisualElement.Disabled, e => e
                .Color(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600))),
        },

        // "DatePicker"

        new Style<DatePicker>(e => e
            .TextColor(e => e.OnLight(AppColors.Gray900).OnDark(White))
            .BackgroundColor(Transparent)
            .FontFamily("OpenSansRegular")
            .FontSize(14)
            .MinimumHeightRequest(44)
            .MinimumWidthRequest(44))
        {
            new VisualState<DatePicker>(VisualStates.VisualElement.Normal),
            new VisualState<DatePicker>(VisualStates.VisualElement.Disabled, e => e
                .TextColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray500)))
        },
        
        // "Editor"

        new Style<Editor>(e => e
            .TextColor(e => e.OnLight(Black).OnDark(White))
            .BackgroundColor(Transparent)
            .FontFamily("OpenSansRegular")
            .FontSize(14)
            .PlaceholderColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray500))
            .MinimumHeightRequest(44)
            .MinimumWidthRequest(44))
        {
            new VisualState<Editor>(VisualStates.VisualElement.Normal),
            new VisualState<Editor>(VisualStates.VisualElement.Disabled, e => e
                .TextColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600)))
        },  
        

        // "Entry"

        new Style<Entry>(e => e
            .TextColor(e => e.OnLight(Black).OnDark(White))
            .BackgroundColor(Transparent)
            .FontFamily("OpenSansRegular")
            .FontSize(14)
            .PlaceholderColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray500))
            .MinimumHeightRequest(44)
            .MinimumWidthRequest(44))
        {
            new VisualState<Entry>(VisualStates.VisualElement.Normal),
            new VisualState<Entry>(VisualStates.VisualElement.Disabled, e => e
                .TextColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600)))
        },              

        // "Frame"
        //
        // Frame is [Obsolete] as of .NET 9 (use Border) but still ships and still works, so the
        // default style stays — a freshly created project that happens to use a Frame should look
        // right out of the box. The suppression is scoped to this one style so the template does
        // not hand every new project a build warning it did not ask for; delete the pragma pair
        // together with the style once MAUI actually removes the type.
#pragma warning disable CS0618 // Type or member is obsolete
        new Style<Frame>(e => e
            .HasShadow(false)
            .BorderColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray950))
            .CornerRadius(8)),
#pragma warning restore CS0618

        // "ImageButton"

        new Style<ImageButton>(e => e
            .Opacity(1.0)
            .BackgroundColor(Transparent)
            .BorderWidth(0)
            .BorderWidth(0)
            .MinimumHeightRequest(44)
            .MinimumWidthRequest(44))
        {
            new VisualState<ImageButton>(VisualStates.VisualElement.Normal),
            new VisualState<ImageButton>(VisualStates.VisualElement.Disabled, e => e
                .Opacity(0.5))
        },         
        

        // "Label"

        new Style<Label>(e => e
            .TextColor(e => e.OnLight(AppColors.Gray900).OnDark(White))
            .BackgroundColor(Transparent)
            .FontFamily("OpenSansRegular")
            .FontSize(14))
        {
            new VisualState<Label>(VisualStates.VisualElement.Normal),
            new VisualState<Label>(VisualStates.VisualElement.Disabled, e => e
                .TextColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600)))
        },

        // "ListView"
        //
        // Deprecated in favour of CollectionView, but still shipping and still functional — kept
        // and suppressed for the same reason as the Frame style above.
#pragma warning disable CS0618 // Type or member is obsolete
        new Style<ListView>(e => e
            .SeparatorColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray500))
            .RefreshControlColor(e => e.OnLight(AppColors.Gray900).OnDark(AppColors.Gray200))),
#pragma warning restore CS0618

        // "Picker"

        new Style<Picker>(e => e
            .TextColor(e => e.OnLight(AppColors.Gray900).OnDark(White))
            .TitleColor(e => e.OnLight(AppColors.Gray900).OnDark(AppColors.Gray200))
            .BackgroundColor(Transparent)
            .FontFamily("OpenSansRegular")
            .FontSize(14)
            .MinimumHeightRequest(44)
            .MinimumWidthRequest(44))
        {
            new VisualState<Picker>(VisualStates.VisualElement.Normal),
            new VisualState<Picker>(VisualStates.VisualElement.Disabled, e => e
                .TextColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600))
                .TitleColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600)))
        },  
        

        // "ProgressBar"

        new Style<ProgressBar>(e => e
            .ProgressColor(e => e.OnLight(AppColors.Primary).OnDark(White)))
        {
            new VisualState<ProgressBar>(VisualStates.VisualElement.Normal),
            new VisualState<ProgressBar>(VisualStates.VisualElement.Disabled, e => e
                .ProgressColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600)))
        },
        
        // "RadioButton"

        new Style<RadioButton>(e => e
            .BackgroundColor(Transparent)
            .TextColor(e => e.OnLight(Black).OnDark(White))
            .FontFamily("OpenSansRegular")
            .FontSize(14)
            .MinimumHeightRequest(44)
            .MinimumWidthRequest(44))
        {
            new VisualState<RadioButton>(VisualStates.VisualElement.Normal),
            new VisualState<RadioButton>(VisualStates.VisualElement.Disabled, e => e
                .TextColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600)))
        },            

        // "RefreshView"

        new Style<RefreshView>(e => e
            .RefreshColor(e => e.OnLight(AppColors.Gray900).OnDark(AppColors.Gray200))),
        
        // "SearchBar"

        new Style<SearchBar>(e => e
            .TextColor(e => e.OnLight(AppColors.Gray900).OnDark(White))
            .PlaceholderColor(AppColors.Gray500)
            .CancelButtonColor(AppColors.Gray500)
            .BackgroundColor(Transparent)
            .FontFamily("OpenSansRegular")
            .FontSize(14))
        {
            new VisualState<SearchBar>(VisualStates.VisualElement.Normal),
            new VisualState<SearchBar>(VisualStates.VisualElement.Disabled, e => e
                .TextColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600))
                .PlaceholderColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600)))
        },      

        // "SearchHandler"

        new Style<SearchHandler>(e => e
            .TextColor(e => e.OnLight(AppColors.Gray900).OnDark(White))
            .PlaceholderColor(AppColors.Gray500)
            .BackgroundColor(Transparent)
            .FontFamily("OpenSansRegular")
            .FontSize(14))
        {
            new VisualState<SearchHandler>(VisualStates.VisualElement.Normal),
            new VisualState<SearchHandler>(VisualStates.VisualElement.Disabled, e => e
                .TextColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600))
                .PlaceholderColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600)))
        },            

        // "Shadow"

        new Style<Shadow>(e => e
            .Radius(15)
            .Opacity(0.5f)
            .Brush(e => e.OnLight(White).OnDark(White))
            .Offset(new Point(10, 10))),
        
        // "Slider"

        new Style<Slider>(e => e
            .MinimumTrackColor(e => e.OnLight(AppColors.Primary).OnDark(White))
            .MaximumTrackColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray600))
            .ThumbColor(e => e.OnLight(AppColors.Primary).OnDark(White)))
        {
            new VisualState<Slider>(VisualStates.VisualElement.Normal),
            new VisualState<Slider>(VisualStates.VisualElement.Disabled, e => e
                .MinimumTrackColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600))
                .MaximumTrackColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600))
                .ThumbColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600)))
        },

        // "SwipeItem"

        new Style<SwipeItem>(e => e
            .BackgroundColor(e => e.OnLight(White).OnDark(Black))),            

        // "Switch"

        new Style<Switch>(e => e
            .OnColor(e => e.OnLight(AppColors.Primary).OnDark(White))
            .ThumbColor(e => e.OnLight(AppColors.Gray400).OnDark(AppColors.Gray500)))
        {
            new VisualState<Switch>(VisualStates.Switch.Normal),

            new VisualState<Switch>(VisualStates.Switch.Disabled, e => e
                .OnColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600))
                .ThumbColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600))),

            new VisualState<Switch>(VisualStates.Switch.On, e => e
                .OnColor(e => e.OnLight(AppColors.Secondary).OnDark(AppColors.Gray200))
                .ThumbColor(e => e.OnLight(AppColors.Primary).OnDark(White))),

            new VisualState<Switch>(VisualStates.Switch.Off, e => e
                .OnColor(e => e.OnLight(AppColors.Primary).OnDark(White))
                .ThumbColor(e => e.OnLight(AppColors.Gray400).OnDark(AppColors.Gray500)))
        },

        // "TimePicker"

        new Style<TimePicker>(e => e
            .TextColor(e => e.OnLight(AppColors.Gray900).OnDark(White))
            .BackgroundColor(Transparent)
            .FontFamily("OpenSansRegular")
            .FontSize(14)
            .MinimumHeightRequest(44)
            .MinimumWidthRequest(44))
        {
            new VisualState<TimePicker>(VisualStates.VisualElement.Normal),
            new VisualState<TimePicker>(VisualStates.VisualElement.Disabled, e => e
                .TextColor(e => e.OnLight(AppColors.Gray300).OnDark(AppColors.Gray600)))
        },            

        // "Page"

        new Style<Page>(applyToDerivedTypes: true, e => e
            .Padding(0)
            .BackgroundColor(e => e.OnLight(White).OnDark(Black))),
        
        // "Shell"

        new Style<Shell>(applyToDerivedTypes: true, e => e
            .ShellNavBarHasShadow(false)
            .ShellBackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(AppColors.Gray950))
            .ShellForegroundColor(e => e.OnLight(AppColors.Primary).OnDark(White))
            .ShellTitleColor(e => e.OnLight(White).OnDark(White))
            .ShellDisabledColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray950))
            .ShellUnselectedColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray200))
            .ShellTabBarBackgroundColor(e => e.OnLight(White).OnDark(Black))
            .ShellTabBarForegroundColor(e => e.OnLight(AppColors.Primary).OnDark(White))
            .ShellTabBarTitleColor(e => e.OnLight(AppColors.Primary).OnDark(White))
            .ShellTabBarUnselectedColor(e => e.OnLight(AppColors.Gray900).OnDark(AppColors.Gray200))),

        // "NavigationPage"

        new Style<NavigationPage>(e => e
            .BarBackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(AppColors.Gray950))
            .BarTextColor(e => e.OnLight(AppColors.Gray200).OnDark(White))),
        
        // "TabbedPage"

        new Style<TabbedPage>(e => e
            .BarBackgroundColor(e => e.OnLight(White).OnDark(AppColors.Gray950))
            .BarTextColor(e => e.OnLight(AppColors.Primary).OnDark(White))
            .UnselectedTabColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray500))
            .SelectedTabColor(e => e.OnLight(AppColors.Gray950).OnDark(AppColors.Gray100)))
    };
}
