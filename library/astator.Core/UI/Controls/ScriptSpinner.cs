using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using astator.Core.UI.Base;
using System;
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

    public string CustomId { get; set; }
    public OnCreatedListener OnCreatedListener { get; set; }

    private string entries;
    private int position;
    private Color textColor = DefaultTheme.TextColor;
    private Color backgroundColor = DefaultTheme.LayoutBackgroundColor;
    private float textSize = DefaultTheme.TextSize;

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        // SetSelection(this.position);
    }

    public ScriptSpinner(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.LayoutParameters = new MarginLayoutParams(this.LayoutParameters ?? new(LayoutParams.MatchParent, LayoutParams.MatchParent));

        this.SetDefaultValue(ref args);
        if (args["textColor"] is not null)
        {
            if (args["textColor"] is string temp) this.textColor = Color.ParseColor(temp);
            if (args["textColor"] is Color color) this.textColor = color;
        }

        if (args["textSize"] is not null)
        {
            if (args["textSize"] is string temp) this.textSize = int.Parse(temp);
            if (args["textSize"] is int i32) this.textSize = i32;
        }

        if (args["bg"] is not null)
        {
            if (args["bg"] is string temp) this.backgroundColor = Color.ParseColor(temp);
            if (args["bg"] is Color color) this.backgroundColor = color;
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
                this.position = Convert.ToInt32(value);
                if (this.Adapter is not null) SetSelection(this.position);
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
                    SetSelection(this.position);
                }
                break;
            }
            case "textColor":
            {
                if (value is string temp) this.textColor = Color.ParseColor(temp);
                else if (value is Color color) this.textColor = color;
                break;
            }
            case "textSize":
            {
                if (value is string temp) this.textSize = int.Parse(temp);
                else if (value is int i32) this.textSize = i32;
                break;
            }
            case "gravity":
            {
                SetGravity(Util.EnumParse<GravityFlags>(value));
                break;
            }
            case "bg":
            {
                if (value is string temp) this.backgroundColor = Color.ParseColor(temp);
                else if (value is Color color) this.backgroundColor = color;
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
