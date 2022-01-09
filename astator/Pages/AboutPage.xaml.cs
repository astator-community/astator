namespace astator
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            //base.OnAppearing();
            MainActivity.Instance.OnKeyDownCallback = (keyCode, e) =>
            {
                if (keyCode == Android.Views.Keycode.Back)
                {
                    this.Navigation.PopToRootAsync();
                    return true;
                }
                return false;
            };
        }

        protected override void OnDisappearing()
        {
            //base.OnDisappearing();
            MainActivity.Instance.OnKeyDownCallback = null;
        }
    }
}