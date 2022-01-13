using System;
using System.Net;
using astator.Core;
using astator.NugetManager;
using astator.Views;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace astator.Pages
{
    public partial class NugetPage : ContentPage
    {
        public NugetPage()
        {
            InitializeComponent();
            SearchPkg();
        }

        private async void SearchPkg()
        {
            SearchBtn.IsEnabled = false;
            SearchBtn.Source = "search_disable.png";
            Refresh.IsRefreshing = true;
            PkgLayout.Clear();

            var pkgs = await NugetCommands.SearchPkgAsync(SearchEditor.Text);

            var cards = await Task.Run(() =>
            {
                var result = new List<PackageInfoCard>();
                if (pkgs is not null)
                {
                    var client = new HttpClient();
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
                return result;
            });

            foreach (var card in cards)
            {
                card.Clicked += PkgCard_Clicked;
                PkgLayout.Add(card);
            }

            Refresh.IsRefreshing = false;
            SearchBtn.Source = "search.png";
            SearchBtn.IsEnabled = true;
        }

        private void SearchBtn_Clicked(object sender, EventArgs e)
        {
            SearchPkg();
        }

        private void PkgCard_Clicked(object sender, EventArgs e)
        {
            var card = sender as PackageInfoCard;

            var page = new PackageInfoPage(card.PkgId);
            Navigation.PushAsync(page);

        }

        private void SearchEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.EndsWith("\n"))
            {
                SearchEditor.Text = SearchEditor.Text.Trim();
                SearchBtn_Clicked(null, null);
            }
        }

        private void Refresh_Refreshing(object sender, EventArgs e)
        {
            if (SearchBtn.IsEnabled)
            {
                SearchPkg();
            }
        }
    }
}