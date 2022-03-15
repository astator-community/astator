using Android.App;
using Android.Content;
using Android.Media.Projection;
using Android.OS;
using AndroidX.Activity.Result;
using astator.Core.Accessibility;
using astator.Core.Graphics;
using astator.Core.UI.Base;
using astator.Core.UI.Floaty;
using System;
using System.Threading.Tasks;

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
    public void ReqScreenCap(Action<bool> callback)
    {
        if (CheckScreenCap())
        {
            return;
        }

        var manager = (MediaProjectionManager)this.activity.GetSystemService("media_projection");
        var intent = manager.CreateScreenCaptureIntent();
        StartActivityForResult(intent,
        result =>
        {
            var isEnabled = result.ResultCode == (int)Result.Ok;
            if (isEnabled)
            {
                var intent = new Intent(this.activity, typeof(ScreenCapturer));
                intent.PutExtra("data", result.Data);
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
    public  bool CheckFloatyAsync()
    {
        if (Android.Provider.Settings.CanDrawOverlays(this.activity))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 申请悬浮窗权限
    /// </summary>
    public void ReqFloaty()
    {
        if (!Android.Provider.Settings.CanDrawOverlays(this.activity))
        {
            var intent = new Intent(Android.Provider.Settings.ActionManageOverlayPermission, Android.Net.Uri.Parse("package:" + this.activity.PackageName));
            StartActivity(intent);
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
    /// 跳转到系统无障碍服务界面
    /// </summary>
    public void ReqAccessibility()
    {
        var intent = new Intent(Android.Provider.Settings.ActionAccessibilitySettings);
        intent.SetFlags(ActivityFlags.NewTask);
        this.activity.StartActivity(intent);
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
        var powerManager = (PowerManager)this.activity.GetSystemService("power");
        return powerManager.IsIgnoringBatteryOptimizations(this.activity.PackageName);
    }

    /// <summary>
    /// 忽略电池优化
    /// </summary>
    public void IgnoringBatteryOptimizations()
    {
        var isIgnored = IsIgnoringBatteryOptimizations();
        if (!isIgnored)
        {
            var intent = new Intent("android.settings.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS");
            intent.SetData(Android.Net.Uri.Parse($"package:{this.activity.PackageName}"));
            this.activity.StartActivity(intent);
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

