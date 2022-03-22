using Android.App.Usage;
using Android.Content;
using Android.Widget;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
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

    /// <summary>
    /// 通过使用情况访问权限获取前台应用包名
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentPackageName()
    {
        var usageStatsManager = (UsageStatsManager)AppContext.GetSystemService("usagestats");
        var endTime = Java.Lang.JavaSystem.CurrentTimeMillis();
        var beginTime = endTime - 10 * 60000;
        var usageEvents = usageStatsManager.QueryEvents(beginTime, endTime);

        UsageEvents.Event latestEvent = null;
        while (usageEvents.HasNextEvent)
        {
            var _event = new UsageEvents.Event();
            usageEvents.GetNextEvent(_event);
            if (_event.EventType == UsageEventType.MoveToForeground)
            {
                latestEvent = _event;
            }
        }
        return latestEvent?.PackageName ?? null;
    }

    /// <summary>
    /// 启动其他app
    /// </summary>
    /// <param name="pkgName">包名</param>
    /// <returns></returns>
    public static bool LaunchApp(string pkgName)
    {
        var intent = AppContext.PackageManager.GetLaunchIntentForPackage(pkgName);
        if (intent is null) return false;
        intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ResetTaskIfNeeded);
        AppContext.StartActivity(intent);
        return true;
    }

    /// <summary>
    /// 获取已安装应用的信息
    /// </summary>
    /// <returns></returns>
    public static IList<Android.Content.PM.PackageInfo> GetInstalledPackages()
    {
        return AppContext.PackageManager.GetInstalledPackages(Android.Content.PM.PackageInfoFlags.MatchAll);
    }
}
