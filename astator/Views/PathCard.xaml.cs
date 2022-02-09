using Android.Content;
using Android.Views;
using astator.Controllers;
using astator.Core;
using astator.Core.UI.Base;
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

        public static readonly BindableProperty IsAddMenuBindableProperty = BindableProperty.Create(nameof(IsAddMenu), typeof(bool), typeof(PathCard));
        public bool IsAddMenu
        {
            get => (bool)GetValue(IsAddMenuBindableProperty);
            set => SetValue(IsAddMenuBindableProperty, value);
        }

        public PathCard()
        {
            InitializeComponent();
        }





        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            var view = this.Handler.NativeView as LayoutViewGroup;

            if (this.IsAddMenu)
            {
                var menu = new AndroidX.AppCompat.Widget.PopupMenu(Globals.AppContext, view, (int)GravityFlags.Right);

                //if (this.TypeImageSource == "dir.png")
                //{
                //    menu.Menu.Add("重命名");
                //    menu.Menu.Add("删除");
                //}
                //else
                {
                    if (this.PathName.EndsWith(".csproj"))
                    {
                        menu.Menu.Add("运行项目");
                        menu.Menu.Add("打包apk");
                        menu.Menu.Add("编译dll");
                    }
                    else if (this.PathName.EndsWith(".cs"))
                    {
                        menu.Menu.Add("运行脚本");
                    }

                    menu.Menu.Add("其他应用打开");
                }


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
                    else if (item.TitleFormatted.ToString() == "打包apk")
                    {
                        var path = this.Tag.ToString();
                        var apkbuilderer = new ApkBuilderer(Path.GetDirectoryName(path));
                        _ = apkbuilderer.Build();
                    }
                    else if (item.TitleFormatted.ToString() == "编译dll")
                    {

                        var path = this.Tag.ToString();
                        var apkbuilderer = new ApkBuilderer(Path.GetDirectoryName(path));
                        _ = apkbuilderer.CompileDll();
                    }

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

