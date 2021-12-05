using Android.AccessibilityServices;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astator.Core.Accessibility
{
    public static class Automator
    {
        private static ScriptAccessibilityService Service {get=> ScriptAccessibilityService.Instance;}

        public static void Click(int x, int y, int duration = 1)
        {
            var  path = new Path();
            path.MoveTo(x, y);
            var gesture = new GestureDescription.Builder().AddStroke(new GestureDescription.StrokeDescription(path, 0L, duration)).Build();
            if (gesture != null)
            {
                Service?.DispatchGesture(gesture, null, null);
            }

        }

        public static void Swipe(int startX, int startY, int endX, int endY, int duration)
        {
            var path = new Path();
            path.MoveTo(startX, startY);
            path.LineTo(endX, endY);
            var gesture = new GestureDescription.Builder().AddStroke(new GestureDescription.StrokeDescription(path, 0L, duration)).Build();
            if (gesture is not null)
            {
                Service?.DispatchGesture(gesture, null, null);
            }
        }
    }
}
