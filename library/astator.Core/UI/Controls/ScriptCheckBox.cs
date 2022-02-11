using Android.Content.Res;
using Android.Graphics;
using Android.Widget;
using astator.Core.UI.Base;

namespace astator.Core.UI.Controls;

public class ScriptCheckBox : CheckBox, IControl
{
    public string CustomId { get; set; }
    public OnAttachedListener OnAttachedListener { get; set; }

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        this.OnAttachedListener?.OnAttached(this);
    }

    public ScriptCheckBox(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.ButtonTintList = ColorStateList.ValueOf(Color.ParseColor("#808080"));

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
                if (value is string temp)
                {
                    this.Checked = temp.ToLower() == bool.TrueString.ToLower();
                }

                break;
            }
            case "color":
            {
                if (value is string temp)
                {
                    this.ButtonTintList = ColorStateList.ValueOf(Color.ParseColor(temp));
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
            "color" => this.ButtonTintList,
            _ => Util.GetAttr(this, key)
        };
    }

    public void On(string key, object listener)
    {
        this.OnListener(key, listener);
    }
}
