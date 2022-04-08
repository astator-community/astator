using System;
using Android.App;
using Android.Content;
using Android.Media.Projection;
using Android.OS;
using Android.Provider;
using AndroidX.Activity.Result;
using astator.Core.Accessibility;
using astator.Core.Graphics;
using astator.Core.UI.Base;

namespace astator.Core.Script;


/// <summary>
/// 权限相关
/// </summary>
public class PermissionHelper
{
    private readonly Activity activity;
    private readonly LifecycleObserver lifecycleObserver;

    public PermissionHelper(Activity activity)
    {
        this.activity = activity;
        this.lifecycleObserver = (activity as IActivity).LifecycleObserver;
    }

    /// <summary>
    /// 启动一个activity
    /// </summary>
    /// <param name="intent">意图</param>
    public void StartActivity(Intent intent)
    {
        this.activity.StartActivity(intent);
    }

    /// <summary>
    /// 启动一个activity并获取回传数据
    /// </summary>
    /// <param name="intent">意图</param>
    public void StartActivityForResult(Intent intent, Action<ActivityResult> callback)
    {
        this.lifecycleObserver.StartActivityForResult(intent, callback);
    }

    /// <summary>
    /// 检查截图服务是否开启
    /// </summary>
    /// <returns></returns>
    public bool CheckScreenCap()
    {
        return ScreenCapturer.Instance is not null;
    }

    /// <summary>
    /// 申请截图权限
    /// </summary>
    public void ReqScreenCap(bool isLandscape, Action<bool> callback)
    {
        if (CheckScreenCap())
        {
            ScreenCapturer.Instance.ResetOrientation(isLandscape);
            callback?.Invoke(true);
            return;
        }

        var manager = (MediaProjectionManager)this.activity.GetSystemService(Context.MediaProjectionService);
        var intent = manager.CreateScreenCaptureIntent();
        StartActivityForResult(intent,
        result =>
        {
            var isEnabled = result.ResultCode == (int)Result.Ok;
            if (isEnabled)
            {
                var intent = new Intent(this.activity, typeof(ScreenCapturer));
                intent.PutExtra("data", result.Data);
                intent.PutExtra("orientation", isLandscape);
                if (OperatingSystem.IsAndroidVersionAtLeast(26))
                {
                    this.activity.StartForegroundService(intent);
                }
                else
                {
                    this.activity.StartService(intent);
                }
            }
            callback?.Invoke(isEnabled);
        });
    }

    /// <summary>
    /// 关闭截图服务
    /// </summary>
    public void CloseScreenCap()
    {
        ScreenCapturer.Instance?.Dispose();
    }

    /// <summary>
    /// 检查悬浮窗权限
    /// </summary>
    /// <returns></returns>
    public bool CheckFloaty()
    {
        return CheckPermission(AppOpsManager.OpstrSystemAlertWindow);
    }

    /// <summary>
    /// 申请悬浮窗权限
    /// </summary>
    public void ReqFloaty(Action<bool> callback)
    {
        if (!CheckFloaty())
        {
            var intent = new Intent(Settings.ActionManageOverlayPermission, Android.Net.Uri.Parse("package:" + this.activity.PackageName));
            StartActivityForResult(intent, (_) =>
            {
                callback.Invoke(CheckFloaty());
            });
        }
    }

    /// <summary>
    /// 检查无障碍服务是否开启
    /// </summary>
    /// <returns></returns>
    public bool CheckAccessibility()
    {
        return ScriptAccessibilityService.Instance is not null;
    }

    /// <summary>
    /// 启动无障碍服务
    /// </summary>
    public void ReqAccessibility(Action<bool> callback)
    {
        try
        {
            var enableds = $"{Settings.Secure.GetString(Application.Context.ContentResolver, Settings.Secure.EnabledAccessibilityServices)}:" ?? string.Empty;
            var req = $"{enableds}{Globals.AppContext.PackageName}/{Java.Lang.Class.FromType(typeof(ScriptAccessibilityService)).CanonicalName}";
            if (!enableds.Contains(req))
            {
                Settings.Secure.PutString(Globals.AppContext.ContentResolver, Settings.Secure.EnabledAccessibilityServices, req);
                Settings.Secure.PutInt(Globals.AppContext.ContentResolver, Settings.Secure.AccessibilityEnabled, 1);
            }
            callback.Invoke(true);
        }
        catch
        {
            var intent = new Intent(Settings.ActionAccessibilitySettings);
            StartActivityForResult(intent, (_) =>
            {
                callback.Invoke(CheckAccessibility());
            });
        }

    }

    /// <summary>
    /// 关闭无障碍服务
    /// </summary>
    public void CloseAccessibility()
    {
        ScriptAccessibilityService.Instance?.DisableSelf();
    }

    /// <summary>
    /// 获取是否已忽略电池优化
    /// </summary>
    /// <returns></returns>
    public bool IsIgnoringBatteryOptimizations()
    {
        var powerManager = (PowerManager)this.activity.GetSystemService(Context.PowerService);
        return powerManager.IsIgnoringBatteryOptimizations(this.activity.PackageName);
    }

    /// <summary>
    /// 忽略电池优化
    /// </summary>
    public void IgnoringBatteryOptimizations(Action<bool> callback)
    {
        var isIgnored = IsIgnoringBatteryOptimizations();
        if (!isIgnored)
        {
            var intent = new Intent("android.settings.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS");
            intent.SetData(Android.Net.Uri.Parse($"package:{this.activity.PackageName}"));
            StartActivityForResult(intent, (_) =>
            {
                callback.Invoke(IsIgnoringBatteryOptimizations());
            });
        }
    }

    /// <summary>
    /// 检查使用情况统计权限是否开启
    /// </summary>
    /// <returns></returns>
    public bool CheckUsageStats()
    {
        return CheckPermission(AppOpsManager.OpstrGetUsageStats);
    }

    /// <summary>
    /// 申请使用情况统计权限
    /// </summary>
    public void ReqUsageStats(Action<bool> callback)
    {
        if (!CheckUsageStats())
        {
            var intent = new Intent(Android.Provider.Settings.ActionUsageAccessSettings);
            StartActivityForResult(intent, (_) =>
            {
                callback.Invoke(CheckUsageStats());
            });
        }
    }

    /// <summary>
    /// 检查权限是否允许
    /// </summary>
    /// <param name="permission">查阅https://developer.android.google.cn/reference/android/app/AppOpsManager?hl=en#constants_1</param>
    /// <returns></returns>
    public bool CheckPermission(string permission)
    {
        var appOpsManager = (AppOpsManager)this.activity.GetSystemService(Context.AppOpsService);
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            return appOpsManager.UnsafeCheckOpNoThrow(permission, Process.MyUid(), this.activity.PackageName) == AppOpsManagerMode.Allowed;
        }
        else
        {
            return appOpsManager.CheckOpNoThrow(permission, Process.MyUid(), this.activity.PackageName) == AppOpsManagerMode.Allowed;
        }
    }

    /// <summary>
    /// 动态权限申请
    /// </summary>
    /// <param name="permission">权限值</param>
    /// <param name="callback">申请结果回调</param>
    public void ReqPermission(string permission, Action<bool> callback)
    {
        this.lifecycleObserver?.ReqPermission(permission, callback);
    }
}

