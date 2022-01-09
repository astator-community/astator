using Android.Views;
using astator.Core;
using astator.Core.UI;

namespace astator.Views;

internal class CustomLabel : Label
{
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var view = this.Handler.NativeView as AndroidX.AppCompat.Widget.AppCompatTextView;

        var menu = new AndroidX.AppCompat.Widget.PopupMenu(Globals.AppContext, view);

        menu.Menu.Add("复制");

        menu.SetOnMenuItemClickListener(new OnMenuItemClickListener((item) =>
        {
            if (item.GroupId == 0)
            {
                Clipboard.SetTextAsync(view.Text);
            }
            return true;
        }));

        view.SetOnLongClickListener(new OnLongClickListener((v) =>
        {
            menu.Show();
            return true;
        }));

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
    }
}
