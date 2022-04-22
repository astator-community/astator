using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views.Accessibility;
using AndroidX.Core.App;
using AndroidX.Core.Graphics.Drawable;
using astator.Core.Accessibility;
using astator.Core.Graphics;
using astator.Core.Script;
using astator.LoggerProvider;
using astator.Modules.Base;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Server;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Text;

namespace astator.Modules;

[Service(Label = ".debug", Enabled = true)]
internal class DebugService : Service, IDisposable
{
    private static DebugService instance;
    public static DebugService Instance
    {
        get => instance;
        private set => instance = value;
    }

    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        Instance = this;
        StartNotification();

        var mode = intent.GetIntExtra("mode", 0);
        var ip = intent.GetStringExtra("ip");

        if (mode == 0)
            StartServer(ip);
        else
            ConnectServer(ip);

        return base.OnStartCommand(intent, flags, startId);
    }

    public override IBinder OnBind(Intent intent)
    {
        throw new NotImplementedException();
    }

    private IMqttServer server;

    public async void StartServer(string ip)
    {
        try
        {
            var mqttFactory = new MqttFactory();
            var mqttServerOptions = new MqttServerOptionsBuilder()
                .Build();

            this.server = mqttFactory.CreateMqttServer();

            Logger.Log($"开启调试服务: {ip}");

            this.server.UseClientConnectedHandler(async (e) =>
            {
                await this.server.SubscribeAsync(e.ClientId,
                    new MqttTopicFilter
                    {
                        Topic = "client/init"
                    },
                    new MqttTopicFilter
                    {
                        Topic = "client/run-project"
                    },
                    new MqttTopicFilter
                    {
                        Topic = "client/run-script"
                    },
                    new MqttTopicFilter
                    {
                        Topic = "client/save-project"
                    },
                    new MqttTopicFilter
                    {
                        Topic = "client/screen-shot"
                    },
                    new MqttTopicFilter
                    {
                        Topic = "client/layout-dump"
                    });

                AstatorLogger.AddCallback("debug-service", (level, time, msg) =>
                {
                    this.server.PublishAsync(new MqttApplicationMessage
                    {
                        Topic = "server/logging",
                        Payload = Stick.MakePackData(time.ToString("HH:mm:ss.fff"), Encoding.UTF8.GetBytes(msg))
                    });
                });
            });

            this.server.UseClientDisconnectedHandler(async (e) =>
            {
                AstatorLogger.RemoveCallback("debug-service");
                await this.server.UnsubscribeAsync(e.ClientId,
                    "client/run-project",
                    "client/run-script",
                    "client/save-project",
                    "client/screen-shot",
                    "client/layout-dump");
                this.server.Dispose();
            });

            this.server.UseApplicationMessageReceivedHandler(ApplicationMessageReceivedAsync);

            await this.server.StartAsync(mqttServerOptions);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            Dispose();
        }
    }

    private IMqttClient client;
    private bool isConnected;

    public async void ConnectServer(string ip)
    {
        try
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                   .WithTcpServer((op) =>
                   {
                       op.Server = ip;
                       op.BufferSize = 1024 * 1024 * 10;
                   })
                   .WithKeepAlivePeriod(TimeSpan.FromMinutes(6))
                   .Build();

            var mqttFactory = new MqttFactory();
            this.client = mqttFactory.CreateMqttClient();

            this.client.UseConnectedHandler(async (e) =>
            {
                this.isConnected = true;
                await this.client.SubscribeAsync(
                    new MqttTopicFilter
                    {
                        Topic = "client/init"
                    },
                    new MqttTopicFilter
                    {
                        Topic = "client/run-project"
                    },
                    new MqttTopicFilter
                    {
                        Topic = "client/run-script"
                    },
                    new MqttTopicFilter
                    {
                        Topic = "client/save-project"
                    },
                    new MqttTopicFilter
                    {
                        Topic = "client/screen-shot"
                    },
                    new MqttTopicFilter
                    {
                        Topic = "client/layout-dump"
                    });

                AstatorLogger.AddCallback("debug-service", (level, time, msg) =>
                {
                    this.client.PublishAsync(new MqttApplicationMessage
                    {
                        Topic = "server/logging",
                        Payload = Stick.MakePackData(time.ToString("HH:mm:ss.fff"), Encoding.UTF8.GetBytes(msg))
                    });
                });

                var pack = Stick.MakePackData(Devices.Brand, Devices.Model);
                await this.client.PublishAsync(new MqttApplicationMessage
                {
                    Topic = "server/init",
                    Payload = pack
                });
            });
            this.client.UseDisconnectedHandler(async (e) =>
            {
                AstatorLogger.RemoveCallback("debug-service");
                await this.client.UnsubscribeAsync("server/init", "server/logging");
                this.client.Dispose();
                this.isConnected = false;
            });

            this.client.UseApplicationMessageReceivedHandler(ApplicationMessageReceivedAsync);

            await this.client.ConnectAsync(mqttClientOptions, CancellationToken.None);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            Dispose();
        }
    }

    private async Task ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        var data = PackData.Parse(e.ApplicationMessage.Payload);
        if (data is null && e.ApplicationMessage.Topic != "client/init") return;

        switch (e.ApplicationMessage.Topic)
        {
            case "client/init":
            {
                var pack = Stick.MakePackData(Devices.Brand, Devices.Model);
                await this.server.PublishAsync(new MqttApplicationMessage
                {
                    Topic = "server/init",
                    Payload = pack
                });
                break;
            }
            case "client/run-project":
            {
                using var zipStream = new MemoryStream(data.Buffer);
                var directory = Path.Combine(MauiApplication.Current.ExternalCacheDir.ToString(), data.Key);
                ClearProject(directory);

                using (var archive = new ZipArchive(zipStream))
                {
                    archive.ExtractToDirectory(directory, true);
                }

                _ = ScriptManager.Instance.RunProject(directory);

                break;
            }
            case "client/run-script":
            {
                using var zipStream = new MemoryStream(data.Buffer);
                var directory = Path.Combine(MauiApplication.Current.ExternalCacheDir.ToString(), data.Key);
                ClearProject(directory);

                using (var archive = new ZipArchive(zipStream))
                {
                    archive.ExtractToDirectory(directory, true);
                }
                _ = ScriptManager.Instance.RunScript(Path.Combine(directory, data.Description));
                break;
            }
            case "client/save-project":
            {
                try
                {
                    var astatorDir = Android.OS.Environment.GetExternalStoragePublicDirectory("astator").ToString();
                    var directory = Path.Combine(astatorDir, "脚本");

                    using var zipStream = new MemoryStream(data.Buffer);
                    var saveDirectory = Path.Combine(directory, data.Key);
                    ClearProject(saveDirectory);

                    using (var archive = new ZipArchive(zipStream))
                    {
                        archive.ExtractToDirectory(saveDirectory, true);
                    }
                    Globals.Toast($"项目已保存至{saveDirectory}");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                break;
            }
            case "client/screen-shot":
            {
                byte[] pack;

                if (PermissionHelperer.CheckScreenCap())
                {
                    try
                    {
                        var img = await ScreenCapturer.Instance.AcquireLatestBitmapOnCurrentOrientation();
                        var ms = new MemoryStream();
                        img.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, ms);
                        var bytes = ms.ToArray();

                        pack = Stick.MakePackData("successed", bytes);
                    }
                    catch (Exception)
                    {
                        pack = Stick.MakePackData("failed", "获取截图失败!");
                    }
                }
                else
                {
                    pack = Stick.MakePackData("failed", "截图服务未开启!");
                }

                if (this.server.IsStarted)
                {
                    _ = this.server.PublishAsync(new MqttApplicationMessage
                    {
                        Topic = "server/screen-shot",
                        Payload = pack
                    });
                }

                if (this.client.IsConnected && this.isConnected)
                {
                    _ = this.client.PublishAsync(new MqttApplicationMessage
                    {
                        Topic = "server/screen-shot",
                        Payload = pack
                    });
                }

                break;
            }
            case "client/layout-dump":
            {
                byte[] pack;

                if (PermissionHelperer.CheckScreenCap() && PermissionHelperer.CheckAccessibility())
                {
                    try
                    {
                        var img = await ScreenCapturer.Instance.AcquireLatestBitmapOnCurrentOrientation();
                        var imgStream = new MemoryStream();
                        img.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, imgStream);
                        var imgBytes = imgStream.ToArray();

                        var layoutInfo = LayoutDump();
                        var js = JsonConvert.SerializeObject(layoutInfo);
                        var jsBytes = Encoding.UTF8.GetBytes(js);

                        var size = 4 + imgBytes.Length + 4 + jsBytes.Length;
                        using var ms = new MemoryStream(size);
                        ms.WriteInt32(imgBytes.Length);
                        ms.Write(imgBytes);
                        ms.WriteInt32(jsBytes.Length);
                        ms.Write(jsBytes);
                        var bytes = ms.GetBuffer();

                        pack = Stick.MakePackData("successed", bytes);
                    }
                    catch (Exception)
                    {
                        pack = Stick.MakePackData("failed", "布局获取失败!");
                    }
                }
                else
                {
                    pack = Stick.MakePackData("failed", "截图服务或无障碍服务未开启!");
                }

                await this.server.PublishAsync(new MqttApplicationMessage
                {
                    Topic = "server/layout-dump",
                    Payload = pack
                });

                if (this.server.IsStarted)
                {
                    _ = this.server.PublishAsync(new MqttApplicationMessage
                    {
                        Topic = "server/layout-dump",
                        Payload = pack
                    });
                }

                if (this.client.IsConnected && this.isConnected)
                {
                    _ = this.client.PublishAsync(new MqttApplicationMessage
                    {
                        Topic = "server/layout-dump",
                        Payload = pack
                    });
                }

                break;
            }
            default:
                break;
        }

    }

    private static MoveCategory LayoutDump(AccessibilityNodeInfo node = null)
    {
        node ??= Automator.GetCurrentWindowRoot();
        var layoutInfo = new MoveCategory
        {
            AccessibilityFocused = node.AccessibilityFocused,
            Bounds = node.GetBounds(),
            Checkable = node.Checkable,
            Checked = node.Checked,
            ClassName = node.ClassName,
            Clickable = node.Clickable,
            ContextClickable = node.ContextClickable,
            Depth = node.GetDepth(),
            Desc = node.ContentDescription,
            Dismissable = node.Dismissable,
            DrawingOrder = node.DrawingOrder,
            Editable = node.Editable,
            Enabled = node.Enabled,
            Focused = node.Focused,
            Id = node.GetId(),
            LongClickable = node.LongClickable,
            PackageName = node.PackageName,
            Scrollable = node.Scrollable,
            Selected = node.Selected,
            Text = node.Text,
            VisibleToUser = node.VisibleToUser
        };

        if (node.ChildCount > 0) layoutInfo.Children = new ObservableCollection<MoveCategory>();

        for (var i = 0; i < node.ChildCount; i++)
        {
            layoutInfo.Children.Add(LayoutDump(node.GetChild(i)));
        }
        return layoutInfo;
    }


    private static void ClearProject(string dir)
    {
        if (!Directory.Exists(dir))
        {
            return;
        }

        var searchPatterns = new string[] { "*.cs", "*.csproj" };

        foreach (var searchPattern in searchPatterns)
        {
            var files = Directory.GetFiles(dir, searchPattern, SearchOption.AllDirectories);
            if (files.Any())
            {
                foreach (var f in files)
                {
                    File.Delete(f);
                }
            }
        }
    }

    private void StartNotification()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            NotificationChannel channel = new("1002", "调试服务", NotificationImportance.Default);
            notificationManager.CreateNotificationChannel(channel);
        }

        var notification = new NotificationCompat.Builder(this, "1002")
          .SetContentTitle("调试服务正在运行中")
          .SetSmallIcon(IconCompat.CreateWithResource(this, Android.Resource.Drawable.SymDefAppIcon))
          .Build();

        StartForeground(1002, notification);
    }

    public override void OnTaskRemoved(Intent rootIntent)
    {
        Dispose();
        base.OnTaskRemoved(rootIntent);
    }


    private bool disposedValue;

    protected new virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                AstatorLogger.RemoveCallback("debug-service");
                this.server?.StopAsync();
                this.server?.Dispose();
                this.client?.Dispose();
                this.disposedValue = true;
                StopSelf();
                Instance = null;
            }
        }
    }

    ~DebugService()
    {
        Dispose(disposing: false);
    }

    public new void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
