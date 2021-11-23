using astator.Core;
using astator.Pages;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using System.IO;

namespace astator
{
    public partial class MainPage : CarouselPage
    {
        public MainPage()
        {
            Build();
        }

        private void Build()
        {
            this.Children.Add(new HomePage());
            this.Children.Add(new LogPage());
            this.Children.Add(new DocPage());
            this.Children.Add(new SettingsPage());

            Console.SetOut(new ScriptConsole());

            OutputResources("References");
        }

        private static async void OutputResources(string folder)
        {
            try
            {
                var path = Path.Combine(MauiApplication.Current.FilesDir.AbsolutePath, "Resources", folder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var list = await MauiApplication.Current.Assets.ListAsync($"Resources/{folder}");
                foreach (var item in list)
                {
                    using var stream = MauiApplication.Current.Assets.Open($"Resources/{folder}/{item}");
                    using var fileStream = File.Create($"{path}/{item}");
                    stream.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                ScriptLogger.Instance.Error(ex.Message);
            }
        }
    }
}
