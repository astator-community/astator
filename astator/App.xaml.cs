
using astator.Core.Script;
using astator.Modules.Base;
using astator.Pages;

namespace astator;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        System.Console.SetOut(new ConsoleOut());
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        if (this.MainPage == null)
        {
            InitializeMainPage();
        }
        return base.CreateWindow(activationState);
    }

    private void InitializeMainPage()
    {
        var mainPage = new TabbedPage();
        if (Android.App.Application.Context.PackageName == Globals.AstatorPackageName)
        {
            mainPage.Children.Add(new HomePage());
            mainPage.Children.Add(new LogPage());
            mainPage.Children.Add(new DocPage());
            mainPage.Children.Add(new SettingsPage());
        }
        else
        {
            mainPage.Children.Add(new LogPage());
        }
        this.MainPage = mainPage;
    }
}


