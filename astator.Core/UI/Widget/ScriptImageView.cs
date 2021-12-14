using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using astator.Core.Exceptions;
using System;
using System.IO;
using static Android.Views.ViewGroup;
using Path = Android.Graphics.Path;

namespace astator.Core.UI.Widget
{
    public class ScriptImageView : ImageView, IScriptView
    {
        public new string Id { get; set; } = string.Empty;
        private OnAttachedListener onAttachedListener;
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            this.onAttachedListener?.OnAttached(this);
        }
        public float[] Radius { get; set; } = new float[8];
        private readonly string _workingDirectory;
        protected override void OnDraw(Canvas canvas)
        {
            var path = new Path();
            path.AddRoundRect(new RectF(0, 0, this.Width, this.Height), this.Radius, Path.Direction.Cw);
            canvas.ClipPath(path);
            base.OnDraw(canvas);
        }
        public ScriptImageView(Android.Content.Context context, string workingDirectory, ViewArgs args) : base(context)
        {
            this.LayoutParameters = new MarginLayoutParams(this.LayoutParameters ?? new(LayoutParams.WrapContent, LayoutParams.WrapContent));
            if (args is null)
            {
                return;
            }
            this._workingDirectory = workingDirectory;
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
                            this.Id = temp.Trim(); break;
                    }
                case "src":
                    {
                        if (value is string temp)
                        {
                            var path = temp;
                            if (!path.StartsWith("/"))
                            {
                                path = System.IO.Path.Combine(this._workingDirectory, path);
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
                case "scaleType":
                    {
                        SetScaleType(Utils.TypeParse<ScaleType>(value));
                        break;
                    }
                case "click":
                    {
                        SetOnClickListener((OnClickListener)value);
                        break;
                    }
                case "w":
                    {
                        var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                        lp.Width = Utils.DpParse(value);
                        this.LayoutParameters = lp;
                        break;
                    }
                case "h":
                    {
                        var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                        lp.Height = Utils.DpParse(value);
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
                            margin[0] = Utils.DpParse(arr[0]);
                            margin[1] = Utils.DpParse(arr[1]);
                            margin[2] = Utils.DpParse(arr[2]);
                            margin[3] = Utils.DpParse(arr[3]);
                        }
                        else if (value is string str)
                        {
                            var strArr = str.Split(",");
                            if (strArr.Length == 1)
                            {
                                var temp = Utils.DpParse(strArr[0]);
                                margin[0] = margin[1] = margin[2] = margin[3] = temp;
                            }
                            else if (strArr.Length == 2)
                            {
                                margin[0] = margin[2] = Utils.DpParse(strArr[0]);
                                margin[1] = margin[3] = Utils.DpParse(strArr[1]);
                            }
                            else if (strArr.Length == 4)
                            {
                                margin[0] = Utils.DpParse(strArr[0]);
                                margin[1] = Utils.DpParse(strArr[1]);
                                margin[2] = Utils.DpParse(strArr[2]);
                                margin[3] = Utils.DpParse(strArr[3]);
                            }
                        }
                        var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                        lp.SetMargins(margin[0], margin[1], margin[2], margin[3]);
                        this.LayoutParameters = lp;
                        break;
                    }
                case "layoutGravity":
                    {
                        var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                        lp.Gravity = Utils.EnumParse<GravityFlags>(value);
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
                            padding[0] = Utils.DpParse(arr[0]);
                            padding[1] = Utils.DpParse(arr[1]);
                            padding[2] = Utils.DpParse(arr[2]);
                            padding[3] = Utils.DpParse(arr[3]);
                        }
                        else if (value is string str)
                        {
                            var strArr = str.Split(",");
                            if (strArr.Length == 1)
                            {
                                var temp = Utils.DpParse(strArr[0]);
                                padding[0] = padding[1] = padding[2] = padding[3] = temp;
                            }
                            else if (strArr.Length == 2)
                            {
                                padding[0] = padding[2] = Utils.DpParse(strArr[0]);
                                padding[1] = padding[3] = Utils.DpParse(strArr[1]);
                            }
                            else if (strArr.Length == 4)
                            {
                                padding[0] = Utils.DpParse(strArr[0]);
                                padding[1] = Utils.DpParse(strArr[1]);
                                padding[2] = Utils.DpParse(strArr[2]);
                                padding[3] = Utils.DpParse(strArr[3]);
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
                            this.Background = new ColorDrawable(Color.ParseColor(temp));
                        break;
                    }
                case "fg":
                    {
                        if (value is string temp)
                            this.Foreground = new ColorDrawable(Color.ParseColor(temp));
                        break;
                    }
                case "radius":
                    {
                        var radius = new float[8];
                        if (value is int i32)
                        {
                            radius[0] = radius[1] = radius[2] = radius[3] = radius[4] = radius[5] = radius[6] = radius[7] = i32;
                        }
                        else if (value is int[] arr)
                        {
                            radius[0] = Utils.DpParse(arr[0]);
                            radius[1] = Utils.DpParse(arr[1]);
                            radius[2] = Utils.DpParse(arr[2]);
                            radius[3] = Utils.DpParse(arr[3]);
                            radius[4] = Utils.DpParse(arr[4]);
                            radius[5] = Utils.DpParse(arr[5]);
                            radius[6] = Utils.DpParse(arr[6]);
                            radius[7] = Utils.DpParse(arr[7]);
                        }
                        else if (value is string str)
                        {
                            var strArr = str.Split(",");
                            if (strArr.Length == 1)
                            {
                                var temp = Utils.DpParse(strArr[0]);
                                radius[0] = radius[1] = radius[2] = radius[3] = radius[4] = radius[5] = radius[6] = radius[7] = temp;
                            }
                            else if (strArr.Length == 4)
                            {
                                radius[0] = radius[1] = Utils.DpParse(strArr[0]);
                                radius[2] = radius[3] = Utils.DpParse(strArr[1]);
                                radius[4] = radius[5] = Utils.DpParse(strArr[2]);
                                radius[6] = radius[7] = Utils.DpParse(strArr[3]);
                            }
                            else if (strArr.Length == 8)
                            {
                                radius[0] = Utils.DpParse(strArr[0]);
                                radius[1] = Utils.DpParse(strArr[1]);
                                radius[2] = Utils.DpParse(strArr[2]);
                                radius[3] = Utils.DpParse(strArr[3]);
                                radius[4] = Utils.DpParse(strArr[4]);
                                radius[5] = Utils.DpParse(strArr[5]);
                                radius[6] = Utils.DpParse(strArr[6]);
                                radius[7] = Utils.DpParse(strArr[7]);
                            }
                        }
                        radius.CopyTo(this.Radius, 0);
                        break;
                    }
                case "visibility":
                    {
                        this.Visibility = Utils.EnumParse<ViewStates>(value);
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
                    throw new AttributeNotExistException(key);
            }
        }
        public object GetAttr(string key)
        {
            return key switch
            {
                "w" => new Func<object>(() =>
                {
                    var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                    return lp.Width;
                }),
                "h" => new Func<object>(() =>
                {
                    var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                    return lp.Height;
                }),
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
                "layoutGravity" => new Func<object>(() =>
                {
                    var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                    return lp.Gravity;
                }),
                "padding" => new int[] { this.PaddingLeft, this.PaddingTop, this.PaddingRight, this.PaddingBottom },
                "alpha" => this.Alpha,
                "bg" => this.Background,
                "fg" => this.Foreground,
                "radius" => this.Radius,
                "visibility" => this.Visibility,
                "rotation" => this.Rotation,
                "translationX" => this.TranslationX,
                "translationY" => this.TranslationY,
                _ => throw new AttributeNotExistException(key)
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
