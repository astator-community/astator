using astator.Controllers;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace astator.Views
{
    public sealed partial class FileCard : UserControl
    {
        public string FullPath { get; set; }
        public FileCard(string fullPath, string pathName, string pathInfo, string iconSource)
        {
            InitializeComponent();
            this.FullPath = fullPath;
            this.PathName.Text = pathName;
            this.PathInfo.Text = pathInfo;
            this.Icon.Source = iconSource;

            switch (Path.GetExtension(pathName))
            {
                case ".cs":
                    this.RunScript.IsEnabled = true;
                    break;
                case ".csproj":
                    this.RunProject.IsEnabled = true;
                    break;
                default:
                    break;
            }
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
        private void RunScript_Clicked(object sender, RoutedEventArgs e)
        {
            // ScriptManager.Instance.RunProject(Path.GetDirectoryName(this.FullPath), Path.GetFileNameWithoutExtension(this.FullPath));
        }
        private void RunProject_Clicked(object sender, RoutedEventArgs e)
        {
            ScriptManager.Instance.RunProject(Path.GetDirectoryName(this.FullPath), Path.GetFileNameWithoutExtension(this.FullPath));
        }

    }
}
