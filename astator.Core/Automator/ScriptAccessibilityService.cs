using Android.AccessibilityServices;
using Android.App;
using Android.Views.Accessibility;
using Path = Android.Graphics.Path;

namespace astator.Core.Automator
{
    [Service(Label = "astator", Enabled = true, Exported = true, Permission = "android.permission.BIND_ACCESSIBILITY_SERVICE")]
    [IntentFilter(new string[] { "android.accessibilityservice.AccessibilityService" })]
    [MetaData("android.accessibilityservice", Resource = "@xml/accessibilityservice")]
    class ScriptAccessibilityService : AccessibilityService
    {
        public static AccessibilityService Instance { get; private set; }
        public static bool Inited { get => Instance is not null; }
        public void Click(int x, int y, int duration = 1)
        {
            Path path = new();
            path.MoveTo(x, y);
            var gesture = new GestureDescription.Builder()?.AddStroke(new GestureDescription.StrokeDescription(path, 0L, duration))?.Build();
            if (gesture != null)
            {
                DispatchGesture(gesture, null, null);
            }

        }
        public void Swipe(int startX, int startY, int endX, int endY, int duration)
        {
            Path path = new();
            path.MoveTo(startX, startY);
            path.LineTo(endX, endY);
            var gesture = new GestureDescription.Builder()?.AddStroke(new GestureDescription.StrokeDescription(path, 0L, duration))?.Build();
            if (gesture is not null)
            {
                DispatchGesture(gesture, null, null);
            }
        }
        protected override void OnServiceConnected()
        {
            base.OnServiceConnected();
            Instance = this;
        }
        public override void OnAccessibilityEvent(AccessibilityEvent e) { }
        public override void OnInterrupt() { }
    }
}
