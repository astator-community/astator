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
using astator.Core.Script;
using System;
using static astator.Core.Globals.Permission;

namespace astator.Core.Graphics
{
    [Service(Label = ".capturer", ForegroundServiceType = ForegroundService.TypeMediaProjection, Enabled = true)]
    public class ScreenCapturer : Service, IDisposable
    {
        public static ScreenCapturer Instance { get; set; }

        private MediaProjection mediaProjection;

        private ImageReader imageReader;

        private VirtualDisplay virtualDisplay;

        public Image AcquireLatestImage()
        {
            return this.imageReader.AcquireLatestImage();
        }
        public Bitmap AcquireLatestBitmap()
        {
            var image = this.imageReader.AcquireLatestImage();
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
            StartNotification();

            var data = (Intent)intent.GetParcelableExtra("data");
            var manager = (MediaProjectionManager)GetSystemService("media_projection");
            this.mediaProjection = manager?.GetMediaProjection((int)Result.Ok, data);
            var orientation = (CaptureOrientation)data.GetIntExtra("orientation", 0);
            var width = orientation == CaptureOrientation.Horizontal ? Devices.Height : Devices.Width;
            var height = orientation == CaptureOrientation.Horizontal ? Devices.Width : Devices.Height;
            this.imageReader = ImageReader.NewInstance(width, height, (ImageFormatType)Format.Rgba8888, 2);
            this.virtualDisplay = this.mediaProjection?.CreateVirtualDisplay("ScreenCapturer", width, height, Devices.Dpi,
                  DisplayFlags.Round, this.imageReader.Surface, null, null);

            if (Instance is not null)
            {
                Instance.Dispose();
            }

            Instance = this;

            return base.OnStartCommand(intent, flags, startId);
        }

        private void StartNotification()
        {
            var notification = new NotificationCompat.Builder(this, "1000")
                .SetContentTitle("astator")
                .SetContentText("截屏服务正在运行中")
                .Build();
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                NotificationChannel channel = new("1000", "astator截屏服务", NotificationImportance.Default);
                notificationManager.CreateNotificationChannel(channel);
            }
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
                }
                this.disposedValue = true;
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
