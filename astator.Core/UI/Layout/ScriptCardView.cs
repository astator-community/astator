using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using System;
namespace astator.Core.UI.Layout
{
    public class ScriptCardView : CardView
    {
        public new string Id { get; set; } = string.Empty;
        private OnAttachedListener onAttachedListener;
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            this.onAttachedListener?.OnAttached(this);
        }
        public new ScriptCardView AddView(View child)
        {
            base.AddView(child);
            return this;
        }
        public ScriptCardView(Android.Content.Context context, UiArgs args) : base(context)
        {
            if (args is null)
            {
                return;
            }
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
                case "radius":
                    {
                        if (value is string temp)
                            this.Radius = float.Parse(temp.Trim());
                        break;
                    }
                case "elevation":
                    {
                        if (value is string temp)
                            this.CardElevation = float.Parse(temp.Trim()); break;
                    }
                case "maxElevation":
                    {
                        if (value is string temp)
                            this.MaxCardElevation = float.Parse(temp.Trim()); break;
                    }
                case "id":
                    {
                        if (value is string temp)
                            this.Id = temp.Trim(); break;
                    }
                case "w":
                    {
                        LayoutParams lp = new(this.LayoutParameters ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                        lp.Width = Util.DpParse(value);
                        this.LayoutParameters = lp;
                        break;
                    }
                case "h":
                    {
                        LayoutParams lp = new(this.LayoutParameters ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                        lp.Height = Util.DpParse(value);
                        this.LayoutParameters = lp;
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
                        var lp = this.LayoutParameters as LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                        lp.SetMargins(margin[0], margin[1], margin[2], margin[3]);
                        this.LayoutParameters = lp;
                        break;
                    }
                case "layoutGravity":
                    {
                        var lp = this.LayoutParameters as LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
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
                        SetContentPadding(padding[0], padding[1], padding[2], padding[3]);
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
                            this.CardBackgroundColor = ColorStateList.ValueOf(Color.ParseColor(temp));
                        break;
                    }
                case "fg":
                    {
                        if (value is string temp)
                            this.Foreground = new ColorDrawable(Color.ParseColor(temp));
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
                "w" => new Func<object>(() =>
                {
                    var lp = this.LayoutParameters as LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                    return lp.Width;
                }),
                "h" => new Func<object>(() =>
                {
                    var lp = this.LayoutParameters as LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                    return lp.Height;
                }),
                "margin" => new Func<object>(() =>
                {
                    var margin = new int[4];
                    var lp = this.LayoutParameters as LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                    margin[0] = lp.LeftMargin;
                    margin[1] = lp.TopMargin;
                    margin[2] = lp.RightMargin;
                    margin[3] = lp.BottomMargin;
                    return margin;
                }),
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
        public void On(string key, object callback)
        {
            switch (key)
            {
                case "attached":
                    {
                        if (callback is OnAttachedListener temp)
                            this.onAttachedListener = temp;
                        break;
                    }
                default:
                    this.LayoutOnListener(key, callback);
                    break;
            }
        }
    }
}
