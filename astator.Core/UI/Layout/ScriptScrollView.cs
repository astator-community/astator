﻿using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using astator.Core.Exceptions;
using System;
namespace astator.Core.UI.Layout
{
    public class ScriptScrollView : ScrollView, IScriptView
    {
        public new string Id { get; set; } = string.Empty;
        private OnAttachedListener onAttachedListener;
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            this.onAttachedListener?.OnAttached(this);
        }
        public new ScriptScrollView AddView(View child)
        {
            base.AddView(child);
            return this;

        }

        public ScriptScrollView(Android.Content.Context context, ViewArgs args) : base(context)
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
                case "id":
                    {
                        if (value is string temp)
                            this.Id = temp.Trim(); break;
                    }
                case "w":
                    {
                        var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
                        lp.Width = Utils.DpParse(value);
                        this.LayoutParameters = lp;
                        break;
                    }
                case "h":
                    {
                        var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
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
                        var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(LayoutParams.MatchParent, LayoutParams.MatchParent));
                        lp.SetMargins(margin[0], margin[1], margin[2], margin[3]);
                        this.LayoutParameters = lp;
                        break;
                    }
                case "layoutGravity":
                    {
                        var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
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
                    var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
                    return lp.Width;
                }),
                "h" => new Func<object>(() =>
                {
                    var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
                    return lp.Height;
                }),
                "margin" => new Func<object>(() =>
                {
                    var margin = new int[4];
                    var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
                    margin[0] = lp.LeftMargin;
                    margin[1] = lp.TopMargin;
                    margin[2] = lp.RightMargin;
                    margin[3] = lp.BottomMargin;
                    return margin;
                }),
                "layoutGravity" => new Func<object>(() =>
                {
                    var lp = this.LayoutParameters as FrameLayout.LayoutParams ?? new(this.LayoutParameters as MarginLayoutParams ?? new(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
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
