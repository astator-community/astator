using Android.Views;

namespace astator.Core.UI.Base;
public class CircleOutlineProvider : ViewOutlineProvider
{
    public override void GetOutline(View view, Android.Graphics.Outline outline)
    {
        outline.SetRoundRect(0, 0, view.Width, view.Height, (view.Width < view.Height ? view.Width : view.Height) / 2);
    }
}
