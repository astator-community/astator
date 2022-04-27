namespace astator.Views
{
    public partial class PackageInfoCard : CustomCard
    {
        public static readonly BindableProperty IconUriBindableProperty = BindableProperty.Create(nameof(IconUri), typeof(Uri), typeof(PackageInfoCard));
        public Uri IconUri
        {
            get => GetValue(IconUriBindableProperty) as Uri;
            set
            {
                if (value is not null)
                {
                    SetValue(IconUriBindableProperty, value);
                    this.Icon.Source = ImageSource.FromUri(value);
                }
            }
        }

        public static readonly BindableProperty PkgIdBindableProperty = BindableProperty.Create(nameof(PkgId), typeof(string), typeof(PackageInfoCard));
        public string PkgId
        {
            get => GetValue(PkgIdBindableProperty) as string;
            set => SetValue(PkgIdBindableProperty, value);
        }

        public static readonly BindableProperty DescriptionBindableProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(PackageInfoCard));
        public string Description
        {
            get => GetValue(DescriptionBindableProperty) as string;
            set => SetValue(DescriptionBindableProperty, value);
        }

        public static readonly BindableProperty VersionBindableProperty = BindableProperty.Create(nameof(Version), typeof(string), typeof(PackageInfoCard));
        public string Version
        {
            get => GetValue(VersionBindableProperty) as string;
            set => SetValue(VersionBindableProperty, value);
        }

        public PackageInfoCard()
        {
            InitializeComponent();
        }
    }
}