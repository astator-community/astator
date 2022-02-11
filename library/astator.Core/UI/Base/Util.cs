using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text.Util;
using Android.Views;
using Android.Widget;
using astator.Core.Exceptions;
using astator.Core.UI.Controls;
using System;
using System.Collections.Generic;
using static Android.Text.TextUtils;
using static Android.Views.ViewGroup;
using static astator.Core.Globals;

namespace astator.Core.UI.Base;

public static class Util
{
    public static T TypeParse<T>(object value)
    {
        var str = value.ToString().Trim().ToLower();
        var properties = typeof(T).GetProperties();
        foreach (var p in properties)
        {
            if (p.Name.ToString().ToLower().Equals(str))
            {
                return (T)p.GetValue(null);
            }
        }
        throw new AttributeNotExistException(str);
    }

    public static T EnumParse<T>(object value)
    {
        var list = new List<int>();
        if (value is string strs)
        {
            var array = strs.Trim().ToLower().Split("|");
            foreach (var item in Enum.GetNames(typeof(T)))
            {
                foreach (var str in array)
                {
                    if (item.ToLower().Equals(str))
                    {
                        list.Add((int)Enum.Parse(typeof(T), item));
                    }
                };
            }
            if (list.Count != 0)
            {
                var result = 0;
                foreach (var v in list)
                {
                    result |= v;
                }
                return (T)(object)result;
            }
            else
            {
                throw new AttributeNotExistException(strs);
            }
        }
        throw new AttributeNotExistException(value.ToString() ?? string.Empty);
    }

    public static int DpParse(object value)
    {
        return (int)(Devices.Dp * float.Parse(value.ToString().Trim()));
    }
    public static T DpParse<T>(object value)
    {
        return (T)(object)(Devices.Dp * float.Parse(value.ToString().Trim()));
    }

    public static void OnListener(this IView view, string key, object listener)
    {
        var v = view as View;
        switch (key)
        {
            case "click":
            {
                if (listener is OnClickListener temp) v.SetOnClickListener(temp);
                break;
            }
            case "longClick":
            {
                if (listener is OnLongClickListener temp) v.SetOnLongClickListener(temp);
                break;
            }
            case "touch":
            {
                if (listener is OnTouchListener temp) v.SetOnTouchListener(temp);
                break;
            }
            case "attached":
            {
                if (listener is OnAttachedListener temp) view.OnAttachedListener = temp;
                break;
            }
            default: throw new AttributeNotExistException(key);
        }
    }
    public static void OnListener(this ILayout view, string key, object listener)
    {
        var v = view as View;
        switch (key)
        {
            case "scrollChange":
            {
                if (listener is OnScrollChangeListener temp) v.SetOnScrollChangeListener(temp);
                break;
            }
            default:
                var iView = view as IView;
                iView.OnListener(key, listener);
                break;
        }
    }

