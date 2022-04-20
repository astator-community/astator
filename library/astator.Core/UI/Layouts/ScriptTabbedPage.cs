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
    public OnCreatedListener OnCreatedListener { get; set; }

    private readonly ScriptViewPager viewPager;
    private readonly ScriptLinearLayout tabLayout;

    private readonly bool isBottom = true;
    private readonly bool iconShow = true;
    private readonly bool titleShow = true;
    private readonly bool accentShow = true;
    private readonly Color tabBackgroundColor = DefaultTheme.LayoutBackgroundColor;
    private readonly Color titleColor = Color.ParseColor("#2a2a2d");
    private readonly Color selectedTitleColor = Color.ParseColor("#2a2a2d");
    private readonly Color accentColor = DefaultTheme.ColorPrimary;
    private readonly int accentWidth = 14;
    private readonly string workDir;

    private readonly int height = 60;

    private readonly List<(ScriptImageView view, string icon, string selectedIcon)> icons = new();
    private readonly List<ScriptTextView> titles = new();
    private readonly List<ScriptCardView> accents = new();

    public ScriptTabbedPage(Context context, string workDir, ViewArgs args) : base(context)
    {
        this.SetDefaultValue(ref args);

        if (args["isBottom"] is not null) this.isBottom = Convert.ToBoolean(args["isBottom"]);
        if (args["iconShow"] is not null) this.iconShow = Convert.ToBoolean(args["iconShow"]);
        if (args["titleShow"] is not null) this.titleShow = Convert.ToBoolean(args["titleShow"]);
        if (args["accentShow"] is not null) this.accentShow = Convert.ToBoolean(args["accentShow"]);
        if (args["accentWidth"] is not null) this.accentWidth = Convert.ToInt32(args["accentWidth"]);

        if (args["tabBg"] is not null)
        {
            if (args["tabBg"] is string str) this.tabBackgroundColor = Color.ParseColor(str);
            else if (args["tabBg"] is Color color) this.tabBackgroundColor = color;
        }
        if (args["titleColor"] is not null)
        {
            if (args["titleColor"] is string str) this.titleColor = Color.ParseColor(str);
            else if (args["titleColor"] is Color color) this.titleColor = color;
        }
        if (args["selectedTitleColor"] is not null)
        {
            if (args["selectedTitleColor"] is string str) this.selectedTitleColor = Color.ParseColor(str);
            else if (args["selectedTitleColor"] is Color color) this.selectedTitleColor = color;
        }
        else this.selectedTitleColor = this.titleColor;

        if (args["accentColor"] is not null)
        {
            if (args["accentColor"] is string str) this.accentColor = Color.ParseColor(str);
            else if (args["accentColor"] is Color color) this.accentColor = color;
        }

        if (!this.iconShow) this.height -= 22;
        if (!this.titleShow) this.height -= 10;

        this.viewPager = new ScriptViewPager(context, null);

        this.viewPager.On("pageChange", new OnPageChangeListener((position) =>
        {
            for (var i = 0; i < this.tabLayout.ChildCount; i++)
            {
                if (i == position)
                {
                    if (this.iconShow) this.icons[i].view.SetAttr("src", this.icons[i].selectedIcon);
                    if (this.titleShow) this.titles[i].SetTextColor(this.selectedTitleColor);
                    if (this.accentShow) this.accents[i].Visibility = ViewStates.Visible;
                }
                else
                {
                    if (this.iconShow) this.icons[i].view.SetAttr("src", this.icons[i].icon);
                    if (this.titleShow) this.titles[i].SetTextColor(this.titleColor);
                    if (this.accentShow) this.accents[i].Visibility = ViewStates.Invisible;
                }
            }
        }));

        this.tabLayout = new ScriptLinearLayout(context, new ViewArgs
        {
            ["h"] = height,
            ["bg"] = this.tabBackgroundColor,
            ["layoutGravity"] = this.isBottom ? "bottom" : "top",
        });

        if (this.isBottom)
        {
            this.viewPager.SetAttr("margin", new int[] { 0, 0, 0, this.height });
            base.AddView(this.viewPager);
            base.AddView(this.tabLayout);
        }
        else
        {
            this.viewPager.SetAttr("margin", new int[] { 0, this.height, 0, 0 });
            base.AddView(this.tabLayout);
            base.AddView(this.viewPager);
        }

        this.workDir = workDir;

        args.Remove("isBottom", "iconShow", "titleShow", "accentShow", "tabBg", "titleColor", "selectedTitleColor", "accentColor", "accentWidth");
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

            var tab = new ScriptFrameLayout(context, new ViewArgs
            {
                ["tag"] = this.tabLayout.ChildCount,
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
                    ["src"] = this.tabLayout.ChildCount == 0 ? tabView.SelectedIcon : tabView.Icon,
                    ["layoutGravity"] = this.titleShow ? "top|center" : "center",
                });
                tab.AddView(icon);
                this.icons.Add((icon, tabView.Icon, tabView.SelectedIcon));
            }

            if (this.titleShow)
            {
                var title = new ScriptTextView(context, new ViewArgs
                {
                    ["margin"] = new int[] { 0, 0, 0, this.iconShow ? 6 : 0 },
                    ["text"] = tabView.Title,
                    ["textSize"] = 14,
                    ["textColor"] = this.tabLayout.ChildCount == 0 ? this.selectedTitleColor : this.titleColor,
                    ["textStyle"] = "bold",
                    ["gravity"] = "center",
                    ["layoutGravity"] = this.iconShow ? "center|bottom" : "center",
                });
                tab.AddView(title);
                this.titles.Add(title);
            }

            if (this.accentShow)
            {
                var accent = new ScriptCardView(context, new ViewArgs
                {
                    ["margin"] = new int[] { 0, 0, 0, this.isBottom ? 2 : 0 },
                    ["w"] = this.accentWidth,
                    ["h"] = 3,
                    ["radius"] = 1.5,
                    ["bg"] = this.accentColor,
                    ["visibility"] = this.tabLayout.ChildCount == 0 ? "visible" : "invisible",
                    ["layoutGravity"] = "center|bottom",
                });
                tab.AddView(accent);
                this.accents.Add(accent);
            }


            tab.On("touch", new OnTouchListener((v, e) =>
            {
                if (e.Action == MotionEventActions.Down)
                {
                    v.SetBackgroundColor(DefaultTheme.ColorHint);
                }
                else
                {
                    v.SetBackgroundColor(Color.Transparent);
                    this.viewPager.SetCurrentItem(Convert.ToInt32(v.Tag), true);
                }
                return true;
            }));

            this.viewPager.AddView(tabView);
            this.tabLayout.AddView(tab);
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
        switch (key)
        {
            case "pageChange": this.viewPager.AddOnPageChangeListener((OnPageChangeListener)listener); break;
            default: Util.OnListener(this, key, listener); break;
        };
    }

}
