using Android.Graphics;
using AndroidX.AppCompat.Widget;
using astator.Core.UI.Base;
using System.IO;
using static Android.Views.ViewGroup;

namespace astator.Core.UI.Controls;

public class ScriptImageView : AppCompatImageView, IControl
{
    public string CustomId { get; set; } = string.Empty;
    public OnAttachedListener OnAttachedListener { get; set; }

    private readonly string workingDirectory;

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        this.OnAttachedListener?.OnAttached(this);
    }

    public ScriptImageView(Android.Content.Context context, string workingDirectory, ViewArgs args) : base(context)
    {
        this.workingDirectory = workingDirectory;
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
            case "src":
            {
                if (value is string temp)
                {
                    var path = temp;
                    if (!path.StartsWith("/"))
                    {
                        path = System.IO.Path.Combine(this.workingDirectory, path);
                    }
                    if (!File.Exists(path))
                    {
                        throw new FileNotFoundException(path + ": open failed! (No such file or directory)");
                    }
                    var bitmap = BitmapFactory.DecodeFile(path);
                    SetImageBitmap(bitmap);
                }
                break;
            }
            case "radius":
            {
                this.ClipToOutline = true;
                this.OutlineProvider = new RadiusOutlineProvider(Util.DpParse(value));
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
