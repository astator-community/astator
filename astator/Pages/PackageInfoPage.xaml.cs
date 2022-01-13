using System;
using System.ComponentModel;
using astator.Core;
using astator.NugetManager;
using astator.Views;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Essentials;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

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

            packages = await NugetCommands.GetPackagesMetadataAsync(PkgId);

            var versions = new List<string>();
            foreach (var pkg in packages)
            {
                versions.Add("v" + pkg.Identity.Version.ToString());
            }
            VersionItems.Items = string.Join(",", versions);

            VersionItems.SelectedItem = 0;
        }

        private void ShowPkgInfo(int index)
        {
            var pkg = packages[index];

            Description.Text = pkg.Description ?? default;
            Version.Text = pkg.Identity.Version.ToString();
            Authors.Text = pkg.Authors;
            if (pkg.LicenseMetadata is not null)
            {
                License.Text = pkg.LicenseMetadata.License;
                License.TextType = TextType.Text;
                License.TextDecorations = TextDecorations.None;
                License.TextColor = (Color)Application.Current.Resources["SecondaryColor"];
                License.Clicked -= Uri_Clicked;
            }
            else if (pkg.LicenseUrl is not null)
            {
                License.Text = "查看许可证";
                License.Tag = pkg.LicenseUrl.ToString();
                License.TextType = TextType.Html;
                License.TextDecorations = TextDecorations.Underline;
                License.TextColor = Color.Parse("#56c2ec");
                License.Clicked += Uri_Clicked;
            }

            PublishDate.Text = pkg.Published.Value.ToString("d");
            ProjectUrl.Text = pkg.ProjectUrl?.ToString() ?? default;
            ProjectUrl.Tag = pkg.ProjectUrl?.ToString() ?? default;

            var dir = Path.Combine(NugetCommands.NugetDirectory, this.PkgId.Text, Version.Text);
            if (Directory.Exists(dir))
            {
                this.AttribBtn.Tag = "delete";
                this.AttribBtn.Source = "file_delete.png";
            }
            else
            {
                this.AttribBtn.Tag = "download";
                this.AttribBtn.Source = "file_download.png";
            }


            DependencyList.Clear();

            var group = NugetCommands.GetNearestFrameworkDependencyGroup(pkg.DependencySets);

            if (group is null)
            {
                var dPkgLabel = new Label
                {
                    Margin = new Thickness(20, 0, 0, 0),
                    Text = "不兼容"
                };
                DependencyList.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                dPkgLabel.SetValue(GridLayout.RowProperty, DependencyList.Children.Count);
                DependencyList.Children.Add(dPkgLabel);
                return;
            }

            var targetLabel = new Label
            {
                Text = group.TargetFramework.GetShortFolderName()
            };

            DependencyList.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            targetLabel.SetValue(GridLayout.RowProperty, DependencyList.Children.Count);
            DependencyList.Children.Add(targetLabel);

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
                    DependencyList.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    dPkgLabel.SetValue(GridLayout.RowProperty, DependencyList.Children.Count);
                    DependencyList.Children.Add(dPkgLabel);
                }
            }
            else
            {
                var dPkgLabel = new Label
                {
                    Margin = new Thickness(20, 0, 0, 0),
                    Text = "无依赖项"
                };
                DependencyList.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                dPkgLabel.SetValue(GridLayout.RowProperty, DependencyList.Children.Count);
                DependencyList.Children.Add(dPkgLabel);
            }
        }

        private async void AttribBtn_Clicked(object sender, EventArgs e)
        {
            Refresh.IsRefreshing = true;

            var id = PkgId.Text;
            var version = NuGetVersion.Parse(Version.Text);
            if (AttribBtn.Tag?.ToString() == "download")
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
                var dir = Path.Combine(NugetCommands.NugetDirectory, id, Version.Text);
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir,true);
                }
            }

            ShowPkgInfo(VersionItems.SelectedItem);
            Refresh.IsRefreshing = false;

        }



        private void VersionItems_SelectionChanged(object sender, SelectedItemChangedEventArgs e)
        {
            Refresh.IsRefreshing = true;
            ShowPkgInfo(e.SelectedItemIndex);
            Refresh.IsRefreshing = false;
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
                Underline.X2 = (Underline.Parent as GridLayout).Width - 10;
            }
        }
    }
}