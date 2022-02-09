using Android.Views;
using Android.Webkit;
using astator.Core.UI.Base;

namespace astator.Core.UI.Controls;

public class ScriptWebView : WebView, IControl
{
    public string CustomId { get; set; } = string.Empty;
    public OnAttachedListener OnAttachedListener { get; set; }
    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        this.OnAttachedListener?.OnAttached(this);
    }

    public ScriptWebView(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.LayoutParameters = new MarginLayoutParams(this.LayoutParameters ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
        if (args is null)
        {
            return;
        }
        if (args["id"] is null)
        {
            this.CustomId = $"{ GetType().Name }-{ UiManager.CreateCount }";
            UiManager.CreateCount++;
        }
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
                if (value is string temp)
                {
                    LoadUrl(temp);
                }

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
}
