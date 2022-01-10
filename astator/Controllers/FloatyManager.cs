using Android.Content;
using Android.Views;
using astator.Core;
using astator.Core.Broadcast;
using astator.Core.UI;
using astator.Core.UI.Floaty;
using astator.Views;
using Microsoft.Maui.Platform;
using View = Android.Views.View;

namespace astator.Controllers
{
    public class FloatyManager
    {
        private static FloatyManager instance;
        public static FloatyManager Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = new FloatyManager();
                }
                return instance;
            }
        }

        private readonly Core.UI.Floaty.FloatyManager floatyManager;

        private readonly List<FloatWindow> floatys = new();

        public FloatyManager()
        {
            this.floatyManager = new Core.UI.Floaty.FloatyManager(Globals.AppContext, default);

        }

        public async void Show()
        {
            if (!Globals.Permission.CheckFloaty())
            {
                Globals.Permission.ReqFloaty();
            }
            else
            {
                await Task.Run(() =>
                {
                    while (FloatyService.Instance is null)
                    {
                        Thread.Sleep(50);
                    }
                });

                var layout = new GridLayout
                {
                    WidthRequest = 42,
                    HeightRequest = 42,
                    BackgroundColor = Colors.Transparent
                };

                layout.Add(new CustomImage
                {
                    Source = "appicon.png",
                    IsCircle = true,
                    WidthRequest = 42,
                    HeightRequest = 42,

                });

                var view = layout.ToNative(Application.Current.MainPage.Handler.MauiContext);

                var window = this.floatyManager.Show(view, Util.DpParse(-10), Util.DpParse(100));
                this.floatys.Add(window);


                view.SetOnTouchListener(new OnTouchListener((v, e) =>
                {
                    return IconWindow_TouchListener(v, e);
                }));



                ScriptBroadcastReceiver.AddListener(Intent.ActionConfigurationChanged, () =>
                 {
                     var width = Globals.Devices.Width;
                     var layoutParams = view.LayoutParameters as WindowManagerLayoutParams;

                     if (layoutParams.X < width / 2)
                     {
                         layoutParams.X = Util.DpParse(-10);
                     }
                     else
                     {
                         layoutParams.X = width - view.Width + Util.DpParse(10);
                     }
                     FloatyService.Instance.UpdateViewLayout(view, layoutParams);

                 });
            }
        }

        private float x;

        private float y;

        private bool isMoving;

        private bool IconWindow_TouchListener(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                this.x = e.RawX;
                this.y = e.RawY;
            }
            else if (e.Action == MotionEventActions.Move)
            {
                var offsetX = (int)(e.RawX - this.x);
                var offsetY = (int)(e.RawY - this.y);
                if (!this.isMoving)
                {
                    if (Math.Abs(offsetX) < 25 && Math.Abs(offsetY) < 25)
                    {
                        return true;
                    }
                }
                this.isMoving = true;
                this.x = e.RawX;
                this.y = e.RawY;

                var layoutParams = v.LayoutParameters as WindowManagerLayoutParams;

                layoutParams.X += offsetX;
                layoutParams.Y += offsetY;
                FloatyService.Instance.UpdateViewLayout(v, layoutParams);
            }
            else if (e.Action == MotionEventActions.Up)
            {
                var width = Globals.Devices.Width;

                if (this.isMoving)
                {
                    var layoutParams = v.LayoutParameters as WindowManagerLayoutParams;

                    if (layoutParams.X < width / 2)
                    {
                        layoutParams.X = Util.DpParse(-10);
                    }
                    else
                    {
                        layoutParams.X = width - v.Width + Util.DpParse(10);
                    }
                    FloatyService.Instance.UpdateViewLayout(v, layoutParams);
                }
                this.isMoving = false;
            }
            return true;
        }


        internal void Remove()
        {
            foreach (var floaty in this.floatys)
            {
                this.floatyManager.Remove(floaty);
            }
        }



    }
}
