using Android.Webkit;
using astator.Core.UI.Base;

namespace astator.Core.UI.Controls;

public class ScriptWebViewClient : WebViewClient
{
    public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
    {
        view.LoadUrl(request.Url.ToString());
        return true;
    }
}

public class ScriptWebView : WebView, IControl
{
    public string CustomId { get; set; }
    public OnCreatedListener OnCreatedListener { get; set; }

    public ScriptWebView(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.LayoutParameters = new Android.Views.ViewGroup.LayoutParams(Android.Views.ViewGroup.LayoutParams.MatchParent, Android.Views.ViewGroup.LayoutParams.MatchParent);

        this.Settings.JavaScriptEnabled = true;
        SetWebViewClient(new ScriptWebViewClient());
        this.SetDefaultValue(ref args);
        foreach (var item in args)
        {
            SetAttr(item.Key.ToString(), item.Value);
        }
    }

    public void SetAttr(string key, object value)
    {
        switch (key)
        {
            case "url":
            {
                if (value is string temp) LoadUrl(temp);
                break;
            }
            default:
            {
                Util.SetAttr(this, key, value);
                break;
            }
        }
    }
    public object GetAttr(string key)
    {
        return key switch
        {
            "url" => this.Url,
            _ => Util.GetAttr(this, key)
        };
    }
    public void On(string key, object listener)
    {
        this.OnListener(key, listener);
    }

    public override void LoadUrl(string url)
    {
        base.LoadUrl(url);
    }
}
