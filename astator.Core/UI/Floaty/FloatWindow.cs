using Android.Graphics;
using Android.OS;
using Android.Views;
using static Android.Views.ViewGroup;
namespace astator.Core.UI.Floaty
{
    public class FloatWindow
    {
        private readonly View view;
        private bool showed = false;
        public FloatWindow(View view, int x = 0, int y = 0)
        {
            this.view = view;
            var layoutParams = new WindowManagerLayoutParams();
            if ((int)Build.VERSION.SdkInt >= 26)
            {
                layoutParams.Type = WindowManagerTypes.ApplicationOverlay;
            }
            else
            {
                layoutParams.Type = WindowManagerTypes.Phone;
            }
            layoutParams.Format = Format.Transparent;
            layoutParams.Gravity = GravityFlags.Left | GravityFlags.Top;
            layoutParams.Flags = WindowManagerFlags.NotFocusable;
            layoutParams.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
            layoutParams.Width = LayoutParams.WrapContent;
            layoutParams.Height = LayoutParams.WrapContent;
            layoutParams.X = x;
            layoutParams.Y = y;
            FloatyService.Instance.AddView(view, layoutParams);
            this.showed = true;
        }
        public void SetPosition(int x, int y)
        {
            if (this.view.LayoutParameters is WindowManagerLayoutParams layoutParams)
            {
                layoutParams.X = x;
                layoutParams.Y = y;
                FloatyService.Instance.UpdateViewLayout(this.view, layoutParams);
            }

        }
        public Point GetPosition()
        {
            if (this.view.LayoutParameters is WindowManagerLayoutParams layoutParams)
            {
                return new Point(layoutParams.X, layoutParams.Y);
            }
            return new Point(-1, -1);
        }
        public bool Hide()
        {
            if (this.showed)
            {
                this.showed = false;
                FloatyService.Instance.RemoveView(this.view);
                return true;
            }
            else
            {
                return false;
            }
        }
        //public bool OnTouchCallBack(View v, MotionEvent e)
        //{
        //    //v.Parent.RequestDisallowInterceptTouchEvent(true);
        //    if (e.Action == MotionEventActions.Down)
        //    {
        //        this._x = e.RawX;
        //        this._y = e.RawY;
        //    }
        //    else if (e.Action == MotionEventActions.Move)
        //    {
        //        var offsetX = (int)(e.RawX - this._x);
        //        var offsetY = (int)(e.RawY - this._y);
        //        if (!this._moving)
        //        {
        //            if (Math.Abs(offsetX) < 25 && Math.Abs(offsetY) < 25)
        //            {
        //                return true;
        //            }
        //        }
        //        this._moving = true;
        //        this._x = e.RawX;
        //        this._y = e.RawY;
        //        this._layoutParams.X += offsetX;
        //        this._layoutParams.Y += offsetY;
        //        FloatyService.Instance.UpdateViewLayout(v, this._layoutParams);
        //    }
        //    else if (e.Action == MotionEventActions.Up)
        //    {
        //        var num = DeviceArgs.Width;
        //        if (this._moving)
        //        {
        //            if (this._layoutParams.X < num / 2)
        //            {
        //                this._layoutParams.X = 0;
        //            }
        //            else
        //            {
        //                this._layoutParams.X = num - this._layoutParams.Width;
        //            }
        //            FloatyService.Instance.UpdateViewLayout(v, this._layoutParams);
        //        }
        //        this._moving = false;
        //    }
        //    return true;
        //}
    }
}
