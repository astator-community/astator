using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Content;
using astator.Core.Exceptions;
using astator.Core.UI.Base;
using System;
using Attribute = Android.Resource.Attribute;

namespace astator.Core.UI.Controls;
public class ScriptEditText : AppCompatEditText, IControl
{
    public string CustomId { get; set; }
    public OnCreatedListener OnCreatedListener { get; set; }

    public ScriptEditText(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.BackgroundTintList = new ColorStateList(
            new int[][]
            {
                new int[] {-Attribute.StateFocused },
                new int[] { Attribute.StateFocused }
            },
            new int[]
            {
                DefaultTheme.TextColor,
                DefaultTheme.ColorPrimary
            });

        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            var drawable = this.TextCursorDrawable;
            drawable.SetColorFilter(new BlendModeColorFilter(DefaultTheme.ColorPrimary, BlendMode.SrcIn));
            this.TextCursorDrawable = drawable;
        }
        else
        {
            var cursorDrawableResField = Java.Lang.Class.FromType(typeof(TextView)).GetDeclaredField("mCursorDrawableRes");
            cursorDrawableResField.Accessible = true;
            var cursorDrawableRes = cursorDrawableResField.GetInt(this);

            var editorField = Java.Lang.Class.FromType(typeof(TextView)).GetDeclaredField("mEditor");
            editorField.Accessible = true;
            var editor = editorField.Get(this);

            var cursorDrawableField = Java.Lang.Class.ForName("android.widget.Editor").GetDeclaredField("mCursorDrawable");
            cursorDrawableField.Accessible = true;
            var drawables = new Drawable[2];
            drawables[0] = ContextCompat.GetDrawable(this.Context, cursorDrawableRes);
            drawables[0].SetColorFilter(DefaultTheme.ColorPrimary, PorterDuff.Mode.SrcIn);
            drawables[1] = ContextCompat.GetDrawable(this.Context, cursorDrawableRes);
            drawables[1].SetColorFilter(DefaultTheme.ColorPrimary, PorterDuff.Mode.SrcIn);
            cursorDrawableField.Set(editor, drawables);
        }

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
            case "hint":
            {
                if (value is string temp) this.Hint = temp;
                break;
            }
            case "hintTextColor":
            {
                if (value is string temp) SetHintTextColor(Color.ParseColor(temp.Trim()));
                else if (value is Color color) SetHintTextColor(color);

                break;
            }
            case "inputType":
            {
                this.InputType = Util.EnumParse<InputTypes>(value);
                break;
            }
            case "singleLine":
            {
                SetSingleLine(Convert.ToBoolean(value));
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
            "hint" => this.Hint,
            "hintTextColor" => this.HintTextColors,
            "inputType" => this.InputType,
            "singleLine" => new Func<object>(() =>
            {
                if (OperatingSystem.IsAndroidVersionAtLeast(29))
                {
                    return this.IsSingleLine;
                }
                throw new SdkNotSupportedException(29);
            }),
            _ => Util.GetAttr(this, key)
        };
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
}
