using Android.Content;
using Android.Views;
using astator.Core.Script;
using astator.Core.UI.Base;
using astator.Modules;
using Microsoft.Maui.Platform;

namespace astator.Views
{
    public partial class PathCard : CustomCard
    {
        public static readonly BindableProperty TypeImageSourceBindableProperty = BindableProperty.Create(nameof(TypeImageSource), typeof(string), typeof(PathCard));
        public string TypeImageSource
        {
            get => GetValue(TypeImageSourceBindableProperty) as string;
            set => SetValue(TypeImageSourceBindableProperty, value);
        }

        public static readonly BindableProperty MenuImageSourceBindableProperty = BindableProperty.Create(nameof(MenuImageSource), typeof(string), typeof(PathCard));
        public string MenuImageSource
        {
            get => GetValue(MenuImageSourceBindableProperty) as string;
            set => SetValue(MenuImageSourceBindableProperty, value);
        }

        public static readonly BindableProperty PathNameBindableProperty = BindableProperty.Create(nameof(PathName), typeof(string), typeof(PathCard));
        public string PathName
        {
            get => GetValue(PathNameBindableProperty) as string;
            set => SetValue(PathNameBindableProperty, value);
        }

        public static readonly BindableProperty PathInfoBindableProperty = BindableProperty.Create(nameof(PathInfo), typeof(string), typeof(PathCard));
        public string PathInfo
        {
            get => GetValue(PathInfoBindableProperty) as string;
            set => SetValue(PathInfoBindableProperty, value);
        }

        public PathCard()
        {
            InitializeComponent();
        }
    }
}

