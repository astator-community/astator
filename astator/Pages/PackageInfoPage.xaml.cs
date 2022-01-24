using astator.Core;
using astator.NugetManager;
using astator.Views;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.ComponentModel;

namespace astator
{
    public partial class PackageInfoPage : ContentPage
    {
        public PackageInfoPage(string PkgId)
        {
            InitializeComponent();

            Initialize(PkgId);
        }

        private List<IPackageSearchMetadata> packages;

        private async void Initialize(string PkgId)
        {
            this.PkgId.Text = PkgId;

            this.packages = await NugetCommands.GetPackagesMetadataAsync(PkgId);

            var versions = new List<string>();
            foreach (var pkg in this.packages)
            {
                versions.Add("v" + pkg.Identity.Version.ToString());
            }
            this.VersionItems.Items = string.Join(",", versions);

            this.VersionItems.SelectedItem = 0;
        }

        private void ShowPkgInfo(int index)
        {
            var pkg = this.packages[index];

            this.Description.Text = pkg.Description ?? default;
            this.Version.Text = pkg.Identity.Version.ToString();
            this.Authors.Text = pkg.Authors;
            if (pkg.LicenseMetadata is not null)
            {
                this.License.Text = pkg.LicenseMetadata.License;
                this.License.TextType = TextType.Text;
                this.License.TextDecorations = TextDecorations.None;
                this.License.TextColor = (Color)Application.Current.Resources["SecondaryColor"];
                this.License.Clicked -= Uri_Clicked;
            }
            else if (pkg.LicenseUrl is not null)
            {
                this.License.Text = "查看许可证";
                this.License.Tag = pkg.LicenseUrl.ToString();
                this.License.TextType = TextType.Html;
                this.License.TextDecorations = TextDecorations.Underline;
                this.License.TextColor = Color.Parse("#56c2ec");
                this.License.Clicked += Uri_Clicked;
            }

            this.PublishDate.Text = pkg.Published.Value.ToString("d");
            this.ProjectUrl.Text = pkg.ProjectUrl?.ToString() ?? default;
            this.ProjectUrl.Tag = pkg.ProjectUrl?.ToString() ?? default;

            var dir = Path.Combine(NugetCommands.NugetDirectory, this.PkgId.Text, this.Version.Text);
            if (Directory.Exists(dir))
            {
                this.AttribBtn.Tag = "delete";
                this.AttribBtn.Source = "delete.png";
            }
            else
            {
                this.AttribBtn.Tag = "download";
                this.AttribBtn.Source = "download.png";
            }


            this.DependencyList.Clear();

            var group = NugetCommands.GetNearestFrameworkDependencyGroup(pkg.DependencySets);

            if (group is null)
            {
                var dPkgLabel = new Label
                {
                    Margin = new Thickness(20, 0, 0, 0),
                    Text = "不兼容"
                };
                this.DependencyList.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                dPkgLabel.SetValue(GridLayout.RowProperty, this.DependencyList.Children.Count);
                this.DependencyList.Children.Add(dPkgLabel);
                return;
            }

            var targetLabel = new Label
            {
                Text = group.TargetFramework.GetShortFolderName()
            };

            this.DependencyList.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            targetLabel.SetValue(GridLayout.RowProperty, this.DependencyList.Children.Count);
            this.DependencyList.Children.Add(targetLabel);

            if (group.Packages.Any())
            {
                foreach (var dPkg in group.Packages)
                {
                    var dPkgLabel = new Label
                    {
                        LineBreakMode = LineBreakMode.CharacterWrap,
                        Margin = new Thickness(20, 0, 0, 0),
                        Text = $"{dPkg.Id}>={dPkg.VersionRange.MinVersion}"
                    };
                    this.DependencyList.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    dPkgLabel.SetValue(GridLayout.RowProperty, this.DependencyList.Children.Count);
                    this.DependencyList.Children.Add(dPkgLabel);
                }
            }
            else
            {
                var dPkgLabel = new Label
                {
                    Margin = new Thickness(20, 0, 0, 0),
                    Text = "无依赖项"
                };
                this.DependencyList.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                dPkgLabel.SetValue(GridLayout.RowProperty, this.DependencyList.Children.Count);
                this.DependencyList.Children.Add(dPkgLabel);
            }
        }

        private async void AttribBtn_Clicked(object sender, EventArgs e)
        {
            this.Refresh.IsRefreshing = true;

            var id = this.PkgId.Text;
            var version = NuGetVersion.Parse(this.Version.Text);
            if (this.AttribBtn.Tag?.ToString() == "download")
            {
                var dependences = await NugetCommands.ListPackageTransitiveDependenceAsync(id, version);

                foreach (var dependence in dependences)
                {
                    var dir = Path.Combine(NugetCommands.NugetDirectory, dependence.Key, dependence.Value.ToString());

                    if (!Directory.Exists(dir))
                    {
                        if (!await NugetCommands.DownLoadPackageAsync(dependence.Key, dependence.Value))
                        {
                            ScriptLogger.Error($"下载nuget包: {dependence.Key}失败!");
                            return;
                        }
                    }
                }
            }
            else
            {
                var dir = Path.Combine(NugetCommands.NugetDirectory, id, this.Version.Text);
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
            }

            ShowPkgInfo(this.VersionItems.SelectedItem);
            this.Refresh.IsRefreshing = false;

        }



        private void VersionItems_SelectionChanged(object sender, SelectedItemChangedEventArgs e)
        {
            this.Refresh.IsRefreshing = true;
            ShowPkgInfo(e.SelectedItemIndex);
            this.Refresh.IsRefreshing = false;
        }
        private void Uri_Clicked(object sender, EventArgs e)
        {
            var view = sender as CustomLabel;
            if (!string.IsNullOrEmpty(view.Tag?.ToString()))
            {
                Launcher.TryOpenAsync(new Uri(view.Tag.ToString()));
            }
        }

        private void VersionItems_SizeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Width")
            {
                this.Underline.X2 = (this.Underline.Parent as GridLayout).Width - 10;
            }
        }
    }
}