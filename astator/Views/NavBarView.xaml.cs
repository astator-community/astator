namespace astator.Views
{
    public partial class NavBarView : ContentView
    {

        public static readonly BindableProperty ActiveTabBindableProperty = BindableProperty.Create(nameof(ActiveTab), typeof(string), typeof(NavBarView));

        public string ActiveTab
        {
            get => GetValue(ActiveTabBindableProperty) as string;
            set
            {
                SetValue(ActiveTabBindableProperty, value);

                if (value == "home")
                {
                    this.HomeTab.Source = ImageSource.FromFile("home_on.png");
                }
                else if (value == "log")
                {
                    this.LogTab.Source = ImageSource.FromFile("log_on.png");
                }
                else if (value == "doc")
                {
                    this.DocTab.Source = ImageSource.FromFile("doc_on.png");
                }
                else if (value == "settings")
                {
                    this.SettingsTab.Source = ImageSource.FromFile("settings_on.png");
                }
            }
        }

        public NavBarView()
        {
            InitializeComponent();
        }


        private void SetCurrentPage(int index)
        {
            //var mainPage = Application.Current.MainPage as NavigationPage;
            var tabbedPage = Application.Current.MainPage as TabbedPage;
            tabbedPage.CurrentPage = tabbedPage.Children[index];
        }

        private void HomeTab_Clicked(object sender, EventArgs e)
        {
            SetCurrentPage(0);
        }
        private void LogTab_Clicked(object sender, EventArgs e)
        {
            SetCurrentPage(1);

        }
        private void DocTab_Clicked(object sender, EventArgs e)
        {
            SetCurrentPage(2);
        }
        private void SettingsTab_Clicked(object sender, EventArgs e)
        {
            SetCurrentPage(3);
        }
    }
}