    public static void SetAttr(IView view, string key, object value)
    {
        if (view is TextView tv)
        {
            switch (key)
            {
                case "w":
                {
                    tv.SetWidth(DpParse(value));
                    return;
                }
                case "h":
                {
                    tv.SetHeight(DpParse(value));
                    return;
                }
                case "text":
                {
                    tv.Text = value.ToString();
                    return;
                }
                case "paintFlags":
                {
                    tv.PaintFlags = EnumParse<PaintFlags>(value);
                    return;
                }
                case "textSize":
                {
                    tv.SetTextSize(Android.Util.ComplexUnitType.Dip, Convert.ToInt32(value));
                    return;
                }
                case "textColor":
                {
                    if (value is string temp) tv.SetTextColor(Color.ParseColor(temp));
                    if (value is Color color) tv.SetTextColor(color);
                    return;
                }
                case "lines":
                {
                    tv.SetLines(DpParse(value));
                    return;
                }
                case "maxLines":
                {
                    tv.SetMaxLines(DpParse(value));
                    return;
                }
                case "typeface":
                {
                    var tf = TypeParse<Typeface>(value);
                    var style = tv.Typeface?.Style ?? tf.Style;
                    tv.SetTypeface(tf, style);
                    return;
                }
                case "textStyle":
                {
                    tv.SetTypeface(tv.Typeface, EnumParse<TypefaceStyle>(value));
                    return;
                }
                case "ems":
                {
                    tv.SetEms(DpParse(value));
                    return;
                }
                case "autoLink":
                {
                    if (value is string temp)
                    {
                        var mode = temp.Trim();
                        if (mode == "web") tv.AutoLinkMask = MatchOptions.WebUrls;
                        else if (mode == "email") tv.AutoLinkMask = MatchOptions.EmailAddresses;
                        else if (mode == "phone") tv.AutoLinkMask = MatchOptions.PhoneNumbers;
                        else if (mode == "map") tv.AutoLinkMask = MatchOptions.MapAddresses;
                        else if (mode == "all") tv.AutoLinkMask = MatchOptions.All;
                    }
                    return;
                }
                case "ellipsize":
                {
                    if (value is string temp) tv.Ellipsize = TruncateAt.ValueOf(temp.Trim());
                    return;
                }
                case "gravity":
                {
                    tv.Gravity = EnumParse<GravityFlags>(value);
                    return;
                }
            }
        }

        var v = view as View;
        FrameLayout.LayoutParams newLp = v is ViewGroup ? new(LayoutParams.MatchParent, LayoutParams.MatchParent) : new(LayoutParams.WrapContent, LayoutParams.WrapContent);
        var lp = v.LayoutParameters as FrameLayout.LayoutParams
            ?? new FrameLayout.LayoutParams(v.LayoutParameters as MarginLayoutParams ?? newLp);

        switch (key)
        {
            case "id":
            {
                if (value is string temp) view.CustomId = temp;
                break;
            }
            case "w":
            {
                lp.Width = DpParse(value);
                v.LayoutParameters = lp;
                break;
            }
            case "h":
            {
                lp.Height = DpParse(value);
                v.LayoutParameters = lp;
                break;
            }
            case "minWidth":
            {
                v.SetMinimumWidth(DpParse(value));
                break;
            }
            case "minHeight":
            {
                v.SetMinimumHeight(DpParse(value));
                break;
            }
            case "weight":
            {
                var _lp = v.LayoutParameters as LinearLayout.LayoutParams ?? new(v.LayoutParameters as MarginLayoutParams ?? new(LayoutParams.WrapContent, LayoutParams.WrapContent));
                _lp.Weight = Convert.ToInt32(value);
                v.LayoutParameters = _lp;
                break;
            }
            case "margin":
            {
                var margin = new int[4];
                if (value is int i32) margin[0] = margin[1] = margin[2] = margin[3] = i32;
                else if (value is int[] arr)
                {
                    margin[0] = DpParse(arr[0]);
                    margin[1] = DpParse(arr[1]);
                    margin[2] = DpParse(arr[2]);
                    margin[3] = DpParse(arr[3]);
                }
                else if (value is string str)
                {
                    var strArr = str.Split(",");
                    if (strArr.Length == 1)
                    {
                        var temp = DpParse(strArr[0]);
                        margin[0] = margin[1] = margin[2] = margin[3] = temp;
                    }
                    else if (strArr.Length == 2)
                    {
                        margin[0] = margin[2] = DpParse(strArr[0]);
                        margin[1] = margin[3] = DpParse(strArr[1]);
                    }
                    else if (strArr.Length == 4)
                    {
                        margin[0] = DpParse(strArr[0]);
                        margin[1] = DpParse(strArr[1]);
                        margin[2] = DpParse(strArr[2]);
                        margin[3] = DpParse(strArr[3]);
                    }
                }
                lp.SetMargins(margin[0], margin[1], margin[2], margin[3]);
                v.LayoutParameters = lp;
                break;
            }
            case "layoutGravity":
            {
                lp.Gravity = EnumParse<GravityFlags>(value);
                v.LayoutParameters = lp;
                break;
            }
            case "padding":
            {
                var padding = new int[4];
                if (value is int i32) padding[0] = padding[1] = padding[2] = padding[3] = i32;
                else if (value is int[] arr)
                {
                    padding[0] = DpParse(arr[0]);
                    padding[1] = DpParse(arr[1]);
                    padding[2] = DpParse(arr[2]);
                    padding[3] = DpParse(arr[3]);
                }
                else if (value is string str)
                {
                    var strArr = str.Split(",");
                    if (strArr.Length == 1)
                    {
                        var temp = DpParse(strArr[0]);
                        padding[0] = padding[1] = padding[2] = padding[3] = temp;
                    }
                    else if (strArr.Length == 2)
                    {
                        padding[0] = padding[2] = DpParse(strArr[0]);
                        padding[1] = padding[3] = DpParse(strArr[1]);
                    }
                    else if (strArr.Length == 4)
                    {
                        padding[0] = DpParse(strArr[0]);
                        padding[1] = DpParse(strArr[1]);
                        padding[2] = DpParse(strArr[2]);
                        padding[3] = DpParse(strArr[3]);
                    }
                }
                v.SetPadding(padding[0], padding[1], padding[2], padding[3]);
                break;
            }
            case "alpha":
            {
                if (value is string temp) v.Alpha = float.Parse(temp.Trim());
                break;
            }
            case "bg":
            {
                if (value is string temp) v.SetBackgroundColor(Color.ParseColor(temp));
                if (value is Color color) v.SetBackgroundColor(color);
                break;
            }
            case "fg":
            {
                if (value is string temp) v.Foreground = new ColorDrawable(Color.ParseColor(temp.Trim()));
                if (value is Color color) v.SetBackgroundColor(color);
                break;
            }
            case "visibility":
            {
                v.Visibility = EnumParse<ViewStates>(value);
                break;
            }
            case "rotation":
            {
                if (value is string temp) v.Rotation = float.Parse(temp.Trim());
                break;
            }
            case "transformPivotX":
            {
                if (value is string temp) v.TranslationX = float.Parse(temp.Trim());
                break;
            }
            case "transformPivotY":
            {
                if (value is string temp) v.TranslationY = float.Parse(temp.Trim());
                break;
            }
            case "radius":
            {
                v.ClipToOutline = true;
                v.OutlineProvider = new RadiusOutlineProvider(DpParse<float>(value));
                break;
            }
            case "tag":
            {
                if (value is string temp) v.Tag = temp;
                if (value is int i32) v.Tag = i32;
                break;
            }
            default:
                throw new AttributeNotExistException(key);
        }
    }

