using astator.Core;
using astator.Pages;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using Microsoft.Maui.Graphics;
using System;

namespace astator
{
    public partial class MainPage : CarouselPage
    {
        public MainPage()
        {
            Build();
        }

        private void Build()
        {
            this.Children.Add(new HomePage());
            this.Children.Add(new LogPage());
            this.Children.Add(new DocPage());
            this.Children.Add(new SettingsPage());


            Console.SetOut(new ScriptConsole());
        }
    }
}
