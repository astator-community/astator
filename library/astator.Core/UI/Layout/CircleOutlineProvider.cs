using Android.Views;

namespace astator.Core.UI.Layout;
public class CircleOutlineProvider : ViewOutlineProvider
{
    public override void GetOutline(Android.Views.View view, Android.Graphics.Outline outline)
    {
        outline.SetRoundRect(0, 0, view.Width, view.Height, (view.Width < view.Height ? view.Width : view.Height) / 2);
    }
}
