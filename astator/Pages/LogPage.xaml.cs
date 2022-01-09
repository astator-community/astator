using AndroidX.AppCompat.App;
using astator.Core;
using NLog;

namespace astator.Pages
{
    public partial class LogPage : ContentPage
    {
        private ScriptLogger logger;
        public LogPage()
        {
            InitializeComponent();

            this.logger = ScriptLogger.Instance;
            this.logger.AddCallback("logPage", (value) => AddLogText(value));

            InitLogList();
        }

        private void InitLogList()
        {
            var path = Path.Combine(MauiApplication.Current.GetExternalFilesDir("Log").ToString(), "log.txt");
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
                        this.LogLayout.Add(label);
                    }
                    File.WriteAllLines(path, logList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                this.LogScrollView.ScrollToAsync(0, this.LogLayout.Height, false);
            }
        }
        public void AddLogText(LogArgs message)
        {
            _ = Device.InvokeOnMainThreadAsync(() =>
            {
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
                this.LogLayout.Add(label);
                this.LogScrollView.ScrollToAsync(0, this.LogLayout.Height, false);
            });
        }
        public void Delete_Clicked(object sender, EventArgs e)
        {

            var alert = new AlertDialog
                .Builder(Globals.AppContext)
                .SetTitle("清空日志")
                .SetMessage("确认清空吗?")
                .SetPositiveButton("确认", (s, e) =>
                {
                    this.LogLayout.Clear();
                    var path = Path.Combine(MauiApplication.Current.GetExternalFilesDir("Log").ToString(), "log.txt");
                    File.WriteAllText(path, string.Empty);
                })
                .SetNegativeButton("取消", (s, e) =>
                {

                });

            alert.Show();

        }

        private async void LogLayout_ChildAdded(object sender, EventArgs e)
        {
            await this.LogScrollView.ScrollToAsync(this.LogScrollView, ScrollToPosition.End, true);
        }
    }
}