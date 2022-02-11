using Android.Content;
using Android.Views;
using Android.Widget;
using astator.Core.UI.Base;

namespace astator.Core.UI.Layouts;
public class ScriptTabbedView : FrameLayout, ILayout
{
    public string CustomId { get; set; }
    public OnAttachedListener OnAttachedListener { get; set; }
    internal string Icon { get; private set; }
    internal string EnableIcon { get; private set; }
    internal string Title { get; private set; }

    public ScriptTabbedView(Context context, ViewArgs args) : base(context)
    {
        this.SetDefaultValue(ref args);

        if (args["icon"] is string icon) this.Icon = icon;
        if (args["enableIcon"] is string enableIcon) this.EnableIcon = enableIcon;
        if (this.EnableIcon is null) this.EnableIcon = this.Icon;
        if (args["title"] is string title) this.Title = title;


        args.Remove("icon", "enableIcon", "title");
        foreach (var item in args)
        {
            SetAttr(item.Key.ToString(), item.Value);
        }
    }

    ILayout ILayout.AddView(View view)
    {
        base.AddView(view);
        return this;
    }

    public void SetAttr(string key, object value)
    {
        Util.SetAttr(this, key, value);
    }

    public object GetAttr(string key)
    {
        return Util.GetAttr(this, key);
    }

    public void On(string key, object listener)
    {
        Util.OnListener(this, key, listener);
    }
}
