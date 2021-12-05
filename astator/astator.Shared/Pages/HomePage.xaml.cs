
using Android.Graphics;
using Android.Views;
using astator.Views;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Path = System.IO.Path;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace astator.Pages
{
    public sealed partial class HomePage : UserControl
    {
        private readonly List<TextBlock> pathTbs = new();

        public HomePage()
        {
            InitializeComponent();
        }

        private void HomePage_Loaded(object sender, EventArgs e)
        {
            var rootPath = "/sdcard/astator.script";
            this.RootPath.Tag = (rootPath, -1);
            this.pathTbs.Add(this.RootPath);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
                return;
            }
            ShowFiles(rootPath);
        }

        private void ShowFiles(string directory)
        {
            this.RootLayout.Children.Clear();
            var dirs = Directory.EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly);
            foreach (var dir in dirs)
            {
                var name = Path.GetFileName(dir);
                var info = $"{new DirectoryInfo(dir).LastWriteTime:yyyy/MM/dd HH:mm}";

                var card = new DirCard(dir, name, info);
                card.Tapped += Dir_Tapped;
                this.RootLayout.Children.Add(card);
            }

            var files = Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                var info = $"{new DirectoryInfo(file).LastWriteTime:yyyy/MM/dd HH:mm}";

                var icon = file.EndsWith(".cs") ? "_script" : file.EndsWith(".csproj") ? "_csproj" : file.EndsWith(".txt") ? "_txt" : file.EndsWith("xml") ? "_xml" : string.Empty;
                var card = new FileCard(file, name, info, $"Assets/Image/file{icon}.png");
                this.RootLayout.Children.Add(card);
            }
        }

        private void Dir_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var card = sender as DirCard;
            var directory = card.FullPath;
            var tb = new TextBlock
            {
                Text = Path.GetFileName(directory) + " ›",
                Tag = (directory, this.pathTbs.Count)
            };
            tb.Tapped += Path_Tapped;

            foreach (var pathTb in this.pathTbs)
            {
                pathTb.Foreground = new SolidColorBrush(Color.ParseColor("#666666"));
            }
            this.pathTbs.Add(tb);
            this.PathLayout.Add(tb);
            this.PathScrollView.ScrollToHorizontalOffset(int.MaxValue);
            ShowFiles(directory);
        }

        private void Path_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var tb = sender as TextBlock;
            var tag = ((string, int))tb.Tag;
            tb.Foreground = new SolidColorBrush(Color.ParseColor("#000000"));

            if (tag.Item2 != -1)
            {
                for (var i = tag.Item2 + 1; i < this.pathTbs.Count; i++)
                {
                    try
                    {
                        this.pathTbs.RemoveAt(i);
                        this.PathLayout.RemoveViewAt(i - 1);
                    }
                    catch { }
                }
            }
            else
            {
                try
                {
                    this.pathTbs.RemoveRange(1, this.pathTbs.Count - 1);
                }
                catch { }
                this.PathLayout.Children.Clear();
            }
            ShowFiles(tag.Item1.ToString());
        }

        public new bool OnKeyDown(Keycode keycode, KeyEvent e)
        {
            if (keycode == Keycode.Back)
            {
                if (this.pathTbs.Count > 1)
                {
                    Path_Tapped(this.pathTbs[^2], default);
                }
                else
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
