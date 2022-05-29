using astator.Core.Script;
using astator.NugetManager;
using astator.Views;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.ComponentModel;

namespace astator
{
    public partial class PackageInfoPage : ContentPage
    {
        private readonly NugetCommands nugetCommands;
        private readonly string nugetSource;
        public PackageInfoPage(string PkgId, string nugetSource)
        {
            InitializeComponent();
            Initialize(PkgId);
            this.nugetSource = nugetSource;
            this.nugetCommands = new NugetCommands(nugetSource);
        }

        private List<IPackageSearchMetadata> packages;

        private async void Initialize(string PkgId)
        {
            this.PkgId.Text = PkgId;

            this.packages = await NugetCommands.GetPackagesMetadataAsync(PkgId,this.nugetSource);

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
                this.License.Clicked += Uri_Clicked;
            }
            else if (pkg.LicenseUrl is not null)
            {
                this.License.Text = "查看许可证";
                this.License.Tag = pkg.LicenseUrl.ToString();
                this.License.TextType = TextType.Html;
                this.License.TextDecorations = TextDecorations.Underline;
                this.License.TextColor = Color.Parse("#56c2ec");
                this.License.Clicked -= Uri_Clicked;
            }

            this.PublishDate.Text = pkg.Published.Value.ToString("d");
            this.ProjectUrl.Text = pkg.ProjectUrl?.ToString() ?? default;
            this.ProjectUrl.Tag = pkg.ProjectUrl?.ToString() ?? default;

            var dir = NugetCommands.GetInstalledDir(this.PkgId.Text, pkg.Identity.Version);
            if (Directory.Exists(dir))
            {
                this.StateBtn.Tag = "delete";
                this.StateBtn.Source = "delete.png";
            }
            else
            {
                this.StateBtn.Tag = "install";
                this.StateBtn.Source = "install.png";
            }


            this.DependencyList.Clear();
            var framework = NugetCommands.GetNearestFramework(pkg.DependencySets.Select(x => x.TargetFramework));
            var group = pkg.DependencySets.Where(x => x.TargetFramework.Equals(framework)).First();

            if (framework is null)
            {
                var dPkgLabel = new Label
                {
                    Margin = new Thickness(20, 0, 0, 0),
                    Text = "不兼容"
                };
                this.DependencyList.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                dPkgLabel.SetValue(Grid.RowProperty, this.DependencyList.Children.Count);
                this.DependencyList.Children.Add(dPkgLabel);
                return;
            }

            var targetLabel = new Label
            {
                Text = framework.GetShortFolderName()
            };

            this.DependencyList.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            targetLabel.SetValue(Grid.RowProperty, this.DependencyList.Children.Count);
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
                    dPkgLabel.SetValue(Grid.RowProperty, this.DependencyList.Children.Count);
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
                dPkgLabel.SetValue(Grid.RowProperty, this.DependencyList.Children.Count);
                this.DependencyList.Children.Add(dPkgLabel);
            }
        }

        private async void StateBtn_Clicked(object sender, EventArgs e)
        {
            this.Refresh.IsRefreshing = true;

            var id = this.PkgId.Text;
            if (this.StateBtn.Tag?.ToString() == "install")
            {
                var result = await this.nugetCommands.InstallPackageAsync(id, NuGetVersion.Parse(this.Version.Text));
                if (!result) Logger.Error($"安装nuget包: {id}失败!");
            }
            else
            {
                var dir = NugetCommands.GetInstalledDir(id, NuGetVersion.Parse(this.Version.Text));
                if (!string.IsNullOrEmpty(dir)) Directory.Delete(dir, true);
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
                this.Underline.X2 = (this.Underline.Parent as Grid).Width - 10;
            }
        }
    }
}