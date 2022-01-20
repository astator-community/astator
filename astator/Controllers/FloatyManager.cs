using Android.Content;
using Android.Views;
using astator.Core;
using astator.Core.Accessibility;
using astator.Core.Broadcast;
using astator.Core.UI;
using astator.Core.UI.Floaty;
using astator.Pages;
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

        private readonly Dictionary<string, FloatWindow> floatys = new();

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
                view.SetOnTouchListener(new OnTouchListener((v, e) =>
                {
                    return IconFloaty_TouchListener(v, e);
                }));

                ScriptBroadcastReceiver.AddListener(Intent.ActionConfigurationChanged, () =>
                 {
                     var width = Globals.Devices.Width;
                     var layoutParams = view.LayoutParameters as WindowManagerLayoutParams;

                     if (layoutParams.X < width / 2)
                     {
                         layoutParams.X = Core.UI.Util.DpParse(-10);
                     }
                     else
                     {
                         layoutParams.X = width - view.Width + Core.UI.Util.DpParse(10);
                     }
                     FloatyService.Instance.UpdateViewLayout(view, layoutParams);
                 });

                var window = this.floatyManager.Show(view, Core.UI.Util.DpParse(-10), Core.UI.Util.DpParse(100));
                this.floatys.Add("iconFloaty", window);
            }
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
                        layoutParams.X = Core.UI.Util.DpParse(-10);
                    }
                    else
                    {
                        layoutParams.X = width - v.Width + Core.UI.Util.DpParse(10);
                    }
                    FloatyService.Instance.UpdateViewLayout(v, layoutParams);
                    this.isMoving = false;
                }
                else
                {
                    var exist = this.floatys.TryGetValue("fastRunner", out var fastRunner);
                    if (exist)
                    {
                        this.floatyManager.Remove(fastRunner);
                        this.floatys.Remove("fastRunner");
                    }
                    else
                    {
                        //ShowFastRunner();
                        var s = UIFinder.FindOne(new SearcherArgs
                        {
                            Text = "YunxiOcr"
                        });

                        ScriptLogger.Log(s.GetBounds().ToString());
                        ScriptLogger.Log(s.GetDepth().ToString());
                        ScriptLogger.Log(s.ViewIdResourceName.Split("/").Last());
                    }
                }
            }
            return true;
        }

        private void ShowFastRunner()
        {
            var width = Globals.Devices.Width / Globals.Devices.Dp;
            var height = Globals.Devices.Height / Globals.Devices.Dp;
            var fastRunner = new FloatyFastRunner();
            var view = fastRunner.ToNative(Application.Current.MainPage.Handler.MauiContext);

            var window = this.floatyManager.Show(view,
                Convert.ToInt32((width - 285) / 2 * Globals.Devices.Dp),
                Convert.ToInt32((height - 550) / 2 * Globals.Devices.Dp));

            view.SetOnTouchListener(new OnTouchListener((v, e) =>
            {
                if (e.Action == MotionEventActions.Outside)
                {
                    this.floatyManager.Remove(window);
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
                this.floatyManager.Remove(floaty);
            }
            this.floatys.Clear();
        }

    }
}
