﻿using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using astator.Core.UI.Base;
using System.Collections.Generic;
using System.Linq;

namespace astator.Core.UI.Controls;

public class ScriptSpinner : AppCompatSpinner, IControl
{
    public class SpinnerAdapter<T> : ArrayAdapter<T>
    {
        public Color TextColor { get; set; }
        public Color BackgroundColor { get; set; }
        public float TextSize { get; set; }
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

    public string CustomId { get; set; } = string.Empty;
    public OnAttachedListener OnAttachedListener { get; set; }

    private string entries;
    private int position;
    private Color textColor = DefaultValue.TextColor;
    private Color backgroundColor = DefaultValue.BackgroundColor;
    private float textSize = DefaultValue.TextSize;
    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();

        SetSelection(this.position);
        this.OnAttachedListener?.OnAttached(this);
    }
    public ScriptSpinner(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.LayoutParameters = new MarginLayoutParams(this.LayoutParameters ?? new(LayoutParams.MatchParent, LayoutParams.MatchParent));
        if (args is null)
        {
            return;
        }
        if (args["id"] is null)
        {
            this.CustomId = $"{ GetType().Name }-{ UiManager.CreateCount }";
            UiManager.CreateCount++;
        }


        if (args["textColor"] is not null)
        {
            if (args["textColor"] is string temp)
            {
                this.textColor = Color.ParseColor(temp);
            }
        }

        if (args["textSize"] is not null)
        {
            if (args["textSize"] is string temp)
            {
                this.textSize = int.Parse(temp.Trim());
            }
        }

        if (args["bg"] is not null)
        {
            if (args["bg"] is string temp)
            {
                this.backgroundColor = Color.ParseColor(temp);
            }
        }

        if (args["textSize"] is null)
        {
            args["textSize"] = DefaultValue.TextSize;
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
            case "position":
            {
                if (value is string temp)
                {
                    this.position = int.Parse(temp.Trim());
                }

                break;
            }
            case "entries":
            {
                if (value is string temp)
                {
                    this.entries = temp;
                    var list = temp.Split("|").ToList();
                    this.Adapter = new SpinnerAdapter<string>(this.Context, Android.Resource.Layout.SimpleSpinnerDropDownItem, list)
                    {
                        TextColor = textColor,
                        BackgroundColor = backgroundColor,
                        TextSize = textSize,
                    };
                }
                break;
            }
            case "textColor":
            {
                if (value is string temp)
                {
                    this.textColor = Color.ParseColor(temp);
                }

                break;
            }
            case "textSize":
            {
                if (value is string temp)
                {
                    this.textSize = int.Parse(temp.Trim());
                }

                break;
            }
            case "gravity":
            {
                SetGravity(Util.EnumParse<GravityFlags>(value));
                break;
            }
            case "bg":
            {
                if (value is string temp)
                {
                    var color = Color.ParseColor(temp);
                    this.backgroundColor = color;
                }
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
            "position" => this.SelectedItemPosition,
            "textColor" => this.textColor,
            "en" => this.entries,
            "textSize" => this.textSize,
            "gravity" => this.Gravity,
            "bg" => this.backgroundColor,
            _ => Util.GetAttr(this, key)
        };
    }

    public void On(string key, object listener)
    {
        switch (key)
        {
            case "selected":
            {
                if (listener is OnItemSelectedListener temp)
                {
                    this.OnItemSelectedListener = temp;
                }

                break;
            }
            default:
                this.OnListener(key, listener);
                break;
        }
    }
}