using Android.Content;
using Android.Content.Res;
using Google.Android.Material.Tabs;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using System.Reflection;

namespace astator.Views;
internal class CustomTabbedPageRenderer : TabbedPageRenderer
{
    public CustomTabbedPageRenderer(Context context) : base(context)
    {

    }

    private bool tabLayoutIsHide = false;

    protected override void OnLayout(bool changed, int l, int t, int r, int b)
    {
        base.OnLayout(changed, l, t, r, b);

        if (!this.tabLayoutIsHide)
        {
            var type = typeof(TabbedPageRenderer);
            var fields = type.GetField("_tabLayout", BindingFlags.NonPublic | BindingFlags.Instance);

            if (fields.GetValue(this) is TabLayout tabLayout)
            {
                tabLayout.RemoveAllTabs();
                tabLayout.Visibility = Android.Views.ViewStates.Gone;
                base.RemoveView(tabLayout);
            }
            this.tabLayoutIsHide = true;
        }
    }

    protected override ColorStateList GetItemIconTintColorState()
    {
        return base.GetItemIconTintColorState();
    }

    //protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
    //{
    //    Element?.OnThisPlatform().SetToolbarPlacement(ToolbarPlacement.Bottom);
    //    var type = typeof(TabbedPageRenderer);
    //    var isBottomTab = type.GetField("_bottomNavigationView", BindingFlags.NonPublic | BindingFlags.Instance);


    //    base.OnElementChanged(e);

    //    if (isBottomTab.GetValue(this) is BottomNavigationView bottomNavigationView)
    //    {
    //        bottomNavigationView.LabelVisibilityMode = LabelVisibilityMode.LabelVisibilityUnlabeled;
    //        var menuView = bottomNavigationView.MenuView as Google.Android.Material.BottomNavigation.BottomNavigationMenuView;
    //        var menuView = bottomNavigationView.MenuView as Google.Android.Material.BottomNavigation.BottomNavigationMenuView;
    //        menuView.LabelVisibilityMode = LabelVisibilityMode.LabelVisibilityUnlabeled;
    //        //bottomNavigationView.ItemIconSize = 200;
    //    }
    //}
}
