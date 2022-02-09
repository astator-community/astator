using Android.Views;

namespace astator.Core.UI.Base;
public interface ILayout : IView
{
    public ILayout AddView(View view);
}
