using Android;
using Android.Content;
using Android.Views;
using astator.Core.Script;
using astator.Core.UI.Floaty;
using astator.Modules;
using astator.NugetManager;
using astator.Popups;
using astator.TipsView;
using astator.Views;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Platform;
using System.IO.Compression;
using System.Xml.Linq;

namespace astator.Pages;

public partial class HomePage : ContentPage
{
    private string rootDir = string.Empty;
    private string currentDir;

    public HomePage()
    {
        InitializeComponent();

        var permissions = new string[]
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage
        };

        PermissionHelperer.ReqPermission(permissions[0], result =>
        {
            if (!result) NotPermissionExit();

            PermissionHelperer.ReqPermission(permissions[1], result =>
            {
                if (!result) NotPermissionExit();

                if (OperatingSystem.IsAndroidVersionAtLeast(30) && !Android.OS.Environment.IsExternalStorageManager)
                {
                    PermissionHelperer.StartActivityForResult(new Intent(Android.Provider.Settings.ActionManageAllFilesAccessPermission), result =>
                    {
                        if (OperatingSystem.IsAndroidVersionAtLeast(30) && Android.OS.Environment.IsExternalStorageManager) Initialze();
                        else NotPermissionExit();
                    });
                }
                else Initialze();
            });
        });
    }

    private static void NotPermissionExit()
    {
        new AndroidX.AppCompat.App.AlertDialog
           .Builder(Globals.AppContext)
           .SetTitle("错误")
           .SetMessage("请求权限失败, 应用退出!")
           .SetPositiveButton("确认", (s, e) =>
           {
               Java.Lang.JavaSystem.Exit(0);
           })
           .Show();
    }

    private void Initialze()
    {
        this.rootDir = Android.OS.Environment.GetExternalStoragePublicDirectory("astator").ToString();

        if (!Directory.Exists(this.rootDir))
        {
            Directory.CreateDirectory(this.rootDir);
        }

        var scriptDir = Path.Combine(this.rootDir, "脚本");

        if (!Directory.Exists(scriptDir))
        {
            Directory.CreateDirectory(scriptDir);
        }

        UpdateDirTbs(scriptDir);

        // DownloadExamples(scriptDir);
    }

    private void ShowFiles(string directory)
    {
        this.FilesLayout.Clear();
        var dirs = Directory.EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly).ToList();
        dirs.Sort();
        foreach (var dir in dirs)
        {
            var name = Path.GetFileName(dir);
            var info = $"{new DirectoryInfo(dir).LastWriteTime:yyyy/MM/dd HH:mm}";

            var card = new PathCard
            {
                Tag = dir,
                PathName = name,
                PathInfo = info,
                TypeImageSource = "folder.png",
            };
            card.Clicked += Dir_Clicked;
            this.FilesLayout.Add(card);
        }

        var files = Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly).ToList();
        files.Sort();
        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            var info = $"{new DirectoryInfo(file).LastWriteTime:yyyy/MM/dd HH:mm}";
            var icon = file.EndsWith(".cs") ? "_code"
                : file.EndsWith(".png") ? "_pic"
                : file.EndsWith(".csproj") ? "_config"
                : file.EndsWith(".json") ? "_config"
                : file.EndsWith(".xml") ? "_txt"
                : file.EndsWith(".txt") ? "_txt"
                : "_unknown";

            var card = new PathCard
            {
                Tag = file,
                PathName = name,
                PathInfo = info,
                TypeImageSource = $"file{icon}.png",
            };
            card.Clicked += File_Clicked;
            card.LongClicked += File_LongClicked;
            this.FilesLayout.Add(card);
        }
    }

    private async void File_Clicked(object sender, EventArgs e)
    {
        var card = sender as PathCard;
        var path = card.Tag as string;

        if (path.EndsWith(".cs")
            || path.EndsWith(".csproj")
            || path.EndsWith(".json")
            || path.EndsWith(".txt")
            || path.EndsWith(".xml"))
        {
            await this.Navigation.PushModalAsync(new CodeEditorPage(path));
        }
        else if (path.EndsWith(".png"))
        {
            var layout = new Grid
            {
                WidthRequest = 200,
                HeightRequest = 200,
                BackgroundColor = Colors.Transparent
            };

            layout.Add(new Image
            {
                Source = ImageSource.FromFile(path),
                WidthRequest = 200,
                HeightRequest = 200
            });

            var view = layout.ToPlatform(Application.Current.MainPage.Handler.MauiContext);
            var floaty = new AppFloatyWindow(Globals.AppContext, view, gravity: GravityFlags.Center);
            await Task.Delay(2000);
            floaty.Remove();
        }
        else if (path.EndsWith(".apk"))
        {
            Globals.InstallApp(path);
        }
    }

    private void Dir_Clicked(object sender, EventArgs e)
    {
        var card = sender as PathCard;
        var directory = card.Tag as string;
        UpdateDirTbs(directory);
    }

    private void DirTb_Clicked(object sender, EventArgs e)
    {
        var dir = sender as CustomLabelButton;
        UpdateDirTbs(dir.Tag as string);
    }

    private async void File_LongClicked(object sender, EventArgs e)
    {
        var card = sender as PathCard;
        var fileOptions = new Popups.FileOptions(card);
        var item = await this.ShowPopupAsync(fileOptions) as string;
        if (string.IsNullOrEmpty(item)) return;
        var fileName = card.Tag.ToString();
        switch (item)
        {
            case "运行项目":
                _ = ScriptManager.Instance.RunProject(Path.GetDirectoryName(fileName));
                break;
            case "运行脚本":
                _ = ScriptManager.Instance.RunScript(fileName);
                break;
            case "打包apk":
            {
                var apkbuilderer = new ApkBuilderer(Path.GetDirectoryName(fileName));
                _ = apkbuilderer.Build();
                break;
            }
            case "编译dll":
            {
                var apkbuilderer = new ApkBuilderer(Path.GetDirectoryName(fileName));
                _ = apkbuilderer.CompileDll();
                break;
            }
            case "重命名":
                var pathRename = new PathRename();
                var newName = await this.ShowPopupAsync(pathRename) as string;
                if (!string.IsNullOrEmpty(newName))
                {
                    File.Move(fileName, Path.Combine(Path.GetDirectoryName(fileName), newName));
                    var refreshView = card.Parent?.Parent?.Parent?.Parent as RefreshView;
                    if (refreshView is not null)
                    {
                        refreshView.IsRefreshing = true;
                    };
                };
                break;
            case "删除":
            {
                var alert = new AndroidX.AppCompat.App.AlertDialog
                    .Builder(Globals.AppContext)
                    .SetTitle("删除文件")
                    .SetMessage($"确认删除 \"{fileName}\" 吗?")
                    .SetPositiveButton("确认", (s, e) =>
                    {
                        File.Delete(fileName);
                        var refreshView = card.Parent?.Parent?.Parent?.Parent as RefreshView;
                        if (refreshView is not null)
                        {
                            refreshView.IsRefreshing = true;
                        }
                    })
                    .SetNegativeButton("取消", (s, e) => { });
                alert.Show();
                break;
            }
            case "分享":
            {
                var intent = new Intent(Intent.ActionSend);
                var contentType = new FileResult(fileName).ContentType;
                var uri = AndroidX.Core.Content.FileProvider.GetUriForFile(Android.App.Application.Context,
                    Android.App.Application.Context.PackageName + ".fileProvider",
                    new Java.IO.File(fileName));
                intent.SetType(contentType);
                intent.PutExtra(Intent.ExtraStream, uri);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                Globals.AppContext.StartActivity(intent);
                break;
            }
            case "其他应用打开":
            {
                var intent = new Intent(Intent.ActionView);
                intent.AddFlags(ActivityFlags.NewTask);
                var contentType = new FileResult(fileName).ContentType;
                var uri = AndroidX.Core.Content.FileProvider.GetUriForFile(Android.App.Application.Context,
                    Android.App.Application.Context.PackageName + ".fileProvider",
                    new Java.IO.File(fileName));
                intent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
                intent.SetDataAndType(uri, contentType);
                Globals.AppContext.StartActivity(intent);
                break;
            }
        };
    }


    private void UpdateDirTbs(string dir)
    {
        this.currentDir = dir;
        this.DirTbLayout.Clear();
        var dirs = Path.GetRelativePath(this.rootDir, dir).Split(Path.DirectorySeparatorChar);
        for (var i = Math.Max(0, dirs.Length - 3); i < dirs.Length; i++)
        {
            var tb = new CustomLabelButton
            {
                Text = dirs[i],
                TextColor = Color.Parse("#888888"),
                Tag = Path.Combine(this.rootDir, string.Join('/', dirs[0..(i + 1)])),
            };

            tb.Clicked += DirTb_Clicked;
            this.DirTbLayout.Add(tb);

            var img = new Image
            {
                HeightRequest = 18,
                WidthRequest = 18,
                Margin = 0,
                VerticalOptions = LayoutOptions.Center,
                Source = ImageSource.FromFile("right.png")
            };

            this.DirTbLayout.Add(img);

            if (i == dirs.Length - 1)
            {
                tb.TextColor = Color.Parse("#4a4a4d");
                ShowFiles(tb.Tag as string);
            }
        }
    }

    public bool OnBackPressed()
    {
        var count = this.DirTbLayout.Children.Count / 2;
        if (count > 1)
        {
            UpdateDirTbs((this.DirTbLayout.Children[(count - 2) * 2] as CustomLabelButton).Tag as string);
            return true;
        }
        return false;
    }

    private void Refresh_Refreshing(object sender, EventArgs e)
    {
        UpdateDirTbs(this.currentDir);
        var refresh = sender as RefreshView;
        refresh.IsRefreshing = false;
    }

    private async void DownloadExamples(string scriptDir)
    {
        try
        {
            var nugetCommands = new NugetCommands();
            var version = await nugetCommands.ParseVersionAsync("astator.Examples", "0.4.*");
            if (version is null) return;

            var versionStr = version.ToString();
            var outputDir = Path.Combine(scriptDir, "Examples");

            if (Directory.Exists(outputDir))
            {
                var csprojPath = Directory.GetFiles(outputDir, "*.csproj", SearchOption.AllDirectories).First();
                if (!string.IsNullOrEmpty(csprojPath))
                {
                    var xd = XDocument.Load(csprojPath);
                    var config = xd.Descendants("ApkBuilderConfigs");
                    var currentVersion = config.Select(x => x.Element("Version")).First()?.Value;
                    if (!string.IsNullOrEmpty(currentVersion))
                    {
                        if (versionStr.Equals(currentVersion))
                        {
                            return;
                        }
                    }
                }
                Directory.Delete(outputDir, true);
            }

            TipsViewImpl.Show();
            TipsViewImpl.ChangeTipsText("正在下载示例文件...");

            var path = Path.Combine(NugetCommands.GetInstalledDir("astator.Examples", version), "lib", "net6.0-android31.0", "examples.zip");
            if (!File.Exists(path))
            {
                if (!await nugetCommands.InstallPackageAsync("astator.Examples", version))
                {
                    Logger.Error("下载示例文件失败!");
                    return;
                }
            }

            if (File.Exists(path))
            {
                using var fs = new FileStream(path, FileMode.Open);
                using var zip = new ZipArchive(fs);
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                zip.ExtractToDirectory(outputDir, true);
                UpdateDirTbs(this.currentDir);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
        finally
        {
            TipsViewImpl.Hide();
        }
    }
}