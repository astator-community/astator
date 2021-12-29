using Android.Views;
using astator.Core.UI;
using Microsoft.Maui.Platform;

namespace astator.Views
{
    public struct PathCardArgs
    {
        public object Tag;
        public string PathName;
        public string PathInfo;
        public string TypeImageSource;
        public string MoreImageSource;
    }

    public partial class PathCard : GridLayout
    {
        public static readonly BindableProperty TagBindableProperty = BindableProperty.Create(nameof(Tag), typeof(object), typeof(LabelButton));
        public object Tag
        {
            get => GetValue(TagBindableProperty);
            set => SetValue(TagBindableProperty, value);
        }

        public static readonly BindableProperty TypeImageSourceBindableProperty = BindableProperty.Create(nameof(TypeImageSource), typeof(string), typeof(PathCard));
        public string TypeImageSource
        {
            get => GetValue(TypeImageSourceBindableProperty) as string;
            set => SetValue(TypeImageSourceBindableProperty, value);
        }

        public static readonly BindableProperty MoreImageSourceBindableProperty = BindableProperty.Create(nameof(MoreImageSource), typeof(string), typeof(PathCard));
        public string MoreImageSource
        {
            get => GetValue(MoreImageSourceBindableProperty) as string;
            set => SetValue(MoreImageSourceBindableProperty, value);
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

        public event EventHandler OnClicked;


        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            var view = this.Handler.NativeView as LayoutViewGroup;

            view.SetOnTouchListener(new OnTouchListener((v, e) =>
            {
                if (e.Action == MotionEventActions.Down)
                {
                    this.BackgroundColor = Color.Parse("#cad4de");
                }
                else
                {
                    this.BackgroundColor = Color.Parse("#f0f3f6");
                }
                return false;
            }));

            view.SetOnClickListener(new OnClickListener((v) =>
            {
                OnClicked?.Invoke(this, null);
            }));

        }
    }
}