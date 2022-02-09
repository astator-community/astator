using Android.Views;
using Android.Widget;
using astator.Core.UI.Base;

namespace astator.Core.UI.Layouts;

public class ScriptScrollView : ScrollView, ILayout
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

    public ScriptScrollView(Android.Content.Context context, ViewArgs args) : base(context)
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
            this.SetAttr(item.Key.ToString(), item.Value);
        }
    }

    public void On(string key, object listener)
    {
        this.OnListener(key, listener);
    }
}
