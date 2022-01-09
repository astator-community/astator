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

#if DEBUG
            ReleaseScriptFiles();
#endif

            Console.SetOut(new ScriptConsole());

            var mainPage = new CarouselPage();
            mainPage.Children.Add(new HomePage());
            mainPage.Children.Add(new LogPage());
            mainPage.Children.Add(new DocPage());
            mainPage.Children.Add(new SettingsPage());

            var navPage = new NavigationPage(mainPage);
            NavigationPage.SetHasNavigationBar(mainPage, false);
            this.MainPage = navPage;

        }

        private void ReleaseScriptFiles()
        {
            var sdkPath = Android.App.Application.Context.Assets.List("Resources/ScriptFiles").FirstOrDefault(name => name.EndsWith("sdk.zip"));
            var sdkFileName = sdkPath[sdkPath.LastIndexOf("v")..];
            var version = sdkFileName[1..sdkFileName.IndexOf('-')];
            var outputDir = Android.App.Application.Context.GetExternalFilesDir("Sdk").ToString();
            var versionPath = Path.Combine(outputDir, "version.txt");
            if (!File.Exists(versionPath) || int.Parse(version) > int.Parse(File.ReadAllText(versionPath)))
            {
                using (var stream = Android.App.Application.Context.Assets.Open(Path.Combine("Resources/ScriptFiles", sdkPath)))
                {
                    using var zip = new ZipArchive(stream);

                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }
                    zip.ExtractToDirectory(outputDir, true);
                }
                File.WriteAllText(versionPath, version);
            };
        }
    }
}