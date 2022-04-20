using AndroidX.AppCompat.Widget;
using astator.Core.UI.Base;

namespace astator.Core.UI.Controls;

public class ScriptTextView : AppCompatTextView, IControl
{
    public string CustomId { get; set; }
    public OnCreatedListener OnCreatedListener { get; set; }

    public ScriptTextView(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.SetDefaultValue(ref args);
        foreach (var item in args)
        {
            SetAttr(item.Key.ToString(), item.Value);
        }
    }

    public void On(string key, object listener)
    {
        switch (key)
        {
            case "changed":
            {
                if (listener is TextWatcher temp) AddTextChangedListener(new ClassOfTextWatcher(this, temp));
                break;
            }
            default:
                this.OnListener(key, listener);
                break;
        }
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
