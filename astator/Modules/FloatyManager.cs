using Android.Content;
using Android.Views;
using astator.Core.Broadcast;
using astator.Core.Script;
using astator.Core.UI.Base;
using astator.Core.UI.Floaty;
using astator.Pages;
using astator.Views;
using Microsoft.Maui.Platform;
using View = Android.Views.View;

namespace astator.Modules
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

        private readonly Dictionary<string, FloatyWindow> floatys = new();

        public bool IsShow()
        {
            return this.floatys.Count > 0;
        }

        public void Show()
        {
            var layout = new Grid
            {
                WidthRequest = 40,
                HeightRequest = 40,
                BackgroundColor = Colors.Transparent
            };

            layout.Add(new CustomImage
            {
                Source = "appicon.png",
                IsCircle = true,
                WidthRequest = 40,
                HeightRequest = 40,

            });

            var view = layout.ToNative(Application.Current.MainPage.Handler.MauiContext);
            view.SetOnTouchListener(new OnTouchListener((v, e) =>
            {
                return IconFloaty_TouchListener(v, e);
            }));

            ScriptBroadcastReceiver.AddListener(Intent.ActionConfigurationChanged, "floatyManager", () =>
            {
                var width = Globals.Devices.Width;
                var height = Globals.Devices.Height;
                var layoutParams = view.LayoutParameters as WindowManagerLayoutParams;

                if (layoutParams.X < width / 2)
                {
                    layoutParams.X = Util.Dp2Px(-6);
                }
                else
                {
                    layoutParams.X = width - view.Width + Util.Dp2Px(6);
                }

                if (layoutParams.Y > height * 0.8)
                {
                    layoutParams.Y = (int)(height * 0.5);
                }

                FloatyService.Instance.UpdateViewLayout(view, layoutParams);
            });

            var window = new FloatyWindow(view, -6, 100);
            this.floatys.Add("iconFloaty", window);
        }

        private float x;

        private float y;

        private bool isMoving;

        private bool IconFloaty_TouchListener(View v, MotionEvent e)
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
                        layoutParams.X = Util.Dp2Px(-6);
                    }
                    else
                    {
                        layoutParams.X = width - v.Width + Util.Dp2Px(6);
                    }
                    FloatyService.Instance.UpdateViewLayout(v, layoutParams);
                    this.isMoving = false;
                }
                else
                {
                    ShowFastRunner();
                }
            }
            return true;
        }

        private void ShowFastRunner()
        {
            var width = Globals.Devices.Width / Globals.Devices.Dp;
            var height = Globals.Devices.Height / Globals.Devices.Dp;
            var fastRunner = new FloatyFastRunner();
            if (Globals.Devices.Width > Globals.Devices.Height)
            {
                fastRunner.HeightRequest = 350;
                fastRunner.RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition
                    {
                        Height = 55
                    },
                    new RowDefinition
                    {
                        Height = 45
                    },
                    new RowDefinition
                    {
                        Height = 250
                    }
                };
            }
            var view = fastRunner.ToNative(Application.Current.MainPage.Handler.MauiContext);

            var window = new FloatyWindow(view, gravity: GravityFlags.Center, flags: WindowManagerFlags.NotFocusable | WindowManagerFlags.LayoutNoLimits | WindowManagerFlags.WatchOutsideTouch);

            view.SetOnTouchListener(new OnTouchListener((v, e) =>
            {
                if (e.Action == MotionEventActions.Outside)
                {
                    window.Remove();
                    this.floatys.Remove("fastRunner");
                    return true;
                }
                return false;
            }));

            this.floatys.Add("fastRunner", window);
        }


        internal void Remove()
        {
            foreach (var floaty in this.floatys.Values)
            {
                floaty.Remove();
            }
            ScriptBroadcastReceiver.RemoveListener(Intent.ActionConfigurationChanged, "floatyManager");
            this.floatys.Clear();
        }

    }
}
