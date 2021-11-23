using astator.Controllers;
using astator.Core;
using astator.Core.Script;
using Java.Net;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace astator.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private TcpListener tcpListener;
        CancellationTokenSource tokenSource;
        public SettingsPage()
        {
            InitializeComponent();
            this.NavBar.ActiveTab = "settings";
        }
        async void Connect_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
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
                    ScriptLogger.Instance.Log("开启调试服务");
                    while (true)
                    {
                        var client = await this.tcpListener.AcceptTcpClientAsync(this.tokenSource.Token);
                        ConnectAsync(client);
                    }
                }
                catch (OperationCanceledException)
                {
                    ScriptLogger.Instance.Log("停止调试服务");
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

        private void ConnectAsync(TcpClient client)
        {
            _ = Task.Run(async () =>
              {
                  var stream = client.GetStream();
                  var info = $"{Devices.Brand} {Devices.Model}";
                  await stream.WriteAsync(Stick.MakePackData("init", info));

                  var logger = ScriptLogger.Instance;

                  var key = logger.AddCallback("info", (args) =>
                  {
                      var pack = Stick.MakePackData("showMessage", $"[{info}] {args.Time:HH:mm:ss.fff}: {args.Message}");
                      stream.WriteAsync(pack);
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

                                      Device.BeginInvokeOnMainThread(() =>
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