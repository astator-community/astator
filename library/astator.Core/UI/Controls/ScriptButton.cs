using AndroidX.AppCompat.Widget;
using astator.Core.UI.Base;
using Color = Android.Graphics.Color;

namespace astator.Core.UI.Controls;

public class ScriptButton : AppCompatButton, IControl
{
    public string CustomId { get; set; }
    public OnCreatedListener OnCreatedListener { get; set; }

    private Color backgroundColor = DefaultTheme.ColorPrimary;

    public ScriptButton(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.SetDefaultValue(ref args);
        args["bg"] ??= this.backgroundColor;
        foreach (var item in args)
        {
            SetAttr(item.Key.ToString(), item.Value);
        }
    }

    public void SetAttr(string key, object value)
    {
        switch (key)
        {
            default:
                {
                    Util.SetAttr(this, key, value);
                    break;
                }
        }
    }

    public object GetAttr(string key)
    {
        return Util.GetAttr(this, key);
    }

    public void On(string key, object listener)
    {
        this.OnListener(key, listener);
    }
}
