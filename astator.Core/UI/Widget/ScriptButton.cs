using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text.Util;
using Android.Views;
using Android.Widget;
using System;
using static Android.Text.TextUtils;
using static Android.Views.ViewGroup;
namespace astator.Core.UI.Widget
{
    public class ScriptButton : Button
    {
        public new string Id { get; set; } = string.Empty;
        private OnAttachedListener onAttachedListener;
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            this.onAttachedListener?.OnAttached(this);
        }
        public ScriptButton(Android.Content.Context context, UiArgs args) : base(context)
        {
            this.LayoutParameters = new MarginLayoutParams(this.LayoutParameters ?? new(LayoutParams.WrapContent, LayoutParams.WrapContent));
            if (args is null)
            {
                return;
            }
            if (args["textColor"] is null) args["textColor"] = DefaultValue.TextColor;
            if (args["textSize"] is null) args["textSize"] = DefaultValue.TextSize;
            if (args["id"] is null)
            {
                this.Id = $"{ GetType().Name }-{ UiManager.CreateCount }";
                UiManager.CreateCount++;
            }
            foreach (var item in args)
            {
                var key = item.Key.ToString();
                if (key != "length")
                {
                    SetAttr(key, item.Value);
                }
            }
        }
        public void SetAttr(string key, object value)
        {
            switch (key)
            {
                case "id":
                    {
                        if (value is string temp)
                            this.Id = temp.Trim();
                        break;
                    }
                case "text":
                    {
                        this.Text = value.ToString();
                        break;
                    }
                case "textSize":
                    {
                        SetTextSize(Android.Util.ComplexUnitType.Dip, Util.DpParse(value));
                        break;
                    }
                case "textColor":
                    {
                        if (value is string temp)
                            SetTextColor(Color.ParseColor(temp));
                        break;
                    }
                case "lines":
                    {
                        SetLines(Util.DpParse(value));
                        break;
                    }
                case "maxLines":
                    {
                        SetMaxLines(Util.DpParse(value));
                        break;
                    }
                case "typeface":
                    {
                        var tf = Util.TypeParse<Typeface>(value);
                        var style = this.Typeface?.Style ?? tf.Style;
                        SetTypeface(tf, style);
                        break;
                    }
                case "textStyle":
                    {
                        SetTypeface(this.Typeface, Util.EnumParse<TypefaceStyle>(value));
                        break;
                    }
                case "ems":
                    {
                        SetEms(Util.DpParse(value));
                        break;
                    }
                case "autoLink":
                    {
                        if (value is string temp)
                        {
                            var mode = temp.Trim();
                            if (mode == "web")
                            {
                                this.AutoLinkMask = MatchOptions.WebUrls;
                            }
                            else if (mode == "email")
                            {
                                this.AutoLinkMask = MatchOptions.EmailAddresses;
                            }
                            else if (mode == "phone")
                            {
                                this.AutoLinkMask = MatchOptions.PhoneNumbers;
                            }
                            else if (mode == "map")
                            {
                                this.AutoLinkMask = MatchOptions.MapAddresses;
                            }
                            else if (mode == "all")
                            {
                                this.AutoLinkMask = MatchOptions.All;
                            }
                        }
                        break;
                    }
                case "ellipsize":
                    {
                        if (value is string temp)
                            this.Ellipsize = TruncateAt.ValueOf(temp.Trim());
                        break;
                    }
                case "click":
                    {
                        if (value is OnClickListener temp)
                            SetOnClickListener(temp);
                        break;
                    }
                case "w":
                    {
                        SetWidth(Util.DpParse(value));
                        break;
                    }
                case "h":
                    {
                        SetHeight(Util.DpParse(value));
                        break;
                    }
                case "minWidth":
                    {
                        SetMinWidth(Util.DpParse(value));
                        break;
                    }
                case "minHeight":
                    {
                        SetMinHeight(Util.DpParse(value));
                        break;
                    }
                case "margin":
                    {
                        var margin = new int[4];
                        if (value is int i32)
                        {
                            margin[0] = margin[1] = margin[2] = margin[3] = i32;
                        }
                        else if (value is int[] arr)
                        {
                            margin[0] = Util.DpParse(arr[0]);
                            margin[1] = Util.DpParse(arr[1]);
                            margin[2] = Util.DpParse(arr[2]);
                            margin[3] = Util.DpParse(arr[3]);
                        }
                        else if (value is string str)
                        {
                            var strArr = str.Split(",");
                            if (strArr.Length == 1)
                            {
                                var temp = Util.DpParse(strArr[0]);
                                margin[0] = margin[1] = margin[2] = margin[3] = temp;
                            }
                            else if (strArr.Length == 2)
                            {
                                margin[0] = margin[2] = Util.DpParse(strArr[0]);
                                margin[1] = margin[3] = Util.DpParse(strArr[1]);
                            }
                            else if (strArr.Length == 4)
                            {
                                margin[0] = Util.DpParse(strArr[0]);
                                margin[1] = Util.DpParse(strArr[1]);
                                margin[2] = Util.DpParse(strArr[2]);
                                margin[3] = Util.DpParse(strArr[3]);
                            }
                        }
                        var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                        lp.SetMargins(margin[0], margin[1], margin[2], margin[3]);
                        this.LayoutParameters = lp;
                        break;
                    }
                case "gravity":
                    {
                        this.Gravity = Util.EnumParse<GravityFlags>(value);
                        break;
                    }
                case "layoutGravity":
                    {
                        var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                        lp.Gravity = Util.EnumParse<GravityFlags>(value);
                        this.LayoutParameters = lp;
                        break;
                    }
                case "padding":
                    {
                        var padding = new int[4];
                        if (value is int i32)
                        {
                            padding[0] = padding[1] = padding[2] = padding[3] = i32;
                        }
                        else if (value is int[] arr)
                        {
                            padding[0] = Util.DpParse(arr[0]);
                            padding[1] = Util.DpParse(arr[1]);
                            padding[2] = Util.DpParse(arr[2]);
                            padding[3] = Util.DpParse(arr[3]);
                        }
                        else if (value is string str)
                        {
                            var strArr = str.Split(",");
                            if (strArr.Length == 1)
                            {
                                var temp = Util.DpParse(strArr[0]);
                                padding[0] = padding[1] = padding[2] = padding[3] = temp;
                            }
                            else if (strArr.Length == 2)
                            {
                                padding[0] = padding[2] = Util.DpParse(strArr[0]);
                                padding[1] = padding[3] = Util.DpParse(strArr[1]);
                            }
                            else if (strArr.Length == 4)
                            {
                                padding[0] = Util.DpParse(strArr[0]);
                                padding[1] = Util.DpParse(strArr[1]);
                                padding[2] = Util.DpParse(strArr[2]);
                                padding[3] = Util.DpParse(strArr[3]);
                            }
                        }
                        SetPadding(padding[0], padding[1], padding[2], padding[3]);
                        break;
                    }
                case "alpha":
                    {
                        if (value is string temp)
                            this.Alpha = float.Parse(temp.Trim());
                        break;
                    }
                case "bg":
                    {
                        if (value is string temp)
                            this.Background = new ColorDrawable(Color.ParseColor(temp.Trim()));
                        break;
                    }
                case "fg":
                    {
                        if (value is string temp)
                            this.Foreground = new ColorDrawable(Color.ParseColor(temp.Trim()));
                        break;
                    }
                case "radius":
                    {
                        var drawable = new GradientDrawable();
                        try
                        {
                            if (this.Background is not null)
                                drawable.SetColor(((ColorDrawable)this.Background).Color);
                        }
                        catch
                        {
                            drawable.SetColor(Color.ParseColor("#d8d8d8"));
                        }
                        drawable.SetCornerRadius(Util.DpParse(value));
                        this.Background = drawable;
                        break;
                    }
                case "visibility":
                    {
                        this.Visibility = Util.EnumParse<ViewStates>(value);
                        break;
                    }
                case "rotation":
                    {
                        if (value is string temp)
                            this.Rotation = float.Parse(temp.Trim());
                        break;
                    }
                case "transformPivotX":
                    {
                        if (value is string temp)
                            this.TranslationX = float.Parse(temp.Trim());
                        break;
                    }
                case "transformPivotY":
                    {
                        if (value is string temp)
                            this.TranslationY = float.Parse(temp.Trim());
                        break;
                    }
                default:
                    throw new ArgumentException(key + ": 未定义属性!");
            }
        }
        public object GetAttr(string key)
        {
            return key switch
            {
                "text" => this.Text,
                "textSize" => this.TextSize,
                "textColor" => this.CurrentTextColor,
                "maxLines" => this.MaxLines,
                "typeface" => this.Typeface,
                "autoLink" => this.AutoLinkMask,
                "ellipsize" => this.Ellipsize,
                "w" => this.Width,
                "h" => this.Height,
                "minWidth" => this.MinWidth,
                "minHeight" => this.MinHeight,
                "margin" => new Func<object>(() =>
                {
                    var margin = new int[4];
                    var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                    margin[0] = lp.LeftMargin;
                    margin[1] = lp.TopMargin;
                    margin[2] = lp.RightMargin;
                    margin[3] = lp.BottomMargin;
                    return margin;
                }),
                "gravity" => this.Gravity,
                "layoutGravity" => new Func<object>(() =>
                {
                    var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                    return lp.Gravity;
                }),
                "padding" => new int[] { this.PaddingLeft, this.PaddingTop, this.PaddingRight, this.PaddingBottom },
                "alpha" => this.Alpha,
                "bg" => this.Background,
                "fg" => this.Foreground,
                "visibility" => this.Visibility,
                "rotation" => this.Rotation,
                "translationX" => this.TranslationX,
                "translationY" => this.TranslationY,
                _ => throw new ArgumentException(key + ": 未定义属性!")
            };
        }
        public void On(string key, object listener)
        {
            switch (key)
            {
                case "attached":
                    {
                        if (listener is OnAttachedListener temp)
                            this.onAttachedListener = temp;
                        break;
                    }
                default:
                    this.OnListener(key, listener);
                    break;
            }
        }
    }
}
