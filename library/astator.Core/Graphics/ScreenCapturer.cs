using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Hardware.Display;
using Android.Media;
using Android.Media.Projection;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.App;
using AndroidX.Core.Graphics.Drawable;
using System;
using Orientation = Android.Content.Res.Orientation;

namespace astator.Core.Graphics
{
    [Service(Label = ".capturer", ForegroundServiceType = ForegroundService.TypeMediaProjection, Enabled = true)]
    public class ScreenCapturer : Service, IDisposable
    {
        public static ScreenCapturer Instance { get; set; }


        private MediaProjection mediaProjection;

        private ImageReader imageReader;

        private VirtualDisplay virtualDisplay;

        private Orientation currentOrientation;

        private Image lastImage;

        private static readonly object locker = new();

        public Image AcquireLatestImage()
        {
            lock (locker)
            {
                var image = this.imageReader.AcquireLatestImage();
                if (image is not null)
                {
                    this.lastImage = image;
                }
                return this.lastImage;
            }
        }

        public Bitmap AcquireLatestBitmap()
        {
            var image = AcquireLatestImage();
            if (image is not null)
            {
                var plane = image.GetPlanes()[0];
                if (plane is not null && plane.Buffer is not null)
                {
                    plane.Buffer.Position(0);
                    var bitmap = Bitmap.CreateBitmap(plane.RowStride / plane.PixelStride, image.Height, Bitmap.Config.Argb8888);
                    bitmap.CopyPixelsFromBuffer(plane.Buffer);
                    image.Close();
                    return bitmap;
                }
            }
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (Instance is not null)
            {
                Instance.Dispose();
            }

            StartNotification();

            var data = (Intent)intent.GetParcelableExtra("data");
            var manager = (MediaProjectionManager)GetSystemService("media_projection");
            this.mediaProjection = manager?.GetMediaProjection((int)Result.Ok, data);

            var width = Globals.Devices.Width;
            var height = Globals.Devices.Height;

            this.imageReader = ImageReader.NewInstance(width, height, (ImageFormatType)Format.Rgba8888, 2);
            this.virtualDisplay = this.mediaProjection?.CreateVirtualDisplay("ScreenCapturer", width, height, Globals.Devices.Dpi,
                  DisplayFlags.Round, this.imageReader.Surface, null, null);

            Instance = this;

            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            if (newConfig.Orientation == this.currentOrientation)
            {
                return;
            }
            else
            {
                this.currentOrientation = newConfig.Orientation;
            }

            this.imageReader?.Close();
            this.virtualDisplay?.Release();

            var width = Globals.Devices.Width;
            var height = Globals.Devices.Height;

            this.imageReader = ImageReader.NewInstance(width, height, (ImageFormatType)Format.Rgba8888, 2);
            this.virtualDisplay = this.mediaProjection?.CreateVirtualDisplay("ScreenCapturer", width, height, Globals.Devices.Dpi,
                  DisplayFlags.Round, this.imageReader.Surface, null, null);
        }

        private void StartNotification()
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                NotificationChannel channel = new("1000", "截屏服务", NotificationImportance.Default);
                notificationManager.CreateNotificationChannel(channel);
            }

            var notification = new NotificationCompat.Builder(this, "1000")
              .SetContentTitle("截屏服务正在运行中")
              .SetSmallIcon(IconCompat.TypeUnknown)
              .Build();

            StartForeground(1000, notification);

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
                    this.mediaProjection?.Dispose();
                    this.imageReader?.Dispose();
                    this.virtualDisplay?.Dispose();
                    this.disposedValue = true;
                    StopSelf();
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
}
