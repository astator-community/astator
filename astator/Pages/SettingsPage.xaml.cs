using Android.Content;
using astator.Core.Script;
using astator.Modules;
using astator.Popups;
using CommunityToolkit.Maui.Views;

namespace astator.Pages;

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

    private async void Debug_Toggled(object sender, ToggledEventArgs e)
    {
        if (!e.Value)
        {
            DebugService.Instance?.Dispose();
            return;
        }

        try
        {
            this.Debug.IsEnabled = false;
            var debugOptions = new DebugOptions();
            var result = await this.ShowPopupAsync(debugOptions);
            if (result is not null)
            {
                var (mode, ip) = ((int, string))result;
                var intent = new Intent(Globals.AppContext, typeof(DebugService));
                intent.PutExtra("mode", mode);
                intent.PutExtra("ip", ip);
                if (OperatingSystem.IsAndroidVersionAtLeast(26))
                {
                    Globals.AppContext.StartForegroundService(intent);
                }
                else
                {
                    Globals.AppContext.StartService(intent);
                }
                await Task.Delay(200);
                if (DebugService.Instance is null)
                {
                    this.Debug.IsToggled = false;
                }
            }
            else
            {
                this.Debug.IsToggled = false;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
        finally
        {
            this.Debug.IsEnabled = true;
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
