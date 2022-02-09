using Android.Graphics;
using Android.Text;
using AndroidX.AppCompat.Widget;
using astator.Core.Exceptions;
using astator.Core.UI.Base;
using System;
using static Android.Views.ViewGroup;

namespace astator.Core.UI.Controls;

public class ScriptEditText : AppCompatEditText, IControl
{
    public string CustomId { get; set; } = string.Empty;
    public OnAttachedListener OnAttachedListener { get; set; }

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        this.OnAttachedListener?.OnAttached(this);
    }

    public ScriptEditText(Android.Content.Context context, ViewArgs args) : base(context)
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
            case "hint":
            {
                if (value is string temp)
                {
                    this.Hint = temp;
                }

                break;
            }
            case "hintTextColor":
            {
                if (value is string temp)
                {
                    SetHintTextColor(Color.ParseColor(temp.Trim()));
                }

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
            "hintTextColor" => this.CurrentHintTextColor,
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
                if (listener is TextWatcher temp)
                {
                    AddTextChangedListener(temp);
                }

                break;
            }
            default:
                this.OnListener(key, listener);
                break;
        }
    }
}
