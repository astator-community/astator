using Android.Content;
using AndroidX.Activity.Result;
using astator.Core.Script;

namespace astator.Modules;
internal static class PermissionHelperer
{
    private static PermissionHelper instance;
    public static PermissionHelper Instance { get => instance; set => instance = value; }

    public static void StartActivity(Intent intent)
    {
        Instance.StartActivity(intent);
    }

    public static void StartActivityForResult(Intent intent, Action<ActivityResult> callback)
    {
        Instance.StartActivityForResult(intent, callback);
    }

    public static void ReqScreenCap(bool isLandscape, Action<bool> callback)
    {
        Instance.ReqScreenCap(isLandscape, callback);
    }
    public static void CloseScreenCap()
    {
        Instance.CloseScreenCap();
    }

    public static bool CheckScreenCap()
    {
        return Instance.CheckScreenCap();
    }

    public static void ReqFloaty(Action<bool> callback)
    {
        Instance.ReqFloaty(callback);
    }

    public static bool CheckFloaty()
    {
        return Instance.CheckFloaty();
    }

    public static bool CheckAccessibility()
    {
        return Instance.CheckAccessibility();
    }

    public static void ReqAccessibility(Action<bool> callback)
    {
        Instance.ReqAccessibility(callback);
    }

    public static void CloseAccessibility()
    {
        Instance.CloseAccessibility();
    }

    public static void ReqPermission(string permission, Action<bool> callback)
    {
        Instance.ReqPermission(permission, callback);
    }

    public static bool IsIgnoringBatteryOptimizations()
    {
        return Instance.IsIgnoringBatteryOptimizations();
    }

    public static void IgnoringBatteryOptimizations(Action<bool> callback)
    {
        Instance.ReqIgnoringBatteryOptimizations(callback);
    }
}
