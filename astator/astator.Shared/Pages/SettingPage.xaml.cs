
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using astator.Controllers;
using astator.Core;
using astator.Core.Accessibility;
using astator.Core.Script;
using Java.Net;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace astator.Pages
{
    public sealed partial class SettingPage : UserControl
    {

        private TcpListener tcpListener;

        private CancellationTokenSource tokenSource;

        private TcpClient CurrentClient;
        public SettingPage()
        {
            InitializeComponent();

            ScriptAccessibilityService.ConnectCallback = () => this.AccessibilityService.IsOn = true;

            ScriptAccessibilityService.DestroyCallback = () => this.AccessibilityService.IsOn = false;
        }

        public new bool OnKeyDown(Keycode keycode, KeyEvent e)
        {
            return false;
        }

        private void AccessibilityService_Toggled(object sender, RoutedEventArgs e)
        {
            var ts = sender as ToggleSwitch;
            if (ts.IsOn)
            {
                if (ScriptAccessibilityService.Instance is null)
                {
                    Globals.StartActivity(new Intent(Android.Provider.Settings.ActionAccessibilitySettings));
                    ts.IsOn = false;
                }
            }
            else
            {
                if (ScriptAccessibilityService.Instance is not null)
                {
                    var dialog = new AlertDialog.Builder(MainActivity.Instance)
                    .SetTitle("提示")
                    .SetMessage("确定要关闭无障碍服务吗?")

                    .SetPositiveButton("确定", (sender, e) => ScriptAccessibilityService.Instance.DisableSelf())

                    .SetNegativeButton("取消", (sender, e) => ts.IsOn = true)

                    .Create();

                    dialog.Window.SetBackgroundDrawable(new ColorDrawable(Windows.UI.Color.FromArgb(0xff, 0xf0, 0xf3, 0xf6)));
                    dialog.Show();
                }
            }
        }

        private async void DebugService_Toggled(object sender, RoutedEventArgs e)
        {
            var ts = sender as ToggleSwitch;
            if (ts.IsOn)
            {
                var hostAddress = string.Empty;
                this.tokenSource = new CancellationTokenSource();
                var networkInterfaces = NetworkInterface.NetworkInterfaces;
                while (networkInterfaces.HasMoreElements)
                {
                    var networkInterface = networkInterfaces.NextElement() as NetworkInterface;
                    var inetAddresses = networkInterface.InetAddresses;
                    while (inetAddresses.HasMoreElements)
                    {
                        var address = inetAddresses.NextElement() as InetAddress;
                        Console.WriteLine(address.HostAddress);
                        if (address.HostAddress.StartsWith("192.168."))
                        {
                            hostAddress = address.HostAddress;
                        }
                    }
                }
                try
                {
                    this.tcpListener = new TcpListener(IPAddress.Parse(hostAddress), 1024);
                    this.tcpListener.Start();
                    ScriptLogger.Instance.Log("开启调试服务: " + hostAddress);
                    this.Address.Text = hostAddress;
                    while (true)
                    {
                        var client = await this.tcpListener.AcceptTcpClientAsync(this.tokenSource.Token);

                        this.CurrentClient?.Close();
                        this.CurrentClient = client;
                        ConnectAsync(client);
                    }
                }
                catch (OperationCanceledException)
                {
                    ScriptLogger.Instance.Log("停止调试服务");
                    this.Address.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    ScriptLogger.Instance.Error(ex.Message);
                }
            }
            else
            {
                this.tokenSource.Cancel();
                this.tcpListener?.Stop();
                this.tcpListener = null;
            }
        }

        private static void ConnectAsync(TcpClient client)
        {
            _ = Task.Run(async () =>
            {
                var stream = client.GetStream();
                var info = $"{Devices.Brand} {Devices.Model}";
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
                                    var directory = Path.Combine(Android.App.Application.Context.ExternalCacheDir.ToString(), $"{data.Description}-{DateTime.Now:MMddHHmmssfff}");
                                    using (var archive = new ZipArchive(zipStream))
                                    {
                                        archive.ExtractToDirectory(directory);
                                    }

                                    Globals.RunOnUiThread(() =>
                                    {
                                        ScriptManager.Instance.RunProject(directory, data.Description);
                                    });

                                    break;
                                }
                            case "saveProject":
                                {
                                    var directory = "/sdcard/astator.script";
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
    }
}
