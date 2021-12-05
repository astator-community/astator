using Android.Views;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace astator.Pages
{
    public sealed partial class DocPage : UserControl
    {
        public DocPage()
        {
            InitializeComponent();
        }


        public new bool OnKeyDown(Keycode keycode, KeyEvent e)
        {
            return false;
        }
    }
}
