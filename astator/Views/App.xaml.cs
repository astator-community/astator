using astator.Pages;

namespace astator
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            var mainPage = new CarouselPage();
            mainPage.Children.Add(new HomePage());
            mainPage.Children.Add(new LogPage());
            mainPage.Children.Add(new DocPage());
            mainPage.Children.Add(new SettingsPage());

            this.MainPage = mainPage;
        }
    }
}