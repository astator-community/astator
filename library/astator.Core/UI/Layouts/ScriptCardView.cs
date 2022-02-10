using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using AndroidX.CardView.Widget;
using astator.Core.UI.Base;

namespace astator.Core.UI.Layouts;

public class ScriptCardView : CardView, ILayout
{
    public string CustomId { get; set; }

    public OnAttachedListener OnAttachedListener { get; set; }
    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        this.OnAttachedListener?.OnAttached(this);
    }

    ILayout ILayout.AddView(View child)
    {
        base.AddView(child);
        return this;
    }
    public ScriptCardView(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.LayoutParameters = new(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

        this.SetCustomId(ref args);
        foreach (var item in args)
        {
            SetAttr(item.Key.ToString(), item.Value);
        }
    }
    public void SetAttr(string key, object value)
    {
        switch (key)
        {
            case "radius":
            {
                this.Radius = Util.DpParse(value);
                break;
            }
            case "elevation":
            {
                if (value is string temp) this.CardElevation = float.Parse(temp);
                break;
            }
            case "maxElevation":
            {
                if (value is string temp) this.MaxCardElevation = float.Parse(temp.Trim());
                break;
            }
            case "bg":
            {
                if (value is string temp) this.CardBackgroundColor = ColorStateList.ValueOf(Color.ParseColor(temp));
                if (value is Color color) this.CardBackgroundColor = ColorStateList.ValueOf(color);
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
            "radius" => this.Radius,
            "elevation" => this.Elevation,
            "maxElevation" => this.MaxCardElevation,
            _ => Util.GetAttr(this, key)
        };
    }
    public void On(string key, object listener)
    {
        this.OnListener(key, listener);
    }
}
