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
    private readonly bool accentShow = true;
    private readonly Color navBackgroundColor = DefaultValue.BackgroundColor;
    private readonly Color titleColor = DefaultValue.TitleColor;
    private readonly Color accentColor = DefaultValue.AccentColor;
    private readonly int accentWidth = 14;
    private readonly string workDir;

    private readonly int height = 60;

    private readonly List<(ScriptImageView view, string icon, string enableIcon)> icons = new();
    private readonly List<ScriptCardView> accents = new();

    public ScriptTabbedPage(Context context, string workDir, ViewArgs args) : base(context)
    {
        this.SetDefaultValue(ref args);

        if (args["isBottom"] is not null) this.isBottom = Convert.ToBoolean(args["isBottom"]);
        if (args["iconShow"] is not null) this.iconShow = Convert.ToBoolean(args["iconShow"]);
        if (args["titleShow"] is not null) this.titleShow = Convert.ToBoolean(args["titleShow"]);
        if (args["accentShow"] is not null) this.accentShow = Convert.ToBoolean(args["accentShow"]);
        if (args["navBg"] is not null) this.navBackgroundColor = Color.ParseColor(args["navBg"].ToString());
        if (args["titleColor"] is not null) this.titleColor = Color.ParseColor(args["titleColor"].ToString());
        if (args["accentColor"] is not null) this.accentColor = Color.ParseColor(args["accentColor"].ToString());
        if (args["accentWidth"] is not null) this.accentWidth = Convert.ToInt32(args["accentWidth"]);

        if (!this.iconShow) this.height -= 28;
        if (!this.titleShow) this.height -= 10;

        this.viewPager = new ScriptViewPager(context, null);

        this.viewPager.On("pageChange", new OnPageChangeListener((position) =>
        {
            for (var i = 0; i < this.NavLayout.ChildCount; i++)
            {
                if (i == position)
                {
                    if (this.iconShow) this.icons[i].view.SetAttr("src", this.icons[i].enableIcon);
                    if (this.accentShow) this.accents[i].Visibility = ViewStates.Visible;
                }
                else
                {
                    if (this.iconShow) this.icons[i].view.SetAttr("src", this.icons[i].icon);
                    if (this.accentShow) this.accents[i].Visibility = ViewStates.Invisible;
                }
            }
        }));

        this.NavLayout = new ScriptLinearLayout(context, new ViewArgs
        {
            ["h"] = height,
            ["bg"] = this.navBackgroundColor,
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

        args.Remove("isBottom", "iconShow", "titleShow", "accentShow", "navBg", "titleColor", "accentColor", "accentWidth");
        foreach (var item in args)
        {
            SetAttr(item.Key.ToString(), item.Value);
        }
    }

    public new ILayout AddView(View view)
    {
        if (view is ScriptTabbedView tabView)
        {
            var context = tabView.Context;

            var navView = new ScriptFrameLayout(context, new ViewArgs
            {
                ["tag"] = this.NavLayout.ChildCount,
                ["h"] = height,
                ["weight"] = 1,
                ["radius"] = 2,
                ["bg"] = Color.Transparent
            });

            if (this.iconShow)
            {
                var icon = new ScriptImageView(context, this.workDir, new ViewArgs
                {
                    ["h"] = this.titleShow ? 28 : 32,
                    ["margin"] = new int[] { 0, this.titleShow ? 6 : 0, 0, 0 },
                    ["scaleType"] = "fitCenter",
                    ["src"] = this.NavLayout.ChildCount == 0 ? tabView.EnableIcon : tabView.Icon,
                    ["layoutGravity"] = this.titleShow ? "top|center" : "center",
                });
                navView.AddView(icon);
                this.icons.Add((icon, tabView.Icon, tabView.EnableIcon));
            }

            if (this.titleShow)
            {
                navView.AddView(new ScriptTextView(context, new ViewArgs
                {
                    ["margin"] = "0,0,0,6",
                    ["text"] = tabView.Title,
                    ["textSize"] = 14,
                    ["textColor"] = this.titleColor,
                    ["textStyle"] = "bold",
                    ["gravity"] = "center",
                    ["layoutGravity"] = "center|bottom",
                }));
            }

            if (this.accentShow)
            {
                var accent = new ScriptCardView(context, new ViewArgs
                {
                    ["margin"] = "0,0,0,1",
                    ["w"] = this.accentWidth,
                    ["h"] = 3,
                    ["radius"] = 1.5,
                    ["bg"] = this.accentColor,
                    ["visibility"] = this.NavLayout.ChildCount == 0 ? "visible" : "invisible",
                    ["layoutGravity"] = "center|bottom",
                });
                navView.AddView(accent);
                this.accents.Add(accent);
            }


            navView.On("touch", new OnTouchListener((v, e) =>
            {
                if (e.Action == MotionEventActions.Down)
                {
                    navView.SetBackgroundColor(DefaultValue.HintColor);
                }
                else
                {
                    navView.SetBackgroundColor(Color.Transparent);
                    this.viewPager.SetCurrentItem(Convert.ToInt32(v.Tag), true);
                }
                return true;
            }));

            this.viewPager.AddView(tabView);
            this.NavLayout.AddView(navView);
        }
        return this;
    }

    public void SetAttr(string key, object value)
    {
        Util.SetAttr(this, key, value);
    }

    public object GetAttr(string key)
    {
        return Util.GetAttr(this, key);
    }

    public void On(string key, object listener)
    {
        Util.OnListener(this, key, listener);
    }

}
