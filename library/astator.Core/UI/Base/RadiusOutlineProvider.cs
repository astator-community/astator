using Android.Views;

namespace astator.Core.UI.Base;
public class RadiusOutlineProvider : ViewOutlineProvider
{
    private readonly float radius;
    public RadiusOutlineProvider(float radius)
    {
        this.radius = radius;
    }
    public override void GetOutline(View view, Android.Graphics.Outline outline)
    {
        outline.SetRoundRect(0, 0, view.Width, view.Height, this.radius);
    }
}
