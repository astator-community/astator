using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using astator.Core.UI.Base;
using astator.Core.UI.Controls;
using System;
using System.Collections.Generic;

namespace astator.Core.UI.Layouts;
public class ScriptTabbedPage : FrameLayout, ILayout
{
    public string CustomId { get; set; }
    public OnAttachedListener OnAttachedListener { get; set; }

    private readonly ScriptViewPager viewPager;
    private readonly ScriptLinearLayout NavLayout;

    private readonly bool isBottom = true;
    private readonly bool iconShow = true;
    private readonly bool titleShow = true;
    private readonly Color backgroundColor = DefaultValue.BackgroundColor;
    private readonly Color titleColor = DefaultValue.TitleColor;
    private readonly Color accentColor = DefaultValue.AccentColor;
    private readonly string workDir;

    private readonly int height = 60;
    private List<ScriptCardView> accents = new();

    public ScriptTabbedPage(Context context, string workDir, ViewArgs args) : base(context)
    {
        this.SetCustomId(ref args);

        if (args["isBottom"] is not null) this.isBottom = Convert.ToBoolean(args["isBottom"]);
        if (args["bg"] is not null) this.backgroundColor = Color.ParseColor(args["bg"].ToString());
        if (args["titleColor"] is not null) this.titleColor = Color.ParseColor(args["titleColor"].ToString());
        if (args["accentColor"] is not null) this.accentColor = Color.ParseColor(args["accentColor"].ToString());
        if (args["iconShow"] is not null) this.iconShow = Convert.ToBoolean(args["iconShow"]);
        if (args["titleShow"] is not null) this.titleShow = Convert.ToBoolean(args["titleShow"]);

        if (!this.iconShow) this.height -= 28;
        if (!this.titleShow) this.height -= 20;

        this.viewPager = new ScriptViewPager(context, null);

        this.viewPager.On("pageChange", new OnPageChangeListener((position) =>
        {
            for (var i = 0; i < this.accents.Count; i++)
            {
                if (i == position)
                    this.accents[i].Visibility = ViewStates.Visible;
                else
                    this.accents[i].Visibility = ViewStates.Invisible;
            }
        }));

        this.NavLayout = new ScriptLinearLayout(context, new ViewArgs
        {
            ["h"] = height,
            ["layoutGravity"] = this.isBottom ? "bottom" : "top",
        });

        if (this.isBottom)
        {
            this.viewPager.SetAttr("margin", new int[] { 0, 0, 0, this.height });
            base.AddView(this.viewPager);
            base.AddView(this.NavLayout);
        }
        else
        {
            this.viewPager.SetAttr("margin", new int[] { 0, this.height, 0, 0 });
            base.AddView(this.NavLayout);
            base.AddView(this.viewPager);
        }

        this.workDir = workDir;
    }

    public new ILayout AddView(View view)
    {
        if (view is ScriptTabbedView tabView)
        {
            var context = tabView.Context;
            this.viewPager.AddView(tabView);

            var navView = new ScriptFrameLayout(context, new ViewArgs
            {
                ["tag"] = this.accents.Count,
                ["h"] = height,
                ["weight"] = 1,
                ["radius"] = 5,
            });

            if (this.iconShow)
            {
                navView.AddView(new ScriptImageView(context, this.workDir, new ViewArgs
                {
                    ["h"] = 28,
                    ["margin"] = "0,4,0,0",
                    ["scaleType"] = "fitCenter",
                    ["src"] = tabView.Icon,
                    ["layoutGravity"] = "top|center",
                }));
            }

            if (this.titleShow)
            {
                navView.AddView(new ScriptTextView(context, new ViewArgs
                {
                    ["margin"] = "0,0,0,10",
                    ["text"] = tabView.Title,
                    ["textSize"] = 14,
                    ["textColor"] = this.titleColor,
                    ["textStyle"] = "bold",
                    ["gravity"] = "center",
                    ["layoutGravity"] = "center|bottom",
                }));
            }

            var hint = new ScriptCardView(context, new ViewArgs
            {
                ["margin"] = "0,0,0,2",
                ["w"] = 14,
                ["h"] = 4,
                ["radius"] = 2,
                ["bg"] = this.accentColor,
                ["visibility"] = this.accents.Count == 0 ? "visible" : "invisible",
                ["layoutGravity"] = "center|bottom",
            });
            navView.AddView(hint);
            this.accents.Add(hint);

            navView.On("touch", new OnTouchListener((v, e) =>
            {
                if (e.Action == MotionEventActions.Down)
                {
                    navView.SetBackgroundColor(DefaultValue.HintColor);
                }
                else
                {
                    navView.SetBackgroundColor(this.backgroundColor);
                    this.viewPager.SetCurrentItem(Convert.ToInt32(v.Tag), true);
                }
                return true;
            }));

            this.NavLayout.AddView(navView);
        }
        return this;
    }

    public object GetAttr(string key)
    {
        throw new NotImplementedException();
    }

    public void On(string key, object listener)
    {
        throw new NotImplementedException();
    }

    public void SetAttr(string key, object value)
    {
        throw new NotImplementedException();
    }
}
