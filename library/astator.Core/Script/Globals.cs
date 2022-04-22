using Android.App.Usage;
using Android.Content;
using Android.Widget;
using astator.LoggerProvider;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
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
        var beginTime = endTime - 60000 * 60 * 12;
        var usageStatsList = usageStatsManager.QueryUsageStats(UsageStatsInterval.Best, beginTime, endTime);

        UsageStats usageStats = null;
        if (usageStatsList.Any())
        {
            foreach (var stats in usageStatsList)
            {
                if (usageStats is null
                    || usageStats.LastTimeUsed < stats.LastTimeUsed)
                {
                    usageStats = stats;
                }
            }
        }
        if (usageStats is null) Logger.Error("获取前台应用包名失败, 请检查使用情况访问权限是否打开!");

        return usageStats?.PackageName ?? null;
    }

    /// <summary>
    /// 启动其他应用
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

    /// <summary>
    /// 安装应用
    /// </summary>
    public static void InstallApp(string path)
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            if (!AppContext.PackageManager.CanRequestPackageInstalls())
            {
                AppContext.StartActivity(new Intent(Android.Provider.Settings.ActionManageUnknownAppSources));
                return;
            }
        }

        var intent = new Intent(Intent.ActionView);
        intent.AddFlags(ActivityFlags.NewTask);
        var uri = AndroidX.Core.Content.FileProvider.GetUriForFile(AppContext, AppContext.PackageName + ".fileProvider", new Java.IO.File(path));
        intent.AddFlags(ActivityFlags.GrantReadUriPermission);
        intent.SetDataAndType(uri, "application/vnd.android.package-archive");
        AppContext.StartActivity(intent);
    }
}
