using Android.Views;
using Android.Widget;
using astator.Core.UI.Base;

namespace astator.Core.UI.Layouts;

public class ScriptRadioGroup : RadioGroup, ILayout
{
    public string CustomId { get; set; } = string.Empty;
    public OnAttachedListener OnAttachedListener { get; set; }

    private int position;
    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        Check(GetChildAt(this.position).Id);
        this.OnAttachedListener?.OnAttached(this);
    }
    ILayout ILayout.AddView(View view)
    {
        base.AddView(view);
        return this;
    }
    public ScriptRadioGroup(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
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
            case "position":
            {
                if (value is string temp)
                {
                    this.position = int.Parse(temp.Trim());
                }

                break;
            }
            case "orientation":
            {
                try
                {
                    if (value is string temp)
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
            "position" => IndexOfChild(FindViewById(this.CheckedRadioButtonId)),
            _ => Util.GetAttr(this, key)
        };
    }
    public void On(string key, object callback)
    {
        switch (key)
        {
            case "Changed":
            {
                if (callback is RadioGroupOnCheckedChangeListener temp)
                {
                    SetOnCheckedChangeListener(temp);
                }

                break;
            }
            default:
                this.OnListener(key, callback);
                break;
        }
    }
}
