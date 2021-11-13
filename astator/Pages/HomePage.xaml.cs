using Microsoft.Maui.Controls;
namespace astator.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            this.NavBar.ActiveTab = "home";
        }
    }
}