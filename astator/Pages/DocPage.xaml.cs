using Android.Views;
using astator.Core.UI.Controls;
using Microsoft.Maui.Handlers;

namespace astator.Pages;

public partial class DocPage : ContentPage
{
    public DocPage()
    {
        InitializeComponent();
    }

    public bool OnBackPressed()
    {
        if ((this.Web.Handler.PlatformView as ScriptWebView).CanGoBack())
        {
            (this.Web.Handler.PlatformView as ScriptWebView).GoBack();
            return true;
        }
        return false;
    }
}

public class CustomWebViewHandler : ViewHandler<IWebView, ScriptWebView>
{
    public static PropertyMapper<WebView, CustomWebViewHandler> Mapper = new(ViewMapper)
    {

    };

    public CustomWebViewHandler() : base(Mapper)
    {

    }

    public CustomWebViewHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(mapper, commandMapper)
    {
    }

    protected override ScriptWebView CreatePlatformView()
    {
        return new ScriptWebView(this.Context, new Core.UI.Base.ViewArgs
        {
            ["url"] = "https://astator.gitee.io/docs"
        }); ;
    }
}

