using Android.Content;
using astator.Controllers;
using astator.Core;
using astator.Core.Accessibility;
using astator.Core.Graphics;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;

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
            await Task.Delay(500);
            this.AccessibilityService.IsToggled = ScriptAccessibilityService.Instance is not null;
            this.CaptureService.IsToggled = ScreenCapturer.Instance is not null;
        }


        private void AccessibilityService_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (ScriptAccessibilityService.Instance is null)
                {
                    var intent = new Intent(Android.Provider.Settings.ActionAccessibilitySettings);
                    intent.SetFlags(ActivityFlags.NewTask);
                    Globals.AppContext.StartActivity(intent);
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
                    Globals.Permission.ReqScreenCap();
                }
            }
            else
            {
                ScreenCapturer.Instance?.Dispose();
            }
        }


        private TcpListener tcpListener;
        private CancellationTokenSource tokenSource;
        private ScriptLogger logger = ScriptLogger.Instance;

        private async void DebugService_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                try
                {
                    var hostAddress = Utils.GetLocalHostAddress() ?? "127.0.0.1";
                    this.tokenSource = new CancellationTokenSource();
                    this.tcpListener = new TcpListener(IPAddress.Parse(hostAddress), 1024);
                    this.tcpListener.Start();
                    ScriptLogger.Instance.Log("开启调试服务");
                    this.HostAddress.Text = $"({hostAddress})";
                    while (true)
                    {
                        var client = await this.tcpListener.AcceptTcpClientAsync(this.tokenSource.Token);
                        ConnectAsync(client);
                    }
                }
                catch (OperationCanceledException)
                {
                    ScriptLogger.Instance.Log("停止调试服务");
                    this.HostAddress.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    ScriptLogger.Instance.Error(ex.Message);
                }
            }
            else
            {
                this.tokenSource?.Cancel();
                this.tcpListener?.Stop();
                this.tcpListener = null;
            }
        }

        private static void ConnectAsync(TcpClient client)
        {
            _ = Task.Run(async () =>
            {
                var stream = client.GetStream();
                var info = $"{Globals.Devices.Brand} {Globals.Devices.Model}";
                await stream.WriteAsync(Stick.MakePackData("init", info));

                var logger = ScriptLogger.Instance;

                var key = logger.AddCallback("info", async (args) =>
                {
                    var pack = Stick.MakePackData("showMessage", $"[{info}] {args.Time:HH:mm:ss.fff}: {args.Message}");
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
                                using var zipStream = new MemoryStream(data.Buffer.Data);
                                var directory = Path.Combine(MauiApplication.Current.ExternalCacheDir.ToString(), $"{data.Description}-{DateTime.Now:MMddHHmmssfff}");
                                using (var archive = new ZipArchive(zipStream))
                                {
                                    archive.ExtractToDirectory(directory);
                                }

                                ScriptManager.Instance.RunProject(directory, data.Description);

                                break;
                            }
                            case "saveProject":
                            {
                                var directory = "/sdcard/astator/脚本";
                                if (Directory.Exists(directory))
                                {
                                    Directory.CreateDirectory(directory);
                                }
                                using var zipStream = new MemoryStream(data.Buffer.Data);
                                var saveDirectory = Path.Combine(directory, data.Description);
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
                        logger.RemoveCallback(key);
                    }
                }

            });
        }

        private void AccessibilityService_Clicked(object sender, EventArgs e)
        {
            this.AccessibilityService.IsToggled = !this.AccessibilityService.IsToggled;
        }

        private void CaptureService_Clicked(object sender, EventArgs e)
        {
            this.CaptureService.IsToggled = !this.CaptureService.IsToggled;
        }

        private void DebugService_Clicked(object sender, EventArgs e)
        {
            this.DebugService.IsToggled = !this.DebugService.IsToggled;
        }

        private async void Info_Clicked(object sender, EventArgs e)
        {
            var page = new AboutPage();
            NavigationPage.SetHasNavigationBar(page, false);
            await this.Navigation.PushAsync(page);
        }
    }
}