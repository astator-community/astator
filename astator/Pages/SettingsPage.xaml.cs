using AndroidX.AppCompat.App;
using astator.Core.Accessibility;
using astator.Core.Graphics;
using astator.Core.Script;
using astator.Modules;
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
                if (!PermissionHelperer.CheckAccessibility())
                {
                    PermissionHelperer.ReqAccessibility();
                    this.AccessibilityService.IsToggled = false;
                }
            }
            else
            {
                PermissionHelperer.CloseAccessibility();
            }
        }


        private void CaptureService_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (!PermissionHelperer.CheckScreenCap())
                {
                    PermissionHelperer.ReqScreenCap(result =>
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

        private void Floaty_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (!PermissionHelperer.CheckFloaty())
                {
                    PermissionHelperer.ReqFloaty();
                    this.Floaty.IsToggled = false;
                }
                else
                {
                    FloatyManager.Instance?.Show();
                }
            }
            else
            {
                FloatyManager.Instance?.Remove();
            }
        }

        private void Debug_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                try
                {
                    var setDebugMode = new SetDebugMode();
                    var view = setDebugMode.ToPlatform(this.Handler.MauiContext);
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
                catch (Exception ex)
                {
                    ScriptLogger.Error(ex);
                }
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