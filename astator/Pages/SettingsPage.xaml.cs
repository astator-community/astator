using AndroidX.AppCompat.App;
using astator.Modules;
using astator.Core.Accessibility;
using astator.Core.Graphics;
using astator.Core.Script;
using Microsoft.Maui.Platform;

namespace astator.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            this.AccessibilityService.IsToggled = ScriptAccessibilityService.Instance is not null;
            this.CaptureService.IsToggled = ScreenCapturer.Instance is not null;
            this.Floaty.IsToggled = FloatyManager.Instance.IsShow();
        }

        public void OnResume()
        {
            this.AccessibilityService.IsToggled = ScriptAccessibilityService.Instance is not null;
            this.Floaty.IsToggled = FloatyManager.Instance.IsShow();
        }

        private void AccessibilityService_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (ScriptAccessibilityService.Instance is null)
                {
                    Globals.Permission.ReqAccessibilityService();
                    this.AccessibilityService.IsToggled = false;
                }
            }
            else
            {
                ScriptAccessibilityService.Instance?.DisableSelf();
            }
        }


        private void CaptureService_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (ScreenCapturer.Instance is null)
                {
                    Globals.Permission.ReqScreenCap(result =>
                    {
                        if (!result)
                        {
                            this.CaptureService.IsToggled = false;
                        }
                    });
                }
            }
            else
            {
                ScreenCapturer.Instance?.Dispose();
            }
        }

        private async void Floaty_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (!Android.Provider.Settings.CanDrawOverlays(Globals.AppContext))
                {
                    Globals.Permission.ReqFloaty();
                    this.Floaty.IsToggled = false;
                }
                else
                {
                    if (Core.UI.Floaty.FloatyService.Instance is null)
                    {
                        await Globals.Permission.StartFloatyService();
                    }
                    FloatyManager.Instance.Show();
                }
            }
            else
            {
                FloatyManager.Instance.Remove();
            }
        }

        private void Debug_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                var setDebugMode = new SetDebugMode();
                var view = setDebugMode.ToNative(this.Handler.MauiContext);
                var builder = new AlertDialog.Builder(Globals.AppContext).SetView(view);
                var dialog = builder.Show();
                setDebugMode.DismissCallback += () =>
                {
                    dialog.Dismiss();
                };

                dialog.DismissEvent += async (s, e) =>
                {
                    await Task.Delay(200);
                    if (DebugService.Instance is null)
                    {
                        this.Debug.IsToggled = false;
                    }
                };
            }
            else
            {
                DebugService.Instance?.Dispose();
            }
        }

        private void AccessibilityService_Clicked(object sender, EventArgs e)
        {
            this.AccessibilityService.IsToggled = !this.AccessibilityService.IsToggled;
        }

        private void CaptureService_Clicked(object sender, EventArgs e)
        {
            this.CaptureService.IsToggled = !this.CaptureService.IsToggled;
        }

        private void Floaty_Clicked(object sender, EventArgs e)
        {
            this.CaptureService.IsToggled = !this.CaptureService.IsToggled;
        }

        private void Debug_Clicked(object sender, EventArgs e)
        {
            this.Debug.IsToggled = !this.Debug.IsToggled;
        }

        private async void NugetManage_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new NugetPage());
        }

        private async void About_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new AboutPage());
        }
    }
}