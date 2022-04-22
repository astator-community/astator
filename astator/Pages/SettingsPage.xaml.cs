using AndroidX.AppCompat.App;
using astator.Core.Script;
using astator.LoggerProvider;
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

        private void AccessibilityService_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (!PermissionHelperer.CheckAccessibility())
                {
                    PermissionHelperer.ReqAccessibility((enabled) =>
                    {
                        if (!enabled)
                        {
                            this.AccessibilityService.IsToggled = false;
                        }
                    });
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
                    PermissionHelperer.ReqScreenCap(false, result =>
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
                PermissionHelperer.CloseScreenCap();
            }
        }

        private void Floaty_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (!PermissionHelperer.CheckFloaty())
                {
                    PermissionHelperer.ReqFloaty((enabled) =>
                    {
                        if (!enabled)
                        {
                            this.Floaty.IsToggled = false;
                        }
                    });
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
                    Logger.Error(ex);
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
            await this.Navigation.PushModalAsync(new NugetPage());
        }

        private async void About_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PushModalAsync(new AboutPage());
        }
    }
}