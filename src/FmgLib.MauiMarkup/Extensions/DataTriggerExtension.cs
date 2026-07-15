namespace FmgLib.MauiMarkup;

public static partial class DataTriggerExtension
{

    public static DataTrigger Binding(this DataTrigger self,
            Func<Binding, Binding> bindingBuilder)
    {
        self.Binding = bindingBuilder(new Binding());
        return self;
    }

}