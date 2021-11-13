using astator.Core;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Color = Microsoft.Maui.Graphics.Color;

namespace astator.Pages
{
    public partial class LogPage : ContentPage
    {
        ScriptLogger logger;
        public LogPage()
        {
            InitializeComponent();
            this.NavBar.ActiveTab = "log";
            this.logger = ScriptLogger.Instance;
            this.logger.Actions.Add((value) => AddLogText(value));
            InitLogList();
        }
        private void InitLogList()
        {
            var path = Path.Combine(MauiApplication.Current.FilesDir.AbsolutePath, "Log", "log.txt");
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

                        var label = new Label
                        {
                            Text = $"{message[1]} {message[2].Trim(':')}"
                        };
                        if (Level == LogLevel.Warn)
                        {
                            label.TextColor = Color.FromRgb(0xf0, 0xdc, 0x0c);
                        }
                        else if (Level == LogLevel.Error || Level == LogLevel.Fatal)
                        {
                            label.TextColor = Colors.Red;
                        }
                        this.LogLayout.Children.Add(label);
                        this.LogScrollView.ScrollToAsync(0, this.LogLayout.Height, false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                File.WriteAllLines(path, logList);
            }
        }
        public void AddLogText(LogArgs message)
        {
            Device.InvokeOnMainThreadAsync(async() => {
                var label = new Label
                {
                    Text = $"{message.Time:MM-dd HH:mm:ss.fff}  {message.Message}"
                };
                if (message.Level == LogLevel.Warn)
                {
                    label.TextColor = Color.FromRgb(0xf0, 0xdc, 0x0c);
                }
                else if (message.Level == LogLevel.Error || message.Level == LogLevel.Fatal)
                {
                    label.TextColor = Colors.Red;
                }
                this.LogLayout.Children.Add(label);
                await this.LogScrollView.ScrollToAsync(0, this.LogLayout.Height, false);
            });
        }
        public async void Delete_Clicked(object sender, EventArgs e)
        {


           // this.logger.Error("test");
           //Manager.Instance.Initialize("/sdcard/Download/Examples/test-1.csproj");
           //if (await DisplayAlert("清空日志", "确认清空吗?", "确认", "取消"))
           //{
           //    this.LogLayout.Children.Clear();
           //}
        }

        private async void LogLayout_ChildAdded(object sender, ElementEventArgs e)
        {
            await this.LogScrollView.ScrollToAsync(0, this.LogLayout.Height, false);
        }
    }
}