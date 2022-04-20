using Android.Graphics;
using Java.Nio;
using System.IO;

namespace astator.Core.Graphics;

/// <summary>
/// image包装类
/// </summary>
public class WrapImage
{

    /// <summary>
    /// 图像宽度
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 图像高度
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 一行所占的字节数
    /// </summary>
    public int RowStride { get; set; }

    /// <summary>
    /// 像素通道数
    /// </summary>
    public int PxFormat { get; set; }

    /// <summary>
    /// 像素数据
    /// </summary>
    public byte[] Data { get; set; }

    private int[][] description;

    /// <summary>
    /// 从位图文件创建WrapImage对象
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static WrapImage CreateFromFile(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException(path);

        var bitmap = BitmapFactory.DecodeFile(path);
        var width = bitmap.Width;
        var height = bitmap.Height;

        var len = bitmap.ByteCount;
        var rowStride = len / height;
        var pxFormat = rowStride / width;
        var data = new byte[len];

        var byteBuf = ByteBuffer.Allocate(len);
        bitmap.CopyPixelsToBuffer(byteBuf);
        byteBuf.Position(0);
        byteBuf.Get(data, 0, len);
        bitmap.Recycle();
        bitmap.Dispose();
        return new(data, width, height, pxFormat, rowStride);
    }

    internal static WrapImage CreateFromBytes(byte[] data, int width, int height, int pxFormat, int rowStride)
        => new(data, width, height, pxFormat, rowStride);

    private WrapImage(byte[] data, int width, int height, int pxFormat, int rowStride)
    {
        this.Data = data;
        this.Width = width;
        this.Height = height;
        this.PxFormat = pxFormat;
        this.RowStride = rowStride;
    }

    /// <summary>
    /// 获取找图色组描述
    /// </summary>
    /// <returns></returns>
    public int[][] GetDescription()
    {
        if (this.description is null)
        {
            var result = new int[this.Width * this.Height][];
            var position = 0;
            var sx = -1;
            var sy = -1;
            for (var y = 0; y < this.Height; y++)
            {
                var location = y * this.RowStride;
                for (var x = 0; x < this.Width; x++, location += this.PxFormat)
                {
                    //当a通道为0时表示透明颜色, 跳过
                    if (this.Data[location + 3] != 0)
                    {
                        if (sx == -1 || sy == -1)
                        {
                            sx = x;
                            sy = y;
                        }

                        result[position] = new int[9];
                        result[position][0] = x - sx;
                        result[position][1] = y - sy;
                        result[position][2] = this.Data[location];
                        result[position][3] = this.Data[location + 1];
                        result[position][4] = this.Data[location + 2];
                        result[position][5] = result[position][6] = result[position][7] = result[position][8] = 0;
                        position++;
                    }
                }
            }
            this.description = result[0..position];
        }
        return this.description;
    }

    /// <summary>
    /// 获取bitmap对象
    /// </summary>
    /// <returns></returns>
    public Bitmap GetBitmap()
    {
        var result = Bitmap.CreateBitmap(this.Width, this.Height, Bitmap.Config.Argb8888);
        result.CopyPixelsFromBuffer(ByteBuffer.Wrap(this.Data));
        return result;
    }

    /// <summary>
    /// 以png格式保存到文件 
    /// </summary>
    /// <param name="path"></param>
    public void SaveToPng(string path)
    {
        var bitmap = GetBitmap();
        using var fs = new FileStream(path, FileMode.OpenOrCreate);
        bitmap.Compress(Bitmap.CompressFormat.Png, 100, fs);
        bitmap.Recycle();
        bitmap.Dispose();
    }
}
