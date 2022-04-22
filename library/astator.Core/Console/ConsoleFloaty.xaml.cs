using Android.Views;
using astator.Core.Script;
using astator.Core.UI.Base;
using astator.Core.UI.Floaty;
using astator.LoggerProvider;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using System;
using Microsoft.Maui.Graphics;
using System.Threading.Tasks;

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
    private string logKey = string.Empty;

    public ConsoleFloaty(string title, int width, int height)
    {
        InitializeComponent();
        this.WidthRequest = width;
        this.HeightRequest = height;
        this.Title.Text = title;
        this.RowDefinitions = new RowDefinitionCollection
        {
            new RowDefinition { Height = 55 },
            new RowDefinition { Height = height - 105 },
            new RowDefinition { Height = 50 },
        };
        this.ColumnDefinitions = new ColumnDefinitionCollection
        {
            new ColumnDefinition { Width = width },
        };
        this.InputGrid.ColumnDefinitions = new ColumnDefinitionCollection
        {
            new ColumnDefinition { Width = width - 100 },
            new ColumnDefinition { Width =  100 },
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
            if (logLevel >= LogLevel.Error)
            {
                label.TextColor = Colors.Red;
            }
            else if (logLevel >= LogLevel.Warn)
            {
                label.TextColor = Color.FromRgb(0xf0, 0xdc, 0x0c);
            }
            this.OutputLayout.Add(label);
            await Task.Delay(50);
            _ = this.OutputScrollView.ScrollToAsync(0, this.OutputLayout.Height, false);
        });
    }
    public void Close() => Globals.InvokeOnMainThreadAsync(() =>
    {
        AstatorLogger.RemoveCallback(logKey);
        this.floaty?.Remove();
    });

    public void Hide() => Globals.InvokeOnMainThreadAsync(() => this.floaty?.Hide());
    public void ClearOutput() => Globals.InvokeOnMainThreadAsync(() => this.OutputLayout.Clear());
    public void SetTitle(string title) => Globals.InvokeOnMainThreadAsync(() => this.Title.Text = title);

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