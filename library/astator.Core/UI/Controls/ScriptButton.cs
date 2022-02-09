using AndroidX.AppCompat.Widget;
using astator.Core.UI.Base;
using static Android.Views.ViewGroup;

namespace astator.Core.UI.Controls;

public class ScriptButton : AppCompatButton, IControl
{
    public string CustomId { get; set; } = string.Empty;
    public OnAttachedListener OnAttachedListener { get; set; }

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        this.OnAttachedListener?.OnAttached(this);
    }

    public ScriptButton(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.LayoutParameters = new MarginLayoutParams(this.LayoutParameters ?? new(LayoutParams.WrapContent, LayoutParams.WrapContent));
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
            SetAttr(item.Key.ToString(), item.Value);
        }
    }

    public void SetAttr(string key, object value)
    {
        switch (key)
        {
            case "radius":
            {
                this.ClipToOutline = true;
                this.OutlineProvider = new RadiusOutlineProvider(Util.DpParse(value));
                break;
            }
            default:
            {
                Util.SetAttr(this, key, value);
                break;
            }
        }
    }

    public void On(string key, object listener)
    {
        this.OnListener(key, listener);
    }
}
