using Android.Content.Res;
using Android.Graphics;
using Android.Widget;
using astator.Core.UI.Base;
using System;
using static Android.Views.ViewGroup;
using Attribute = Android.Resource.Attribute;

namespace astator.Core.UI.Controls;

public class ScriptSwitch : Switch, IControl
{
    public string CustomId { get; set; }

    private Color color = DefaultTheme.ColorAccent;

    public OnCreatedListener OnCreatedListener { get; set; }

    public ScriptSwitch(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.LayoutParameters = new MarginLayoutParams(this.LayoutParameters ?? new(LayoutParams.MatchParent, LayoutParams.WrapContent));

        this.SetDefaultValue(ref args);
        if (args["color"] is not null)
        {
            this.color = Color.ParseColor(args["color"].ToString());
        }

        this.ThumbDrawable?.SetColorFilter(new PorterDuffColorFilter(this.color, PorterDuff.Mode.Multiply));
        this.TrackTintList = new ColorStateList(
            new int[][]
            {
                new int[] {-Attribute.StateChecked},
                new int[] { Attribute.StateChecked }
            },
            new int[]
            {
                Color.ParseColor("#bdbdbd"),
                this.color
            }
        );


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
                this.Checked = Convert.ToBoolean(value);
                break;
            }
            case "color":
            {
                if (value is string temp)
                {
                    this.color = Color.ParseColor(temp);
                }
                else if (value is Color color)
                {
                    this.color = color;
                }
                this.ThumbDrawable?.SetColorFilter(new PorterDuffColorFilter(this.color, PorterDuff.Mode.Multiply));
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
