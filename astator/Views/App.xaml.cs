using astator.Core;

namespace astator
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            Console.SetOut(new ScriptConsole());

            var mainPage = new CarouselPage();
            var navPage = new NavigationPage(mainPage);
            NavigationPage.SetHasNavigationBar(mainPage, false);
            this.MainPage = navPage;
        }
    }
}