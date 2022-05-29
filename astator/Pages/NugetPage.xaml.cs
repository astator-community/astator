using astator.NugetManager;
using astator.Views;

namespace astator.Pages
{
    public partial class NugetPage : ContentPage
    {
        private readonly List<string> sourceNames;
        private readonly List<string> sourceUris;
        public NugetPage()
        {
            InitializeComponent();

            var nugetCommands = new NugetCommands();
            var sourceRepositories = nugetCommands.GetRepositories();
            this.sourceNames = sourceRepositories.Select(x => x.PackageSource.Name).ToList();
            this.sourceUris = sourceRepositories.Select(x => x.PackageSource.Source).ToList();
            this.SourceItems.Items = string.Join(",", this.sourceNames);
            var source = Core.Script.Preferences.Get("NugetSource", string.Empty, "astator");
            this.SourceItems.SelectedItem = this.sourceNames.IndexOf(source);
        }

        private async void SearchPkg()
        {
            if (this.SourceItems.SelectedItem == -1) return;
            this.SearchBtn.IsEnabled = false;
            this.SearchBtn.Source = "search_disable.png";
            this.Refresh.IsRefreshing = true;
            this.PkgLayout.Clear();

            var pkgs = await NugetCommands.SearchPkgAsync(this.SearchEditor.Text, this.sourceUris[this.SourceItems.SelectedItem]);

            //var cards = await Task.Run(() =>
            //{
            var result = new List<PackageInfoCard>();
            if (pkgs is not null)
            {
                foreach (var pkg in pkgs)
                {
                    var card = new PackageInfoCard
                    {
                        IconUri = pkg.IconUrl is not null
                            ? new Uri(pkg.IconUrl.ToString().Replace("api.nuget.org", "nuget.cdn.azure.cn"))
                            : null,

                        PkgId = pkg.Identity.Id,
                        Version = "v" + pkg.Identity.Version.ToString(),
                        Description = pkg.Description
                    };

                    result.Add(card);
                }
            }
            //return result;
            //});

            foreach (var card in result)
            {
                card.Clicked += PkgCard_Clicked;
                this.PkgLayout.Add(card);
            }

            this.Refresh.IsRefreshing = false;
            this.SearchBtn.Source = "search.png";
            this.SearchBtn.IsEnabled = true;
        }

        private void SearchBtn_Clicked(object sender, EventArgs e)
        {
            SearchPkg();
        }

        private void PkgCard_Clicked(object sender, EventArgs e)
        {
            var card = sender as PackageInfoCard;

            var page = new PackageInfoPage(card.PkgId, this.sourceUris[this.SourceItems.SelectedItem]);
            this.Navigation.PushModalAsync(page);

        }

        private void SearchEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.EndsWith("\n"))
            {
                this.SearchEditor.Text = this.SearchEditor.Text.Trim();
                SearchBtn_Clicked(null, null);
            }
        }

        private void Refresh_Refreshing(object sender, EventArgs e)
        {
            if (this.SearchBtn.IsEnabled)
            {
                SearchPkg();
            }
        }

        private void SourceItems_SelectionChanged(object sender, SelectedItemChangedEventArgs e)
        {
            Core.Script.Preferences.Set("NugetSource", this.sourceNames[this.SourceItems.SelectedItem], "astator");
        }
    }
}