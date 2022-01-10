using Android.Content;
using Android.Views;
using astator.Controllers;
using astator.Core;
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

    public partial class CustomPathCard : CustomCard
    {
        public static readonly BindableProperty TagBindableProperty = BindableProperty.Create(nameof(Tag), typeof(object), typeof(CustomLabelButton));
        public object Tag
        {
            get => GetValue(TagBindableProperty);
            set => SetValue(TagBindableProperty, value);
        }

        public static readonly BindableProperty TypeImageSourceBindableProperty = BindableProperty.Create(nameof(TypeImageSource), typeof(string), typeof(CustomPathCard));
        public string TypeImageSource
        {
            get => GetValue(TypeImageSourceBindableProperty) as string;
            set => SetValue(TypeImageSourceBindableProperty, value);
        }

        public static readonly BindableProperty MenuImageSourceBindableProperty = BindableProperty.Create(nameof(MenuImageSource), typeof(string), typeof(CustomPathCard));
        public string MenuImageSource
        {
            get => GetValue(MenuImageSourceBindableProperty) as string;
            set => SetValue(MenuImageSourceBindableProperty, value);
        }

        public static readonly BindableProperty PathNameBindableProperty = BindableProperty.Create(nameof(PathName), typeof(string), typeof(CustomPathCard));
        public string PathName
        {
            get => GetValue(PathNameBindableProperty) as string;
            set => SetValue(PathNameBindableProperty, value);
        }

        public static readonly BindableProperty PathInfoBindableProperty = BindableProperty.Create(nameof(PathInfo), typeof(string), typeof(CustomPathCard));
        public string PathInfo
        {
            get => GetValue(PathInfoBindableProperty) as string;
            set => SetValue(PathInfoBindableProperty, value);
        }

        public EventHandler MenuItemClicked;

        public CustomPathCard()
        {
            InitializeComponent();
        }


        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            var view = this.Handler.NativeView as LayoutViewGroup;

            if (this.TypeImageSource != "dir.png")
            {
                var menu = new AndroidX.AppCompat.Widget.PopupMenu(Globals.AppContext, view, (int)GravityFlags.Right);

                if (this.PathName.EndsWith(".csproj"))
                {
                    menu.Menu.Add("运行项目");
                    menu.Menu.Add("打包apk");
                    menu.Menu.Add("编译dll");
                }
                else if (this.PathName.EndsWith(".csx"))
                {
                    menu.Menu.Add("运行脚本");
                }

                menu.Menu.Add("其他应用打开");


                menu.SetOnMenuItemClickListener(new OnMenuItemClickListener((item) =>
                {
                    if (item.TitleFormatted.ToString() == "运行项目")
                    {
                        _ = ScriptManager.Instance.RunProject(Path.GetDirectoryName(this.Tag.ToString()));
                    }
                    else if (item.TitleFormatted.ToString() == "运行脚本")
                    {
                        _ = ScriptManager.Instance.RunScript(this.Tag.ToString());
                    }
                    else if (item.TitleFormatted.ToString() == "其他应用打开")
                    {
                        var path = this.Tag.ToString();
                        var intent = new Intent(Intent.ActionView);
                        intent.AddFlags(ActivityFlags.NewTask);

                        var contentType = new FileResult(this.Tag.ToString()).ContentType;

                        var uri = AndroidX.Core.Content.FileProvider.GetUriForFile(Android.App.Application.Context,
                            Android.App.Application.Context.PackageName + ".fileProvider",
                            new Java.IO.File(path));

                        intent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
                        intent.SetDataAndType(uri, contentType);

                        Globals.AppContext.StartActivity(intent);
                    }
                    //MenuItemClicked?.Invoke(this, new MenuItemOnMenuItemClickEventArgs(true, item));
                    return true;
                }));

                view.SetOnLongClickListener(new OnLongClickListener((v) =>
                {
                    menu.Show();
                    return true;
                }));
            }
        }
    }
}

