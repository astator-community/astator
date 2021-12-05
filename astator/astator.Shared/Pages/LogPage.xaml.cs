
using Android.Graphics.Drawables;
using Android.Views;
using AndroidX.AppCompat.App;
using astator.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI;

/* 项目“astator.Mobile (net6.0-android)”的未合并的更改
在此之前:
using Windows.UI.Popups;
using astator.Core;
using System.Linq;
在此之后:
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Linq;
*/
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace astator.Pages
{
    public sealed partial class LogPage : UserControl
    {
        ScriptLogger logger = ScriptLogger.Instance;
        public LogPage()
        {
            InitializeComponent();

            Console.SetOut(new ScriptConsole());

            this.logger.Callbacks.Add("logPage", (value) => AddLogText(value));
            InitLogList();
        }



        private void InitLogList()
        {
            var path = Path.Combine(Android.App.Application.Context.GetExternalFilesDir("Log").ToString(), "log.txt");
            var logList = new List<string>();
            if (File.Exists(path))
            {
                var lines = File.ReadLines(path);
                var maxLen = lines.Count();
                try
                {
                    for (var i = maxLen > 100 ? maxLen - 100 : 0; i < maxLen; i++)
                    {
                        var line = lines.ElementAt(i);
                        logList.Add(line);
                        var message = line.Split("*/");
                        var Level = LogLevel.FromString(message[0]) ?? LogLevel.Debug;

                        var label = new TextBlock
                        {
                            Text = $"{message[1]} {message[2].Trim(':')}"
                        };
                        if (Level == LogLevel.Warn)
                        {
                            label.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0xf0, 0xdc, 0x0c));
                        }
                        else if (Level == LogLevel.Error || Level == LogLevel.Fatal)
                        {
                            label.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        else
                        {
                            label.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0x66, 0x66, 0x66));
                        }
                        this.LogLayout.Add(label);
                    }
                    File.WriteAllLines(path, logList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                //this.PathScrollView.ScrollToVerticallOffset(int.MaxValue);
            }
        }
        public void AddLogText(LogArgs message)
        {
            Globals.RunOnUiThread(() =>
            {
                var label = new TextBlock
                {
                    Text = $"{message.Time:MM-dd HH:mm:ss.fff}  {message.Message}"
                };
                if (message.Level == LogLevel.Warn)
                {
                    label.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0xf0, 0xdc, 0x0c));
                }
                else if (message.Level == LogLevel.Error || message.Level == LogLevel.Fatal)
                {
                    label.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    label.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0x66, 0x66, 0x66));
                }
                this.LogLayout.Add(label);
                //this.PathScrollView.ScrollToVerticallOffset(int.MaxValue);
            });
        }
        //public async void Delete_Clicked(object sender, EventArgs e)
        //{
        //    if (await DisplayAlert("清空日志", "确认清空吗?", "确认", "取消"))
        //    {
        //        var path = Path.Combine(MauiApplication.Current.GetExternalFilesDir("Log").ToString(), "log.txt");
        //        this.LogLayout.Clear();
        //        File.WriteAllText(path, string.Empty);
        //    }
        //}



        public new bool OnKeyDown(Keycode keycode, KeyEvent e)
        {
            return false;
        }

        private async void Remove_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dialog = new AlertDialog.Builder(MainActivity.Instance)
                .SetTitle("提示")
                .SetMessage("确定要清空日志吗?")
                .SetPositiveButton("确定", (sender, e) =>
                {
                    this.LogLayout.Children.Clear();
                    var path = Path.Combine(Android.App.Application.Context.GetExternalFilesDir("Log").ToString(), "log.txt");
                    File.WriteAllText(path, string.Empty);
                })
                .SetNegativeButton("取消", (sender, e) => { })
                .Create();

            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.FromArgb(0xff, 0xf0, 0xf3, 0xf6)));
            dialog.Show();
        }
    }
}
