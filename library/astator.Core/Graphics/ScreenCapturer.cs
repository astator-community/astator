using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Hardware.Display;
using Android.Media;
using Android.Media.Projection;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.App;
using AndroidX.Core.Graphics.Drawable;
using astator.Core.Script;
using Java.Nio;
using System;
using System.Threading.Tasks;

namespace astator.Core.Graphics;

[Service(Label = ".capturer", ForegroundServiceType = ForegroundService.TypeMediaProjection, Enabled = true)]
public class ScreenCapturer : Service, IDisposable
{
    public static ScreenCapturer Instance { get; set; }

    private static readonly object locker = new();

    private MediaProjection mediaProjection;

    private ImageReader imageReader;

    private VirtualDisplay virtualDisplay;

    public Image AcquireLatestImage()
    {
        lock (locker)
        {
            return this.imageReader.AcquireLatestImage();
        }
    }

    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        Instance?.Dispose();

        StartNotification();

        var data = (Intent)intent.GetParcelableExtra("data");
        var isLandscape = intent.GetBooleanExtra("orientation", false);
        var manager = (MediaProjectionManager)GetSystemService("media_projection");
        this.mediaProjection = manager?.GetMediaProjection((int)Result.Ok, data);

        var w = Devices.Width;
        var h = Devices.Height;

        var width = isLandscape switch
        {
            true => Math.Max(w, h),
            false => Math.Min(w, h),
        };

        var height = isLandscape switch
        {
            true => Math.Min(w, h),
            false => Math.Max(w, h),
        };

        this.imageReader = ImageReader.NewInstance(width, height, (ImageFormatType)Format.Rgba8888, 2);
        this.virtualDisplay = this.mediaProjection?.CreateVirtualDisplay("ScreenCapturer", width, height, Devices.Dpi, DisplayFlags.Round, this.imageReader.Surface, null, null);

        Instance = this;

        return base.OnStartCommand(intent, flags, startId);
    }

    public void ResetOrientation(bool isLandscape)
    {
        this.imageReader?.Close();
        this.virtualDisplay?.Release();

        var w = Devices.Width;
        var h = Devices.Height;

        var width = isLandscape switch
        {
            true => Math.Max(w, h),
            false => Math.Min(w, h),
        };

        var height = isLandscape switch
        {
            true => Math.Min(w, h),
            false => Math.Max(w, h),
        };

        this.imageReader = ImageReader.NewInstance(width, height, (ImageFormatType)Format.Rgba8888, 2);
        this.virtualDisplay = this.mediaProjection?.CreateVirtualDisplay("ScreenCapturer", width, height, Devices.Dpi, DisplayFlags.Round, this.imageReader.Surface, null, null);
    }

    public async Task<Bitmap> AcquireLatestBitmapOnCurrentOrientation()
    {
        var width = Devices.Width;
        var height = Devices.Height;

        var imageReader = ImageReader.NewInstance(width, height, (ImageFormatType)Format.Rgba8888, 2);
        var virtualDisplay = this.mediaProjection?.CreateVirtualDisplay("ScreenCapturer", width, height, Devices.Dpi, DisplayFlags.Round, imageReader.Surface, null, null);
        await Task.Delay(200);
        var image = imageReader.AcquireLatestImage();
        Bitmap result = null;

        if (image is not null)
        {
            var plane = image.GetPlanes()[0];
            if (plane is not null && plane.Buffer is not null)
            {
                var rowStride = plane.RowStride;
                var pxStride = plane.PixelStride;
                var w = image.Width;
                var h = image.Height;
                var bmpRowStride = w * pxStride;

                var byteBuf = image.GetPlanes()[0].Buffer;
                var data = new byte[rowStride * h];
                var bmpData = new byte[w * pxStride * h];

                byteBuf.Position(0);
                byteBuf.Get(data, 0, data.Length);

                for (var i = 0; i < h; i++)
                {
                    System.Buffer.BlockCopy(data, rowStride * i, bmpData, bmpRowStride * i, bmpRowStride);
                }

                result = Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888);
                result.CopyPixelsFromBuffer(ByteBuffer.Wrap(bmpData));
                image.Close();
            }
        }
        imageReader.Close();
        imageReader.Dispose();
        virtualDisplay.Release();
        virtualDisplay.Dispose();

        return result;
    }


    private void StartNotification()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            NotificationChannel channel = new("1000", "截屏服务", NotificationImportance.Default);
            notificationManager.CreateNotificationChannel(channel);

            var notification = new NotificationCompat.Builder(this, "1000")
              .SetContentTitle("截屏服务正在运行中")
              .SetSmallIcon(IconCompat.CreateWithResource(this, Android.Resource.Drawable.SymDefAppIcon))
              .Build();

            StartForeground(1000, notification);
        }
    }


    public override void OnTaskRemoved(Intent rootIntent)
    {
        Dispose();
        base.OnTaskRemoved(rootIntent);
    }

    public override IBinder OnBind(Intent intent)
    {
        throw new NotImplementedException();
    }

    private bool disposedValue;

    protected new virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.mediaProjection?.Stop();
                this.mediaProjection?.Dispose();
                this.imageReader?.Close();
                this.imageReader?.Dispose();
                this.virtualDisplay?.Release();
                this.virtualDisplay?.Dispose();
                StopSelf();
                this.disposedValue = true;
                Instance = null;
            }
        }
    }

    ~ScreenCapturer()
    {
        Dispose(disposing: false);
    }

    public new void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}