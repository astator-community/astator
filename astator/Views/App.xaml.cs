using astator.Core.Script;
using astator.Pages;

namespace astator;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        Console.SetOut(new ScriptConsole());
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



    private static void NotPermissionExit()
    {
        new AndroidX.AppCompat.App.AlertDialog
           .Builder(Globals.AppContext)
           .SetTitle("错误")
           .SetMessage("请求权限失败, 应用退出!")
           .SetPositiveButton("确认", (s, e) =>
           {
               Java.Lang.JavaSystem.Exit(0);
           })
           .Show();
    }
}


