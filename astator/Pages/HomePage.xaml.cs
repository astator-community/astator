using AndroidX.AppCompat.Widget;
using astator.Core;
using Microsoft.Maui.Controls;
using System;

namespace astator.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            this.NavBar.ActiveTab = "home";

        }

        public void Click(object sender, EventArgs e)
        {

            (this.btn.Handler.NativeView as AppCompatButton).SetBackgroundColor(Android.Graphics.Color.Black);
        }
    }
}