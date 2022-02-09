using Android.Views;
using Android.Widget;
using astator.Core.UI.Base;

namespace astator.Core.UI.Layouts;

public class ScriptLinearLayout : LinearLayout, ILayout
{
    public string CustomId { get; set; } = string.Empty;

    public OnAttachedListener OnAttachedListener { get; set; }
    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        this.OnAttachedListener?.OnAttached(this);
    }
    ILayout ILayout.AddView(View view)
    {
        base.AddView(view);
        return this;
    }
    public ScriptLinearLayout(Android.Content.Context context, ViewArgs args) : base(context)
    {
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
            case "orientation":
            {
                try
                {
                    if (value is int v)
                    {
                        this.Orientation = (Orientation)v;
                    }
                    if (value is string temp)
                    {
                        this.Orientation = (Orientation)int.Parse(temp);
                    }
                }
                catch
                {
                    this.Orientation = Util.EnumParse<Orientation>(value);
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
            "orientation" => this.Orientation,
            _ => Util.GetAttr(this, key),
        };
    }
    public void On(string key, object listener)
    {
        this.OnListener(key, listener);
    }
}
