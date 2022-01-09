using Android.Views;
using astator.Core.UI;

namespace astator.Views;

internal class CustomSwitch : Switch
{

    public event EventHandler Clicked;

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var view = this.Handler.NativeView as Android.Views.View;

        view.SetOnTouchListener(new OnTouchListener((v, e) =>
        {
            if (e.Action == MotionEventActions.Down)
            {
                Clicked?.Invoke(this, null);
            }

            return Clicked is not null;
        }));
    }
}
