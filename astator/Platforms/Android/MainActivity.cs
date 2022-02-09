using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using astator.Core;
using astator.Core.Broadcast;
using astator.Core.UI;
using astator.Pages;
using Microsoft.Maui.Platform;

namespace astator
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : MauiAppCompatActivity
    {
        public static MainActivity Instance { get; private set; }

        public Func<Keycode, KeyEvent, bool> OnKeyDownCallback { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            Globals.AppContext = this;
            TipsView.TipsViewImpl.AppContext = this;

            this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            this.Window.SetStatusBarColor(((Color)Microsoft.Maui.Controls.Application.Current.Resources["PrimaryColor"]).ToNative());
            this.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;

            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);


            var permissionLifecycleObserver = new LifecycleObserver(this);
            this.Lifecycle.AddObserver(permissionLifecycleObserver);
            Globals.Permission.LifecycleObserver = permissionLifecycleObserver;

            var permissions = new string[]
            {
                Manifest.Permission.ReadExternalStorage,
                Manifest.Permission.WriteExternalStorage
            };


            Globals.Permission.ReqPermission(permissions[0], result =>
            {
                if (!result)
                {
                    NotPermissionExit();
                }

                Globals.Permission.ReqPermission(permissions[1], result =>
                {
                    if (!result)
                    {
                        NotPermissionExit();
                    }

                    if (OperatingSystem.IsAndroidVersionAtLeast(30) && !Android.OS.Environment.IsExternalStorageManager)
                    {
                        Globals.Permission.StartActivityForResult(new Intent(Android.Provider.Settings.ActionManageAllFilesAccessPermission), result =>
                        {
                            if (OperatingSystem.IsAndroidVersionAtLeast(30) && Android.OS.Environment.IsExternalStorageManager)
                            {
                                InitializePage();
                            }
                            else
                            {
                                NotPermissionExit();
                            }
                        });
                    }
                    else
                    {
                        InitializePage();
                    }
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
                   Process.KillProcess(Process.MyPid());
               })
               .Show();
        }


        private void InitializePage()
        {
            var mainPage = Microsoft.Maui.Controls.Application.Current.MainPage as NavigationPage;
            var carouselPage = mainPage.RootPage as CarouselPage;
            if (this.PackageName == "com.astator.astator")
            {
                carouselPage.Children.Add(new HomePage());
                carouselPage.Children.Add(new LogPage());
                carouselPage.Children.Add(new DocPage());
                carouselPage.Children.Add(new SettingsPage());
            }
            else
            {
                carouselPage.Children.Add(new LogPage());
            }
        }

        protected override void OnDestroy()
        {
            UnregisterReceiver(ScriptBroadcastReceiver.Instance);
            base.OnDestroy();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }


        private DateTime latestTime;

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            var result = this.OnKeyDownCallback?.Invoke(keyCode, e) ?? false;

            if (!result && keyCode == Keycode.Back)
            {
                var pages = Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.NavigationStack;
                if (pages.Count > 1)
                {
                    Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    var time = DateTime.Now;
                    if (time.Subtract(this.latestTime).TotalMilliseconds < 1000)
                    {
                        Process.KillProcess(Process.MyPid());
                    }
                    else
                    {
                        this.latestTime = time;
                        Globals.Toast("再按一次返回退出应用");
                    }
                }
                return true;
            }

            return result ? result : base.OnKeyDown(keyCode, e);
        }
    }
}