using Android.Views;
using System.Threading;
using System.Threading.Tasks;

namespace astator.Core.Script;
public static class Console
{
    private static ConsoleFloaty consoleFloaty;

    /// <summary>
    /// 启动控制台悬浮窗
    /// </summary>
    /// <param name="title">控制台标题</param>
    /// <param name="width">控制台宽度</param>
    /// <param name="height">控制台高度</param>
    /// <returns></returns>
    public static async Task ShowAsync(string title = "控制台", int width = 300, int height = 500, int x = 0, int y = 0, GravityFlags gravity = GravityFlags.Left | GravityFlags.Top,
        WindowManagerFlags flags = WindowManagerFlags.LayoutNoLimits | WindowManagerFlags.NotTouchModal)
    {
        Close();
        consoleFloaty = await ConsoleFloaty.CreateAsync(title, width, height, x, y, gravity, flags);
    }

    public static void Close() => consoleFloaty?.Close();
    public static void Hide() => consoleFloaty?.Hide();
    public static void Clear() => consoleFloaty?.ClearOutput();
    public static void SetTitle(string title) => consoleFloaty?.SetTitle(title);

    private static bool HasInput = false;
    private static string InputValue = string.Empty;

    /// <summary>
    /// 读取用户输入
    /// </summary>
    /// <param name="timeout">超时时间</param>
    /// <returns></returns>
    public static async Task<string> ReadInputAsync(int timeout)
    {
        if (consoleFloaty is null) return string.Empty;
        var source = new CancellationTokenSource(timeout);
        var token = source.Token;
        HasInput = false;
        return await Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(500);
                if (token.IsCancellationRequested) return string.Empty;
                if (HasInput) return InputValue;
            }
        }, token);
    }

    internal static void SendInput(string value)
    {
        InputValue = value;
        HasInput = true;
        Logger.Warn(value);
    }

    public static void Write(string value) => Logger.Trace(value);
    public static void Write(params object[] items) => Logger.Trace(items);
    public static void WriteLine(string value) => Logger.Trace(value);
    public static void WriteLine(params object[] items) => Logger.Trace(items);
    public static void Log(string value) => Logger.Log(value);
    public static void Log(params object[] items) => Logger.Log(items);
    public static void Trace(string value) => Logger.Trace(value);
    public static void Trace(params object[] items) => Logger.Trace(items);
    public static void Debug(string value) => Logger.Debug(value);
    public static void Debug(params object[] items) => Logger.Debug(items);
    public static void Warn(string value) => Logger.Warn(value);
    public static void Warn(params object[] items) => Logger.Warn(items);
    public static void Error(string value) => Logger.Error(value);
    public static void Error(params object[] items) => Logger.Error(items);
    public static void Fatal(string value) => Logger.Fatal(value);
    public static void Fatal(params object[] items) => Logger.Fatal(items);
}
