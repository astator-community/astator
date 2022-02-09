using Android.Views;
using AndroidX.ViewPager.Widget;
using astator.Core.UI.Base;
using System.Collections.Generic;

namespace astator.Core.UI.Layouts;

public class ScriptViewPager : ViewPager, ILayout
{
    public class ViewPagerAdapter : PagerAdapter
    {
        public override int Count => this.pages.Count;
        private readonly List<View> pages;
        public ViewPagerAdapter(List<View> pages)
        {
            this.pages = pages;
        }
        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }
        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            container.AddView(this.pages[position]);
            return this.pages[position];
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView((View)@object);
        }
    }

    public string CustomId { get; set; } = string.Empty;
    public OnAttachedListener OnAttachedListener { get; set; }

    private readonly List<View> _pages = new();

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        this.Adapter = new ViewPagerAdapter(this._pages);
        this.OnAttachedListener?.OnAttached(this);
    }

    ILayout ILayout.AddView(View view)
    {
        this._pages.Add(view);
        return this;
    }

    public ScriptViewPager(Android.Content.Context context, ViewArgs args) : base(context)
    {
        if (args is null)
        {
            return;
        }
        if (args["id"] is null)
        {
            this.CustomId = $"{ GetType().Name }-{ UiManager.CreateCount }";
            UiManager.CreateCount++;
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
            case "currentItem":
            {
                this.CurrentItem = Util.DpParse(value); break;
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
            "currentItem" => this.CurrentItem,
            _ => Util.GetAttr(this, key)
        };
    }
    public void On(string key, object listener)
    {
        this.OnListener(key, listener);
    }
}
