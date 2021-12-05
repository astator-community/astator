using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace astator.Views
{
    public sealed partial class DirCard : UserControl
    {
        public string FullPath { get; set; }
        public DirCard(string fullPath, string pathName, string pathInfo)
        {
            InitializeComponent();
            this.FullPath = fullPath;
            this.PathName.Text = pathName;
            this.PathInfo.Text = pathInfo;
        }

        private void Option_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var grid = sender as Grid;
            grid.ContextFlyout?.ShowAt(grid);
            e.Handled = true;
        }

        private void Rename_Clicked(object sender, RoutedEventArgs e)
        {

        }

        private void Remove_Clicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
