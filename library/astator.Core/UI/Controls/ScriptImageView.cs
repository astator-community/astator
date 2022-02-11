using AndroidX.AppCompat.Widget;
using astator.Core.UI.Base;
using System.IO;

namespace astator.Core.UI.Controls;

public class ScriptImageView : AppCompatImageView, IControl
{
    public string CustomId { get; set; }
    public OnAttachedListener OnAttachedListener { get; set; }

    private readonly string workDir;

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        this.OnAttachedListener?.OnAttached(this);
    }

    public ScriptImageView(Android.Content.Context context, string workDir, ViewArgs args) : base(context)
    {
        this.workDir = workDir;

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
            case "src":
            {
                if (value is string temp)
                {
                    var path = temp;
                    if (!path.StartsWith("/"))
                    {
                        path = System.IO.Path.Combine(this.workDir, "assets", path);
                    }
                    if (!File.Exists(path))
                    {
                        throw new FileNotFoundException(path + ": open failed! (No such file or directory)");
                    }
                    SetImageURI(Android.Net.Uri.FromFile(new Java.IO.File(path)));
                }
                break;
            }
            case "scaleType":
            {
                SetScaleType(Util.TypeParse<ScaleType>(value));
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
            "scaleType" => GetScaleType(),
            _ => Util.GetAttr(this, key)
        };
    }
    public void On(string key, object listener)
    {
        this.OnListener(key, listener);
    }
}
