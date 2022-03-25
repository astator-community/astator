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

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();

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
}
