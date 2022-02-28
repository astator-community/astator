using Android.Content.Res;
using Android.Graphics;
using Android.Widget;
using astator.Core.UI.Base;
using System;

namespace astator.Core.UI.Controls;

public class ScriptCheckBox : CheckBox, IControl
{
    public string CustomId { get; set; }
    public OnCreatedListener OnCreatedListener { get; set; }

    public ScriptCheckBox(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.ButtonTintList = ColorStateList.ValueOf(DefaultTheme.ColorAccent);

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
            case "checked":
            {
                this.Checked = Convert.ToBoolean(value);
                break;
            }
            case "color":
            {
                if (value is string temp) this.ButtonTintList = ColorStateList.ValueOf(Color.ParseColor(temp));
                else if (value is Color color) this.ButtonTintList = ColorStateList.ValueOf(color);

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
            "color" => this.ButtonTintList,
            _ => Util.GetAttr(this, key)
        };
    }

    public void On(string key, object listener)
    {
        if (key == "changed")
        {
            SetOnCheckedChangeListener((OnCheckedChangeListener)listener);
        }
        else
            this.OnListener(key, listener);
    }
}
