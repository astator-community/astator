using Android.Views;
using astator.Core.Script;
using astator.Core.UI.Base;
using astator.Core.UI.Floaty;
using astator.LoggerProvider;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using LogLevel = astator.LoggerProvider.LogLevel;

namespace astator.Core;

public partial class ConsoleFloaty : Grid
{
    public static async Task<ConsoleFloaty> CreateAsync(string title, int width, int height, int x, int y, GravityFlags gravity,
        WindowManagerFlags flags)
    {
        var result = await Globals.InvokeOnMainThreadAsync(() =>
        {
            var consoleFloaty = new ConsoleFloaty(title, width, height);
            var view = consoleFloaty.ToPlatform(Application.Current.MainPage.Handler.MauiContext);
            var floaty = new SystemFloatyWindow(Globals.AppContext, view, x, y, gravity, flags);
            consoleFloaty.floaty = floaty;
            return consoleFloaty;
        });
        return result;
    }

    private SystemFloatyWindow floaty;
    private readonly string logKey = string.Empty;
    private ConcurrentDictionary<string, Color> logLevelColors = new()
    {
        ["trace"] = Color.Parse("#4a4a4d"),
        ["debug"] = Color.Parse("#4a4a4d"),
        ["info"] = Color.Parse("#4a4a4d"),
        ["wran"] = Color.Parse("#f0dc0c"),
        ["error"] = Colors.Red,
        ["fatal"] = Colors.Red,
    };

    public ConsoleFloaty(string title, int width, int height)
    {
        InitializeComponent();
        this.WidthRequest = width;
        this.HeightRequest = height;
        this.Title.Text = title;

        width = Math.Min(width, (int)(Devices.Width / Devices.Dp));
        height = Math.Min(height, (int)(Devices.Height / Devices.Dp));

        this.RowDefinitions = new RowDefinitionCollection
        {
            new RowDefinition { Height = 50 },
            new RowDefinition { Height = height - 100 },
            new RowDefinition { Height = 50 },
        };
        this.ColumnDefinitions = new ColumnDefinitionCollection
        {
            new ColumnDefinition { Width = width },
        };
        this.InputGrid.ColumnDefinitions = new ColumnDefinitionCollection
        {
            new ColumnDefinition { Width = width - 80 },
            new ColumnDefinition { Width =  60 },
        };

        this.logKey = AstatorLogger.AddCallback("logPage", AddLogText);
    }

    public void AddLogText(LogLevel logLevel, DateTime time, string msg)
    {
        Globals.InvokeOnMainThreadAsync(async () =>
        {
            var label = new Label
            {
                Text = $"{time:MM-dd HH:mm:ss.fff}  {msg}"
            };

            label.TextColor = logLevel switch
            {
                LogLevel.Trace => this.logLevelColors["trace"],
                LogLevel.Debug => this.logLevelColors["debug"],
                LogLevel.Info => this.logLevelColors["info"],
                LogLevel.Warn => this.logLevelColors["wran"],
                LogLevel.Error => this.logLevelColors["error"],
                LogLevel.Fatal => this.logLevelColors["fatal"],
                _ => this.logLevelColors["fatal"]
            };

            this.OutputLayout.Add(label);
            await Task.Delay(20);
            _ = this.OutputScrollView.ScrollToAsync(0, this.OutputLayout.Height, false);
        });
    }
    public void Close() => Globals.InvokeOnMainThreadAsync(() =>
    {
        AstatorLogger.RemoveCallback(this.logKey);
        this.floaty?.Remove();
    });

    public void Hide() => Globals.InvokeOnMainThreadAsync(() => this.floaty?.Hide());
    public void ClearOutput() => Globals.InvokeOnMainThreadAsync(() => this.OutputLayout.Clear());
    public void SetTitle(string title) => Globals.InvokeOnMainThreadAsync(() => this.Title.Text = title);
    public void SetLogLevelColors(string trace = "#4a4a4d", string debug = "#4a4a4d", string info = "#4a4a4d",
        string wran = "#f0dc0c", string error = "red", string fatal = "red")
    {
        this.logLevelColors.TryUpdate("trace", Color.Parse(trace), this.logLevelColors["trace"]);
        this.logLevelColors.TryUpdate("debug", Color.Parse(debug), this.logLevelColors["debug"]);
        this.logLevelColors.TryUpdate("info", Color.Parse(info), this.logLevelColors["info"]);
        this.logLevelColors.TryUpdate("wran", Color.Parse(wran), this.logLevelColors["wran"]);
        this.logLevelColors.TryUpdate("error", Color.Parse(error), this.logLevelColors["error"]);
        this.logLevelColors.TryUpdate("fatal", Color.Parse(fatal), this.logLevelColors["fatal"]);
    }

    private void Send_Clicked(object sender, EventArgs e)
    {
        Script.Console.SendInput(this.Input.Text);
        this.Input.Text = string.Empty;
    }

    private void Input_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(this.Input.Text))
            this.Send.IsEnabled = false;
        else
            this.Send.IsEnabled = true;
    }
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var view = this.Handler.PlatformView as LayoutViewGroup;
        view.ClipToOutline = true;
        view.OutlineProvider = new RadiusOutlineProvider(30);
    }
}