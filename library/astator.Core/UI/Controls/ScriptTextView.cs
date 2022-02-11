using Android.Text.Util;
using AndroidX.AppCompat.Widget;
using astator.Core.UI.Base;

namespace astator.Core.UI.Controls;

public class ScriptTextView : AppCompatTextView, IControl
{
    public string CustomId { get; set; }
    public OnAttachedListener OnAttachedListener { get; set; }

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        this.OnAttachedListener?.OnAttached(this);
    }

    public ScriptTextView(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.SetDefaultValue(ref args);
        if (args["autoLink"] is not null)
        {
            if (args["autoLink"] is string temp)
            {
                var mode = temp.Trim();
                if (mode == "web")
                {
                    this.AutoLinkMask = MatchOptions.WebUrls;
                }
                else if (mode == "email")
                {
                    this.AutoLinkMask = MatchOptions.EmailAddresses;
                }
                else if (mode == "phone")
                {
                    this.AutoLinkMask = MatchOptions.PhoneNumbers;
                }
                else if (mode == "map")
                {
                    this.AutoLinkMask = MatchOptions.MapAddresses;
                }
                else if (mode == "all")
                {
                    this.AutoLinkMask = MatchOptions.All;
                }
            }
        }

        foreach (var item in args)
        {
            SetAttr(item.Key.ToString(), item.Value);
        }
    }

    public void On(string key, object listener)
    {
        switch (key)
        {
            case "changed":
            {
                if (listener is TextWatcher temp)
                {
                    AddTextChangedListener(temp);
                }

                break;
            }
            default:
                this.OnListener(key, listener);
                break;
        }
    }

    public void SetAttr(string key, object value)
    {
        Util.SetAttr(this, key, value);
    }

    public object GetAttr(string key)
    {
        return Util.GetAttr(this, key);
    }
}
