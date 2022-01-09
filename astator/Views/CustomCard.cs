using Android.Views;
using astator.Core.UI;
using Microsoft.Maui.Platform;

namespace astator.Views;

public class CustomCard : GridLayout
{
    public event EventHandler Clicked;
    protected override void OnHandlerChanged()
    {
        var view = this.Handler.NativeView as LayoutViewGroup;

        view.SetOnTouchListener(new OnTouchListener((v, e) =>
        {
            if (e.Action == MotionEventActions.Down)
            {
                this.BackgroundColor = Color.Parse("#cad4de");
            }
            else
            {
                this.BackgroundColor = (Color)Application.Current.Resources["PrimaryColor"];
            }
            return false;
        }));


        view.SetOnClickListener(new OnClickListener((v) =>
        {
            Clicked?.Invoke(this, null);
        }));
    }
}