    public static object GetAttr(IView view, string key)
    {
        if (view is TextView tv)
        {
            switch (key)
            {
                case "text": return tv.Text;
                case "textSize": return tv.TextSize;
                case "textColor": return tv.CurrentTextColor;
                case "maxLines": return tv.MaxLines;
                case "typeface": return tv.Typeface;
                case "autoLink": return tv.AutoLinkMask;
                case "ellipsize": return tv.Ellipsize;
                case "gravity": return tv.Gravity;
            };
        }

        var v = view as View;

        FrameLayout.LayoutParams newLp = v is ViewGroup ? new(LayoutParams.MatchParent, LayoutParams.MatchParent) : new(LayoutParams.WrapContent, LayoutParams.WrapContent);
        var lp = v.LayoutParameters as FrameLayout.LayoutParams
            ?? new FrameLayout.LayoutParams(v.LayoutParameters as MarginLayoutParams ?? newLp);

        return key switch
        {
            "id" => view.CustomId,
            "w" => v.Width,
            "h" => v.Height,
            "minWidth" => v.MinimumWidth,
            "minHeight" => v.MinimumHeight,
            "weight" => () =>
            {
                var _lp = v.LayoutParameters as LinearLayout.LayoutParams ?? new(v.LayoutParameters as MarginLayoutParams ?? new(LayoutParams.WrapContent, LayoutParams.WrapContent));
                return _lp.Weight;
            }
            ,
            "margin" => new int[] { lp.LeftMargin, lp.TopMargin, lp.RightMargin, lp.BottomMargin },
            "layoutGravity" => lp.Gravity,
            "padding" => new int[] { v.PaddingLeft, v.PaddingTop, v.PaddingRight, v.PaddingBottom },
            "alpha" => v.Alpha,
            "bg" => v.Background,
            "fg" => v.Foreground,
            "visibility" => v.Visibility,
            "rotation" => v.Rotation,
            "translationX" => v.TranslationX,
            "translationY" => v.TranslationY,
            _ => throw new AttributeNotExistException(key)
        };
    }

    public static void SetDefaultValue(this IView view, ref ViewArgs args)
    {
        args ??= new ViewArgs();
        args["id"] ??= $"scriptView-{UiManager.CreateCount++}";

        if (view is not ScriptEditText)
        {
            args["bg"] ??= DefaultValue.BackgroundColor;
        }

        if (view is TextView)
        {
            args["textColor"] ??= DefaultValue.TextColor;
            args["textSize"] ??= DefaultValue.TextSize;
        }
    }
}
