using AndroidX.AppCompat.Widget;
using astator.Core.UI.Base;

namespace astator.Core.UI.Controls;

public class ScriptButton : AppCompatButton, IControl
{
    public string CustomId { get; set; }
    public OnAttachedListener OnAttachedListener { get; set; }

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        this.OnAttachedListener?.OnAttached(this);
    }

    public ScriptButton(Android.Content.Context context, ViewArgs args) : base(context)
    {
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
