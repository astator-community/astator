using Android.Views;
using Android.Widget;
using astator.Core.UI.Base;

namespace astator.Core.UI.Layouts;

public class ScriptLinearLayout : LinearLayout, ILayout
{
    public string CustomId { get; set; }

    public OnCreatedListener OnCreatedListener { get; set; }
    public new ILayout AddView(View view)
    {
        base.AddView(view);
        return this;
    }

    public ScriptLinearLayout(Android.Content.Context context, ViewArgs args) : base(context)
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
            case "orientation":
                {
                    try
                    {
                        if (value is int v)
                        {
                            this.Orientation = (Orientation)v;
                        }
                        else if (value is string temp)
                        {
                            this.Orientation = (Orientation)int.Parse(temp);
                        }
                    }
                    catch
                    {
                        this.Orientation = Util.EnumParse<Orientation>(value);
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
            "orientation" => this.Orientation,
            _ => Util.GetAttr(this, key),
        };
    }
    public void On(string key, object listener)
    {
        this.OnListener(key, listener);
    }
}
