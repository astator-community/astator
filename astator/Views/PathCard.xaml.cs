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

        public static readonly BindableProperty IsAddMenuBindableProperty = BindableProperty.Create(nameof(IsAddMenu), typeof(bool), typeof(PathCard));
        public bool IsAddMenu
        {
            get => (bool)GetValue(IsAddMenuBindableProperty);
            set => SetValue(IsAddMenuBindableProperty, value);
        }

        private bool AlreadyAddMenu = false;

        public PathCard()
        {
            InitializeComponent();
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (this.IsAddMenu && !AlreadyAddMenu)
            {
                AlreadyAddMenu = true;
                var view = this.Handler.PlatformView as LayoutViewGroup;
                var menu = new AndroidX.AppCompat.Widget.PopupMenu(Globals.AppContext, view, (int)GravityFlags.Right);
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

                menu.Menu.Add("重命名");
                menu.Menu.Add("删除");
                menu.Menu.Add("分享");
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
                    else if (item.TitleFormatted.ToString() == "重命名")
                    {
                        var title = new Label
                        {
                            Text = "重命名文件",
                            FontSize = 20,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center
                        };

                        var editor = new Editor
                        {
                            FontSize = 16,
                            Placeholder = "输入新文件名"
                        };

                        var cancel = new CustomLabelButton
                        {
                            Text = "取消",
                            Padding = new Thickness(16, 10),
                            BackgroundColor = (Color)Application.Current.Resources["PrimaryColor"],
                            FontSize = 15
                        };

                        var confirm = new CustomLabelButton
                        {
                            Text = "确认",
                            Margin = new Thickness(5, 0, 0, 0),
                            Padding = new Thickness(16, 10),
                            BackgroundColor = (Color)Application.Current.Resources["PrimaryColor"],
                            FontSize = 15
                        };
                        var stack = new HorizontalStackLayout
                        {
                            cancel,
                            confirm
                        };
                        stack.HorizontalOptions = LayoutOptions.End;

                        var layout = new Grid
                        {
                            title,
                            editor,
                            stack
                        };

                        layout.Padding = new Thickness(20, 15, 20, 5);
                        layout.RowSpacing = 10;
                        layout.RowDefinitions = new RowDefinitionCollection
                        {
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto }
                        };

                        title.SetValue(RowProperty, 0);
                        editor.SetValue(RowProperty, 1);
                        stack.SetValue(RowProperty, 2);

                        var view = layout.ToPlatform(this.Handler.MauiContext);
                        var builder = new AndroidX.AppCompat.App.AlertDialog.Builder(Globals.AppContext).SetView(view);
                        var dialog = builder.Show();

                        cancel.Clicked += (s, e) =>
                        {
                            dialog.Dismiss();
                        };

                        confirm.Clicked += (s, e) =>
                        {
                            dialog.Dismiss();
                            var oldName = this.Tag.ToString();
                            var newName = Path.Combine(Path.GetDirectoryName(oldName), editor.Text);
                            File.Move(oldName, newName);
                            var refreshView = this.Parent?.Parent?.Parent?.Parent as RefreshView;
                            if (refreshView is not null)
                            {
                                refreshView.IsRefreshing = true;
                            }
                        };
                    }
                    else if (item.TitleFormatted.ToString() == "删除")
                    {
                        var alert = new AndroidX.AppCompat.App.AlertDialog
                            .Builder(Globals.AppContext)
                            .SetTitle("删除文件")
                            .SetMessage($"确认删除 \"{this.Tag}\" 吗?")
                            .SetPositiveButton("确认", (s, e) =>
                            {
                                File.Delete(this.Tag.ToString());
                                var refreshView = this.Parent?.Parent?.Parent?.Parent as RefreshView;
                                if (refreshView is not null)
                                {
                                    refreshView.IsRefreshing = true;
                                }
                            })
                            .SetNegativeButton("取消", (s, e) => { });

                        alert.Show();
                    }
                    else if (item.TitleFormatted.ToString() == "分享")
                    {
                        var path = this.Tag.ToString();
                        var intent = new Intent(Intent.ActionSend);
                        var contentType = new FileResult(this.Tag.ToString()).ContentType;
                        var uri = AndroidX.Core.Content.FileProvider.GetUriForFile(Android.App.Application.Context,
                            Android.App.Application.Context.PackageName + ".fileProvider",
                            new Java.IO.File(path));
                        intent.SetType(contentType);
                        intent.PutExtra(Intent.ExtraStream, uri);
                        intent.AddFlags(ActivityFlags.GrantReadUriPermission);
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

