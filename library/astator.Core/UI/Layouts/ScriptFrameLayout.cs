using Android.Views;
using AndroidX.AppCompat.Widget;
using astator.Core.UI.Base;
namespace astator.Core.UI.Layouts;

public class ScriptFrameLayout : ContentFrameLayout, ILayout
{
    public string CustomId { get; set; }

    public OnCreatedListener OnCreatedListener { get; set; }

    public new ILayout AddView(View view)
    {
        base.AddView(view);
        return this;
    }

    public ScriptFrameLayout(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.SetDefaultValue(ref args);
        foreach (var item in args)
        {
            SetAttr(item.Key.ToString(), item.Value);
        }
    }

    public void On(string key, object listener)
    {
        this.OnListener(key, listener);
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
