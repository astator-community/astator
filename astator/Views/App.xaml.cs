using astator.Controllers;
using astator.Core;
using astator.Pages;
using System.IO.Compression;

namespace astator
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            Console.SetOut(new ScriptConsole());

            if (Android.App.Application.Context.PackageName == "com.astator.astator")
            {
                var mainPage = new CarouselPage();
                mainPage.Children.Add(new HomePage());
                mainPage.Children.Add(new LogPage());
                mainPage.Children.Add(new DocPage());
                mainPage.Children.Add(new SettingsPage());

                var navPage = new NavigationPage(mainPage);
                NavigationPage.SetHasNavigationBar(mainPage, false);
                this.MainPage = navPage;
            }
            else
            {
                var mainPage = new ScriptLogPage();
                this.MainPage = mainPage;

                RunProject();
            }
        }

        private static async void RunProject()
        {
            try
            {
                var outputDir = Android.App.Application.Context.GetExternalFilesDir("project").ToString();
                if (Directory.Exists(outputDir))
                {
                    var PackageUndateTime = Android.App.Application.Context.PackageManager.GetPackageInfo(Android.App.Application.Context.PackageName, 0).LastUpdateTime;

                    long lastUpdateTime = 0;

                    if (File.Exists(Path.Combine(outputDir, "lastUpdateTime.txt")))
                    {
                        lastUpdateTime = long.Parse(File.ReadAllText(Path.Combine(outputDir, "lastUpdateTime.txt")));
                    }

                    if (PackageUndateTime > lastUpdateTime)
                    {
                        ReleaseProject();
                    }
                }
                else
                {
                    ReleaseProject();
                }

                var runtime = await ScriptManager.Instance.RunProjectFromDll(outputDir);
                runtime.IsExitAppOnStoped = runtime.IsUiMode;
            }
            catch (Exception ex)
            {
                ScriptLogger.Error(ex.Message);
            }
        }

        private static void ReleaseProject()
        {
            var outputDir = Android.App.Application.Context.GetExternalFilesDir("project").ToString();
            using var stream = Android.App.Application.Context.Assets.Open(Path.Combine("Resources/project.zip"));
            using var zip = new ZipArchive(stream);

            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }
            Directory.CreateDirectory(outputDir);
            zip.ExtractToDirectory(outputDir, true);

            File.WriteAllText(Path.Combine(outputDir, "lastUpdateTime.txt"), Java.Lang.JavaSystem.CurrentTimeMillis().ToString());
        }
    }
}