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

    public string CustomId { get; set; }
    public OnAttachedListener OnAttachedListener { get; set; }

    private readonly List<View> pages = new();

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        this.Adapter = new ViewPagerAdapter(this.pages);
        SetCurrentItem(0, true);
        this.OnAttachedListener?.OnAttached(this);
    }

    public new ILayout AddView(View view)
    {
        this.pages.Add(view);
        return this;
    }

    public ScriptViewPager(Android.Content.Context context, ViewArgs args) : base(context)
    {
        this.SetCustomId(ref args);
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
        switch (key)
        {
            case "pageChange":
            {
                AddOnPageChangeListener((OnPageChangeListener)listener);
                break;
            }
            default:
            {
                this.OnListener(key, listener);
                return;
            }
        }

    }
}
