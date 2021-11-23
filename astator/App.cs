using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Application = Microsoft.Maui.Controls.Application;
using WinApp = Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific.Application;

namespace astator
{
    public partial class App : Application
    {
        public App()
        {
            Build();

            this.MainPage = new MainPage();
        }

        private void Build()
        {
            WinApp.SetImageDirectory(this, "Assets");
            this.Resources.Add("PrimaryColor", Color.FromArgb("#512BDF"));
            this.Resources.Add("PrimaryTextColor", Color.FromArgb("#444444"));
            this.Resources.Add("SecondaryColor", Colors.White);
            this.Resources.Add("PageBackgroundColor", Color.FromArgb("#f0f3f6"));

            this.Resources.Add(new Style(typeof(Label))
            {
                Setters =
                {
                    new() { Property = Label.TextColorProperty, Value = this.Resources["PrimaryTextColor"] },
                    new() { Property = Label.FontFamilyProperty, Value = "OpenSansRegular" }
                }
            });

            this.Resources.Add(new Style(typeof(Button))
            {
                Setters =
                {
                    new() { Property = Button.TextColorProperty, Value = this.Resources["PrimaryTextColor"] },
                    new() { Property = Button.FontFamilyProperty, Value = "OpenSansRegular" },
                    new() { Property = VisualElement.BackgroundColorProperty, Value = this.Resources["PrimaryColor"] },
                    new() { Property = Button.PaddingProperty, Value = new Thickness(14,10) }
                }
            });
        }
    }
}
