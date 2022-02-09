using astator.Controllers;
using astator.Core;
using astator.Core.Accessibility;
using astator.Core.Graphics;
using Java.Net;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Path = System.IO.Path;

namespace astator.Pages
{
    public partial class SettingsPage : ContentPage
    {

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await Task.Delay(200);
            this.AccessibilityService.IsToggled = ScriptAccessibilityService.Instance is not null;
            this.CaptureService.IsToggled = ScreenCapturer.Instance is not null;
            this.Floaty.IsToggled = FloatyManager.Instance.IsShow();
        }


        private async void AccessibilityService_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (ScriptAccessibilityService.Instance is null)
                {
                    Globals.Permission.ReqAccessibilityService();
                    await Task.Delay(200);
                    if (ScriptAccessibilityService.Instance is null)
                    {
                        this.AccessibilityService.IsToggled = false;
                    }
                }
            }
            else
            {
                ScriptAccessibilityService.Instance?.DisableSelf();
            }
        }


        private void CaptureService_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (ScreenCapturer.Instance is null)
                {
                    Globals.Permission.ReqScreenCap(result =>
                    {
                        if (!result)
                        {
                            this.CaptureService.IsToggled = false;
                        }
                    });
                }
            }
            else
            {
                ScreenCapturer.Instance?.Dispose();
            }
        }

        private async void Floaty_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (!Android.Provider.Settings.CanDrawOverlays(Globals.AppContext))
                {
                    Globals.Permission.ReqFloaty();
                    this.Floaty.IsToggled = false;
                }
                else
                {
                    if (Core.UI.Floaty.FloatyService.Instance is null)
                    {
                        Globals.AppContext.StartService(new(Globals.AppContext, typeof(Core.UI.Floaty.FloatyService)));
                        await Task.Delay(200);
                    }
                    FloatyManager.Instance.Show();
                }
            }
            else
            {
                FloatyManager.Instance.Remove();
            }
        }


        private TcpListener tcpListener;
        private CancellationTokenSource tokenSource;

        private async void DebugService_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                try
                {
                    var hostAddress = GetLocalHostAddress() ?? "127.0.0.1";
                    this.tokenSource = new CancellationTokenSource();
                    this.tcpListener = new TcpListener(IPAddress.Parse(hostAddress), 1024);
                    this.tcpListener.Start();
                    ScriptLogger.Log("开启调试服务");
                    this.HostAddress.Text = $"({hostAddress})";
                    while (true)
                    {
                        var client = await this.tcpListener.AcceptTcpClientAsync(this.tokenSource.Token);
                        ConnectAsync(client);
                    }
                }
                catch (OperationCanceledException)
                {
                    ScriptLogger.Log("停止调试服务");
                    this.HostAddress.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    ScriptLogger.Error(ex.Message);
                }
            }
            else
            {
                this.tokenSource?.Cancel();
                this.tcpListener?.Stop();
                this.tcpListener = null;
            }
        }

        private static string GetLocalHostAddress()
        {
            var ie = NetworkInterface.NetworkInterfaces;
            while (ie.HasMoreElements)
            {
                var intf = ie.NextElement() as NetworkInterface;
                var enumIpAddr = intf.InetAddresses;
                while (enumIpAddr.HasMoreElements)
                {
                    var inetAddress = enumIpAddr.NextElement() as InetAddress;
                    if (!inetAddress.IsLoopbackAddress && inetAddress.HostAddress.StartsWith("192.168."))
                    {
                        return inetAddress.HostAddress.ToString();
                    }
                }
            }
            return null;
        }

        private static void ConnectAsync(TcpClient client)
        {
            _ = Task.Run(async () =>
            {
                var stream = client.GetStream();
                var info = $"{Globals.Devices.Brand} {Globals.Devices.Model}";
                await stream.WriteAsync(Stick.MakePackData("init", info));

                var key = ScriptLogger.AddCallback("info", async (args) =>
                {
                    var pack = Stick.MakePackData("showMessage", Encoding.UTF8.GetBytes($"[{info}] {args.Time:HH:mm:ss.fff}: {args.Message}"));
                    await stream.WriteAsync(pack);
                });

                while (true)
                {
                    try
                    {
                        Thread.Sleep(50);
                        var data = await Stick.ReadPackAsync(stream);

                        switch (data.Key)
                        {
                            case "runProject":
                            {
                                using var zipStream = new MemoryStream(data.Buffer);
                                var directory = Path.Combine(MauiApplication.Current.ExternalCacheDir.ToString(), data.Description);
                                ClearProject(directory);

                                using (var archive = new ZipArchive(zipStream))
                                {
                                    archive.ExtractToDirectory(directory, true);
                                }

                                _ = ScriptManager.Instance.RunProject(directory);

                                break;
                            }
                            case "runScript":
                            {
                                using var zipStream = new MemoryStream(data.Buffer);
                                var description = data.Description.Split("|");
                                var directory = Path.Combine(MauiApplication.Current.ExternalCacheDir.ToString(), description[0]);
                                ClearProject(directory);

                                using (var archive = new ZipArchive(zipStream))
                                {
                                    archive.ExtractToDirectory(directory, true);
                                }

                                _ = ScriptManager.Instance.RunScript(Path.Combine(directory, description[1]));

                                break;
                            }
                            case "saveProject":
                            {
                                var directory = "/sdcard/astator/脚本";
                                if (Directory.Exists(directory))
                                {
                                    Directory.CreateDirectory(directory);
                                }

                                using var zipStream = new MemoryStream(data.Buffer);
                                var saveDirectory = Path.Combine(directory, data.Description);
                                ClearProject(directory);

                                using var archive = new ZipArchive(zipStream);
                                archive.ExtractToDirectory(saveDirectory, true);
                                Globals.Toast($"项目已保存至{saveDirectory}");
                                break;
                            }
                            case "screenShot":
                            {
                                byte[] pack;

                                if (ScreenCapturer.Instance is not null)
                                {
                                    try
                                    {
                                        var img = ScreenCapturer.Instance.AcquireLatestBitmap();

                                        var ms = new MemoryStream();
                                        img.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, ms);
                                        var bytes = ms.ToArray();

                                        pack = Stick.MakePackData("screenShot_success", bytes);
                                    }
                                    catch (Exception)
                                    {
                                        pack = Stick.MakePackData("screenShot_fail", "获取截图失败!");
                                    }
                                }
                                else
                                {
                                    pack = Stick.MakePackData("screenShot_fail", "截图服务未开启!");
                                }


                                await stream.WriteAsync(pack);
                                break;
                            }
                            case "heartBeat":
                            {
                                var pack = Stick.MakePackData("heartBeat");
                                await stream.WriteAsync(pack);
                                break;
                            }
                            default:
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        ScriptLogger.RemoveCallback(key);
                    }
                }
            });
        }

        private static void ClearProject(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            var searchPatterns = new string[] { "*.cs", "*.csproj" };

            foreach (var searchPattern in searchPatterns)
            {
                var files = Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories);
                if (files.Any())
                {
                    foreach (var f in files)
                    {
                        File.Delete(f);
                    }
                }
            }
        }


        private void AccessibilityService_Clicked(object sender, EventArgs e)
        {
            this.AccessibilityService.IsToggled = !this.AccessibilityService.IsToggled;
        }

        private void CaptureService_Clicked(object sender, EventArgs e)
        {
            this.CaptureService.IsToggled = !this.CaptureService.IsToggled;
        }

        private void Floaty_Clicked(object sender, EventArgs e)
        {
            this.CaptureService.IsToggled = !this.CaptureService.IsToggled;
        }

        private void DebugService_Clicked(object sender, EventArgs e)
        {
            this.DebugService.IsToggled = !this.DebugService.IsToggled;
        }

        private async void NugetManage_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new NugetPage());
        }

        private async void About_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new AboutPage());
        }
    }
}