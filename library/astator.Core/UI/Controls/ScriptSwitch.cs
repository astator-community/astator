using Android.Content.Res;
using Android.Graphics;
using Android.Widget;
using astator.Core.UI.Base;
using static Android.Views.ViewGroup;

namespace astator.Core.UI.Controls;

public class ScriptSwitch : Switch, IControl
{
    public string CustomId { get; set; } = string.Empty;

    private Color color = Color.ParseColor("#2B0B98");

    public OnAttachedListener OnAttachedListener { get; set; }

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        this.OnAttachedListener?.OnAttached(this);
    }

    public ScriptSwitch(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.LayoutParameters = new MarginLayoutParams(this.LayoutParameters ?? new(LayoutParams.MatchParent, LayoutParams.WrapContent));

        args ??= new ViewArgs();

        if (args["id"] is null)
        {
            this.CustomId = $"{ GetType().Name }-{ UiManager.CreateCount }";
            UiManager.CreateCount++;
        }

        if (args["color"] is not null)
        {
            this.color = Color.ParseColor(args["color"].ToString());
        }
        this.ThumbDrawable?.SetColorFilter(new PorterDuffColorFilter(this.color, PorterDuff.Mode.Multiply));
        this.TrackTintList = ColorStateList.ValueOf(Color.ParseColor("#bdbdbd"));

        CheckedChange += (s, e) =>
        {
            if (e.IsChecked)
            {
                this.TrackTintList = ColorStateList.ValueOf(this.color);
            }
            else
            {
                this.TrackTintList = ColorStateList.ValueOf(Color.ParseColor("#bdbdbd"));
            }
        };

        foreach (var item in args)
        {
            SetAttr(item.Key.ToString(), item.Value);
        }
    }

    public void SetAttr(string key, object value)
    {
        switch (key)
        {
            case "checked":
            {
                if (value is string temp)
                {
                    this.Checked = temp == bool.TrueString.ToLower();
                }

                break;
            }
            case "color":
            {
                if (value is string temp)
                {
                    this.color = Color.ParseColor(temp);
                    this.ThumbDrawable?.SetColorFilter(new PorterDuffColorFilter(this.color, PorterDuff.Mode.Multiply));
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
            "checked" => this.Checked,
            "color" => this.color,
            _ => Util.GetAttr(this, key)
        };
    }

    public void On(string key, object listener)
    {
        switch (key)
        {
            case "changed":
            {
                if (listener is OnCheckedChangeListener temp)
                {
                    SetOnCheckedChangeListener(temp);
                }

                break;
            }
            default:
                this.OnListener(key, listener);
                break;
        }
    }
}
