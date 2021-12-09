using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.Views.Accessibility;
using Action = System.Action;

namespace astator.Core.Accessibility
{
    [Service(Label = "astator", Enabled = true, Exported = true, Permission = "android.permission.BIND_ACCESSIBILITY_SERVICE")]
    [IntentFilter(new string[] { "android.accessibilityservice.AccessibilityService" })]
    [MetaData("android.accessibilityservice", Resource = "@xml/accessibilityservice")]
    public class ScriptAccessibilityService : AccessibilityService
    {
        public static ScriptAccessibilityService Instance { get; private set; }

        public static Action ConnectCallback { get; set; }

        public static Action DestroyCallback { get; set; }

        protected override void OnServiceConnected()
        {
            base.OnServiceConnected();
            Instance = this;

            ConnectCallback?.Invoke();
        }

        public override void OnAccessibilityEvent(AccessibilityEvent e)
        { }

        public override void OnInterrupt()
        {
            Instance = null;

            DestroyCallback?.Invoke();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null;

            DestroyCallback?.Invoke();
        }
    }
}
