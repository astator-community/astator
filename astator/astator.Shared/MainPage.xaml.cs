using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace astator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var sdkPath = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith("sdk.zip"));
            var sdkFileName = sdkPath[sdkPath.LastIndexOf("v")..];
            var version = sdkFileName[1..sdkFileName.IndexOf('-')];
            var outputDir = Android.App.Application.Context.GetExternalFilesDir("Sdk").ToString();
            var versionPath = Path.Combine(outputDir, "version.txt");
            if (!File.Exists(versionPath) || int.Parse(version) > int.Parse(File.ReadAllText(versionPath)))
            {
                using (var stream = assembly.GetManifestResourceStream(sdkPath))
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
            InitializeComponent();
        }

        private void MainPage_Loaded(object sender, EventArgs e)
        {
            MainActivity.Instance.KeyDownCallback = this.HomePage.OnKeyDown;
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var flip = sender as FlipView;
            var index = flip.SelectedIndex;
            this.HomeTab.Source = index == 0 ? "Assets/Images/tab_home_on.png" : "Assets/Images/tab_home.png";
            this.LogTab.Source = index == 1 ? "Assets/Images/tab_log_on.png" : "Assets/Images/tab_log.png";
            this.DocTab.Source = index == 2 ? "Assets/Images/tab_doc_on.png" : "Assets/Images/tab_doc.png";
            this.SettingTab.Source = index == 3 ? "Assets/Images/tab_setting_on.png" : "Assets/Images/tab_setting.png";

            MainActivity.Instance.KeyDownCallback = index == 0 ? this.HomePage.OnKeyDown : index == 1 ? this.LogPage.OnKeyDown : index == 2 ? this.DocPage.OnKeyDown : index == 3 ? this.SettingPage.OnKeyDown : null;
        }

        private void TabBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var btn = sender as Button;
            this.FlipView.SelectedIndex = btn.Name == "LogTabBtn" ? 1 : btn.Name == "DocTabBtn" ? 2 : btn.Name == "SettingTabBtn" ? 3 : 0;
        }

    }
}
