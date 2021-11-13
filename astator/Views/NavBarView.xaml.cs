using Microsoft.Maui.Controls;
using System;
namespace astator.Views
{
    public partial class NavBarView
    {
        private string activeTab;
        public string ActiveTab
        {
            get
            {
                return this.activeTab;
            }
            set
            {
                this.activeTab = value;
                var target = this.activeTab;
                var homeImg = (target == "home") ? "tab_home_on.png" : "tab_home.png";
                var docImg = (target == "doc") ? "tab_doc_on.png" : "tab_doc.png";
                var logImg = (target == "log") ? "tab_log_on.png" : "tab_log.png";
                var settingsImg = (target == "settings") ? "tab_settings_on.png" : "tab_settings.png";
                this.HomeTab.Source = ImageSource.FromFile(homeImg);
                this.DocTab.Source = ImageSource.FromFile(docImg);
                this.LogTab.Source = ImageSource.FromFile(logImg);
                this.SettingsTab.Source = ImageSource.FromFile(settingsImg);
            }
        }
        public NavBarView()
        {
            InitializeComponent();
        }
        public static void SetCurrentPage(int index)
        {
            var page = Application.Current.MainPage as CarouselPage;
            page.CurrentPage = page.GetPageByIndex(index);
        }
        private void HomeTab_Clicked(object sender, EventArgs e) => SetCurrentPage(0);
        private void LogTab_Clicked(object sender, EventArgs e) => SetCurrentPage(1);
        private void DocTab_Clicked(object sender, EventArgs e) => SetCurrentPage(2);
        private void SettingsTab_Clicked(object sender, EventArgs e) => SetCurrentPage(3);
    }
}