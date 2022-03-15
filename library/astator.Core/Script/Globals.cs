using Android.Content;
using Android.Widget;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Application = Android.App.Application;

namespace astator.Core.Script;

public class Globals
{

#if DEBUG
    public const string AstatorPackageName = "com.debug.astator";
#elif RELEASE
    public const string AstatorPackageName = "com.astator.astator";
#endif
    public static Context AppContext { get; set; }

    /// <summary>
    /// 给用户展示简短消息的视图
    /// </summary>
    /// <param name="text">内容</param>
    /// <param name="duration">持续时间, 默认ToastLength.Short</param>
    public static void Toast(string text, ToastLength duration = ToastLength.Short)
    {
        InvokeOnMainThreadAsync(() => Android.Widget.Toast.MakeText(Application.Context, text, duration).Show());
    }


    /// <summary>
    /// 在ui线程执行action
    /// </summary>
    public static Task InvokeOnMainThreadAsync(Action action)
    {
        return Device.InvokeOnMainThreadAsync(action);
    }

    /// <summary>
    /// 在ui线程执行func
    /// </summary>
    public static Task<T> InvokeOnMainThreadAsync<T>(Func<T> func)
    {
        return Device.InvokeOnMainThreadAsync(func);
    }

    /// <summary>
    /// 在ui线程执行funcTask
    /// </summary>
    public static Task InvokeOnMainThreadAsync(Func<Task> funcTask)
    {
        return Device.InvokeOnMainThreadAsync(funcTask);
    }

    /// <summary>
    /// 在ui线程执行funcTask
    /// </summary>
    public static Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask)
    {
        return Device.InvokeOnMainThreadAsync(funcTask);
    }
}
