using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using astator.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace astator.Core.UI.Widget
{
    public class ScriptSpinner : AppCompatSpinner, IScriptView
    {
        public class SpinnerAdapter<T> : ArrayAdapter<T>
        {
            public Color TextColor { get; set; } = Color.ParseColor(DefaultValue.TextColor);
            public Color BackgroundColor { get; set; } = Color.ParseColor(DefaultValue.BackgroundColor);
            public float TextSize { get; set; } = DefaultValue.TextSize;
            public Typeface Typeface { get; set; }
            public SpinnerAdapter(Android.Content.Context context, int textViewResourceId, List<T> objects) : base(context, textViewResourceId, objects)
            {
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var view = (TextView)base.GetView(position, convertView, parent);
                view.SetTextColor(this.TextColor);
                view.SetBackgroundColor(this.BackgroundColor);
                view.TextSize = this.TextSize;

                if (this.Typeface is not null)
                {
                    view.Typeface = this.Typeface;
                }

                return view;
            }


            public override View GetDropDownView(int position, View convertView, ViewGroup parent)
            {
                var view = (TextView)base.GetView(position, convertView, parent);
                view.SetTextColor(this.TextColor);
                view.SetBackgroundColor(this.BackgroundColor);
                view.TextSize = this.TextSize;

                if (this.Typeface is not null)
                {
                    view.Typeface = this.Typeface;
                }

                return view;
            }
        }

        public new string Id { get; set; } = string.Empty;
        private OnAttachedListener onAttachedListener;
        private List<string> list = new();
        private int position;
        private Color textColor = Color.ParseColor(DefaultValue.TextColor);
        private Color backgroundColor = Color.ParseColor(DefaultValue.BackgroundColor);
        private float textSize = DefaultValue.TextSize;
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            this.onAttachedListener?.OnAttached(this);
        }
        public ScriptSpinner(Android.Content.Context context, ViewArgs args) : base(context)
        {
            this.LayoutParameters = new MarginLayoutParams(this.LayoutParameters ?? new(LayoutParams.MatchParent, LayoutParams.MatchParent));
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
            if (this.list.Count != 0)
            {
                this.Adapter = new SpinnerAdapter<string>(context, Android.Resource.Layout.SimpleListItemChecked, this.list)
                {
                    TextColor = textColor,
                    BackgroundColor = backgroundColor,
                    TextSize = textSize,
                };
                SetSelection(this.position);
            }
        }
        public void SetAttr(string key, object value)
        {
            switch (key)
            {
                case "position":
                {
                    if (value is string temp)
                        this.position = int.Parse(temp.Trim()); break;
                }
                case "id":
                {
                    if (value is string temp)
                        this.Id = temp.Trim(); break;
                }
                case "entries":
                {
                    if (value is string temp)
                        this.list = temp.Split("|").ToList();
                    break;
                }
                case "textColor":
                {
                    if (value is string temp)
                        this.textColor = Color.ParseColor(temp); break;
                }
                case "textSize":
                {
                    if (value is string temp)
                        this.textSize = int.Parse(temp.Trim()); break;
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
                    SetGravity(Util.EnumParse<GravityFlags>(value));
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
                    {
                        var color = Color.ParseColor(temp);
                        //this.Background = new ColorDrawable(color);
                        this.backgroundColor = color;
                    }
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
                    throw new AttributeNotExistException(key);
            }
        }
        public object GetAttr(string key)
        {
            return key switch
            {
                "position" => this.SelectedItemPosition,
                "w" => this.Width,
                "h" => this.Height,
                "textColor" => ((SpinnerAdapter<string>)this.Adapter)?.TextColor,
                "textSize" => ((SpinnerAdapter<string>)this.Adapter)?.TextSize,
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
                case "selected":
                {
                    if (listener is OnItemSelectedListener temp)
                        this.OnItemSelectedListener = temp;
                    break;
                }
                default:
                    this.OnListener(key, listener);
                    break;
            }
        }
    }
}
