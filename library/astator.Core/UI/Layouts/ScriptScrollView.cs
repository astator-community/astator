using Android.Views;
using Android.Widget;
using astator.Core.UI.Base;

namespace astator.Core.UI.Layouts;

public class ScriptScrollView : ScrollView, ILayout
{
    public string CustomId { get; set; }
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

    public ScriptScrollView(Android.Content.Context context, ViewArgs args) : base(context)
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
