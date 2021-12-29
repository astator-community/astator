using astator.Views;
using Microsoft.Maui.Controls.Compatibility;

namespace astator;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OPPOSans-R.ttf", "OPPOSansRegular");
            }).ConfigureMauiHandlers(handler =>
            {
                handler.AddCompatibilityRenderer(typeof(LabelButton), typeof(LabelbuttonRenderer));

            });
        return builder.Build();
    }
}
