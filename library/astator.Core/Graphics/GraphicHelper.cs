using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace astator.Core.Graphics;

/// <summary>
/// 图色查找类
/// </summary>
public class GraphicHelper
{
    private readonly ScreenCapturer capturer = ScreenCapturer.Instance;
    private int width = -1;
    private int height = -1;
    private int rowStride = -1;
    private int pxFormat = -1;
    private byte[] screenData;

    private short[] redXList;
    private short[] redYList;
    private readonly int[] starts = new int[256];
    private readonly int[] ends = new int[256];

    /// <summary>
    /// 静态构造函数
    /// </summary>
    /// <returns>GraphicHelper, 当初始化失败时返回null</returns>
    public static GraphicHelper Create()
    {
        var helper = new GraphicHelper();
        if (helper.Initialize())
        {
            return helper;
        }
        return null;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    private bool Initialize()
    {
        return ReInitialize();
    }

    /// <summary>
    /// 重新初始化
    /// </summary>
    /// <returns></returns>
    public bool ReInitialize()
    {
        try
        {
            var image = this.capturer.AcquireLatestImage();
            if (image == null) return false;

            var byteBuf = image.GetPlanes()[0].Buffer;
            this.width = image.Width;
            this.height = image.Height;
            this.rowStride = image.GetPlanes()[0].RowStride;
            this.pxFormat = this.rowStride / this.width;
            this.screenData = new byte[this.rowStride * this.height];
            byteBuf.Position(0);
            byteBuf.Get(this.screenData, 0, this.rowStride * this.height);
            this.redXList = new short[this.width * this.height];
            this.redYList = new short[this.width * this.height];
            image.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取截图数据
    /// </summary>
    /// <param name="sign">是否需要调用多点找色</param>
    /// <returns></returns>
    public bool KeepScreen(bool sign)
    {
        if (AcquireLatestImage())
        {
            if (sign)
            {
                UpdateRedList();
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 从截图服务中获取最新一帧的图像
    /// </summary>
    /// <returns></returns>
    private bool AcquireLatestImage()
    {
        var image = this.capturer.AcquireLatestImage();
        if (image is null) return false;

        var byteBuf = image.GetPlanes()[0].Buffer;
        byteBuf.Position(0);
        byteBuf.Get(this.screenData, 0, this.rowStride * this.height);
        image.Close();
        return true;
    }

    /// <summary>
    /// 更新r值映射集合, 用于多点找色
    /// </summary>
    public unsafe void UpdateRedList()
    {
        fixed (byte* ptr = &this.screenData[0])
        {
            var lens = new int[256];
            for (var y = 0; y < this.height; y++)
            {
                var location = this.rowStride * y;
                for (var x = 0; x < this.width; x++, location += this.pxFormat)
                {
                    var r = ptr[location];
                    lens[r]++;
                }
            }

            var mark = 0;
            for (var i = 0; i < 256; i++)
            {
                this.starts[i] = mark;
                this.ends[i] = mark;
                mark += lens[i];
            }

            for (short y = 0; y < this.height; y++)
            {
                var location = this.rowStride * y;
                for (short x = 0; x < this.width; x++, location += this.pxFormat)
                {
                    var r = ptr[location];
                    this.redXList[this.ends[r]] = x;
                    this.redYList[this.ends[r]] = y;
                    this.ends[r]++;
                }
            }
        }
    }

    /// <summary>
    /// 获取WrapImage对象
    /// </summary>
    /// <returns></returns>
    public WrapImage GetImage()
    {
        return GetImage(0, 0, this.width - 1, this.height - 1);
    }

    /// <summary>
    /// 获取WrapImage对象
    /// </summary>
    /// <returns></returns>
    public WrapImage GetImage(Rect bounds)
    {
        return GetImage(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
    }

    /// <summary>
    /// 获取WrapImage对象
    /// </summary>
    /// <returns></returns>
    public WrapImage GetImage(int left, int top, int right, int bottom)
    {
        left = Math.Max(left, 0);
        top = Math.Max(top, 0);
        right = Math.Min(right, this.width - 1);
        bottom = Math.Min(bottom, this.height - 1);
        var width = right - left;
        var height = bottom - top;
        var data = new byte[width * height * 4];
        var site = 0;
        for (var i = top; i < bottom; i++)
        {
            var location = left * this.pxFormat + i * this.rowStride;
            for (var j = left; j < right; j++)
            {
                data[site] = this.screenData[location];
                data[site + 1] = this.screenData[location + 1];
                data[site + 2] = this.screenData[location + 2];
                data[site + 3] = 255;
                location += this.pxFormat;
                site += 4;
            }
        }
        return WrapImage.CreateFromBytes(data, width, height, this.pxFormat, this.rowStride);
    }

    /// <summary>
    /// 四舍六入五成双
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private static int Round(double num)
    {
        var local = (int)((num - (int)num) * 100);
        if (local / 10 >= 6)
        {
            return (int)num + 1;
        }

        if (local / 10 <= 4)
        {
            return (int)num;
        }

        if (local % 10 == 0)
        {
            if ((int)num % 2 == 0)
            {
                return (int)num;
            }
        }
        return (int)num + 1;
    }

    /// <summary>
    /// 获取指定像素数据
    /// </summary>
    /// <returns>rgb数组, 例如:"int[]{255,255,255}"</returns>
    public int[] GetPixel(int x, int y)
    {
        if (x >= 0 && x < this.width && y >= 0 && y < this.height)
        {
            var location = x * this.pxFormat + y * this.rowStride;
            var result = new int[3];
            result[0] = this.screenData[location];
            result[1] = this.screenData[location + 1];
            result[2] = this.screenData[location + 2];
            return result;
        }
        else
        {
            return new int[] { 0, 0, 0 };
        }
    }

    /// <summary>
    /// 获取指定像素数据
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>颜色字符串, 例如:"0xffffff"</returns>
    public string GetPixelStr(int x, int y)
    {
        var result = GetPixel(x, y);
        return $"0x{result[0]:x2}{result[1]:x2}{result[2]:x2}";
    }

    /// <summary>
    /// 获取指定像素数据
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>颜色值, 例如:0xffffff</returns>
    public int GetPixelHex(int x, int y)
    {
        var result = GetPixel(x, y);
        return (result[0] & 0xff) << 16 | (result[1] & 0xff) << 8 | (result[2] & 0xff);
    }

    public short[] GetCmpColorData(int x, int y, int color)
    {
        var result = new short[9];
        result[0] = (short)x;
        result[1] = (short)y;
        result[2] = (short)((color & 0xff0000) >> 16);
        result[3] = (short)((color & 0xff00) >> 8);
        result[4] = (short)(color & 0xff);
        result[5] = result[6] = result[7] = result[8] = 0;
        return result;
    }

    /// <summary>
    /// 解析比色色组数据
    /// </summary>
    /// <param name="str">色组字符串</param>
    /// <returns></returns>
    public short[][] ParseCmpColorString(string str)
    {
        var desc = str.Split(",");
        var result = new short[desc.Length][];
        for (var i = 0; i < desc.Length; i++)
        {
            result[i] = new short[9];
            var currentDesc = desc[i].Trim().Split("|");
            result[i][0] = short.Parse(currentDesc[0]);
            result[i][1] = short.Parse(currentDesc[1]);

            var color = Convert.ToInt32(currentDesc[2], 16);
            result[i][2] = (short)((color & 0xff0000) >> 16);
            result[i][3] = (short)((color & 0xff00) >> 8);
            result[i][4] = (short)(color & 0xff);
            result[i][5] = result[i][6] = result[i][7] = result[i][8] = 0;
            if (currentDesc.Length >= 4)
            {
                if (currentDesc[3].StartsWith("0x"))
                {
                    var offsetColor = Convert.ToInt32(currentDesc[3], 16);
                    result[i][5] = (short)((offsetColor & 0xff0000) >> 16);
                    result[i][6] = (short)((offsetColor & 0xff00) >> 8);
                    result[i][7] = (short)(offsetColor & 0xff);
                }
                else
                {
                    result[i][8] = short.Parse(currentDesc[3]);
                }

                if (currentDesc.Length == 5)
                {
                    result[i][8] = short.Parse(currentDesc[4]);
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 解析找色色组数据
    /// </summary>
    /// <param name="str">色组字符串</param>
    /// <returns></returns>
    public short[][] ParseFindColorString(string str)
    {
        var desc = str.Split(",");
        var result = new short[desc.Length][];

        {
            result[0] = new short[9];
            var currentDesc = desc[0].Trim().Split("|");
            result[0][0] = short.Parse(currentDesc[0]);
            result[0][1] = short.Parse(currentDesc[1]);

            var color = Convert.ToInt32(currentDesc[2], 16);
            result[0][2] = (short)((color & 0xff0000) >> 16);
            result[0][3] = (short)((color & 0xff00) >> 8);
            result[0][4] = (short)(color & 0xff);
            result[0][5] = result[0][6] = result[0][7] = result[0][8] = 0;
            if (currentDesc.Length >= 4)
            {
                if (currentDesc[3].StartsWith("0x"))
                {
                    var offsetColor = Convert.ToInt32(currentDesc[3], 16);
                    result[0][5] = (short)((offsetColor & 0xff0000) >> 16);
                    result[0][6] = (short)((offsetColor & 0xff00) >> 8);
                    result[0][7] = (short)(offsetColor & 0xff);
                }
            }
        }

        for (var i = 1; i < desc.Length; i++)
        {
            result[i] = new short[9];
            var currentDesc = desc[i].Trim().Split("|");
            result[i][0] = (short)(short.Parse(currentDesc[0]) - result[0][0]);
            result[i][1] = (short)(short.Parse(currentDesc[1]) - result[0][1]);

            var color = Convert.ToInt32(currentDesc[2], 16);
            result[i][2] = (short)((color & 0xff0000) >> 16);
            result[i][3] = (short)((color & 0xff00) >> 8);
            result[i][4] = (short)(color & 0xff);
            result[i][5] = result[i][6] = result[i][7] = result[i][8] = 0;
            if (currentDesc.Length >= 4)
            {
                if (currentDesc[3].StartsWith("0x"))
                {
                    var offsetColor = Convert.ToInt32(currentDesc[3], 16);
                    result[i][5] = (short)((offsetColor & 0xff0000) >> 16);
                    result[i][6] = (short)((offsetColor & 0xff00) >> 8);
                    result[i][7] = (short)(offsetColor & 0xff);
                }
                else
                {
                    result[i][8] = short.Parse(currentDesc[3]);
                }

                if (currentDesc.Length == 5)
                {
                    result[i][8] = short.Parse(currentDesc[4]);
                }
            }
        }
        return result;
    }

    #region 比色
    private bool CompareColor(short[] data, int offsetX, int offsetY, int offsetLength)
    {
        var x = data[0] + offsetX;
        var y = data[1] + offsetY;
        var offsetPoint = new Point[]
        {
            new Point(x, y),
            new Point(x - 1, y - 1),
            new Point(x - 1, y),
            new Point(x - 1, y + 1),
            new Point(x, y - 1),
            new Point(x, y + 1),
            new Point(x + 1, y - 1),
            new Point(x + 1, y),
            new Point(x + 1, y + 1),
        };
        for (var i = 0; i < offsetLength; i++)
        {
            var _x = offsetPoint[i].X;
            var _y = offsetPoint[i].Y;
            if (_x >= 0 && _x < this.width && _y >= 0 && _y < this.height)
            {
                var location = _y * this.rowStride + _x * this.pxFormat;
                if (Math.Abs(this.screenData[location] - data[2]) <= data[5])
                {
                    if (Math.Abs(this.screenData[location + 1] - data[3]) <= data[6])
                    {
                        if (Math.Abs(this.screenData[location + 2] - data[4]) <= data[7])
                        {
                            return data[8] == 0;
                        }
                    }
                }
            }
        }
        return data[8] == 1;
    }

    /// <summary>
    /// 单点比色
    /// </summary>
    /// <param name="data">色组描述</param>
    /// <param name="sim">相似度, 0~100</param>
    /// <param name="isOffset">是否偏移查找</param>
    /// <returns></returns>
    public bool CompareColor(short[] data, int sim, bool isOffset)
    {
        var offsetLength = isOffset ? 9 : 1;
        var similarity = Round(255 - 255 * (sim / 100.0));
        var similarity_R = (short)(similarity + data[5]);
        var similarity_G = (short)(similarity + data[6]);
        var similarity_B = (short)(similarity + data[7]);
        var temp = new short[]{
            data[0],
            data[1],
            data[2],
            data[3],
            data[4],
            similarity_R,
            similarity_G,
            similarity_B,
            data[8]
        };
        return CompareColor(temp, 0, 0, offsetLength);
    }

    private unsafe bool CompareMultiColor(short[][] data, int x, int y, int offsetLength)
    {
        foreach (var array in data)
        {
            if (!CompareColor(array, x, y, offsetLength))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 多点比色
    /// </summary>
    /// <param name="data">色组描述</param>
    /// <param name="sim">相似度, 0~100</param>
    /// <param name="isOffset">是否偏移查找</param>
    /// <returns></returns>
    public bool CompareMultiColor(short[][] data, int sim, bool isOffset)
    {
        foreach (var array in data)
        {
            if (!CompareColor(array, sim, isOffset))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 条件循环多点比色
    /// </summary>
    /// <param name="data">色组描述</param>
    /// <param name="sim">相似度</param>
    /// <param name="isOffset">是否偏移查找</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="timelag">间隔时间</param>
    /// <param name="sign">跳出条件,true为找色成功时返回,false为找色失败时返回</param>
    /// <returns></returns>
    public bool CompareMultiColorLoop(short[][] data, int sim, bool isOffset, int timeout, int timelag, bool sign)
    {
        var num = timeout / timelag;
        if (sign)
        {
            for (var i = 0; i < num; i++)
            {
                if (KeepScreen(false))
                {
                    if (CompareMultiColor(data, sim, isOffset))
                    {
                        return true;
                    }
                }
                Thread.Sleep(timelag);
            }
        }
        else
        {
            for (var i = 0; i < num; i++)
            {
                if (KeepScreen(false))
                {
                    if (!CompareMultiColor(data, sim, isOffset))
                    {
                        return true;
                    }
                }
                Thread.Sleep(timelag);
            }
        }
        return false;
    }
    #endregion

    #region 找色
    private unsafe Point FindMultiColor(int left, int top, int right, int bottom, short[] firstData, short[][] offsetData, int offsetLength)
    {
        var red = firstData[2];
        fixed (byte* screenDataPtr = &this.screenData[0])
        {
            fixed (short* redXPtr = &this.redXList[0], redYPtr = &this.redYList[0])
            {
                var mark = false;
                for (var j = 0; j <= firstData[5]; j++)
                {
                    var position = red + j;
                    if (position > 255 || position < 0) continue;
                    if (mark)
                    {
                        j = 0 - j; mark = false;
                    }
                    else
                    {
                        j = Math.Abs(j) + 1; mark = true;
                    }

                    var start = this.starts[position];
                    var end = this.ends[position];

                    for (var i = start; i < end; i++)
                    {
                        var x = redXPtr[i];
                        var y = redYPtr[i];
                        if (x >= left && x <= right && y >= top && y <= bottom)
                        {
                            var location = y * this.rowStride + x * this.pxFormat;
                            if (Math.Abs(screenDataPtr[location] - firstData[2]) <= firstData[5])
                            {
                                if (Math.Abs(screenDataPtr[location + 1] - firstData[3]) <= firstData[6])
                                {
                                    if (Math.Abs(screenDataPtr[location + 2] - firstData[4]) <= firstData[7])
                                    {
                                        if (CompareMultiColor(offsetData, x, y, offsetLength))
                                        {
                                            return new Point(x, y);
                                        }
                                    }
                                }
                            }
                        }
                        else if (x > right && y > bottom)
                        {
                            break;
                        }
                    }
                }
            }
        }
        return new Point(-1, -1);
    }

    private static (short[], short[][]) GetFindColorData(short[][] data, int sim)
    {
        var firstData = new short[9];
        Array.Copy(data[0], firstData, 9);
        var offsetData = new short[data.Length - 1][];
        var similarity = Round(255 - 255 * (sim / 100.0));
        firstData[5] = (short)(similarity + data[0][5]);
        firstData[6] = (short)(similarity + data[0][6]);
        firstData[7] = (short)(similarity + data[0][7]);
        for (var j = 0; j < offsetData.Length; j++)
        {
            offsetData[j] = new short[9];
            Array.Copy(data[j + 1], offsetData[j], 9);
            offsetData[j][5] = (short)(similarity + data[j + 1][5]);
            offsetData[j][6] = (short)(similarity + data[j + 1][6]);
            offsetData[j][7] = (short)(similarity + data[j + 1][7]);
        }
        return (firstData, offsetData);
    }

    /// <summary>
    /// 多点找色
    /// </summary>
    /// <param name="data">色组描述</param>
    /// <param name="sim">相似度, 0~100</param>
    /// <param name="isOffset">是否偏移查找</param>
    /// <returns></returns>
    public Point FindMultiColor(int left, int top, int right, int bottom, short[][] data, int sim, bool isOffset)
    {
        left = Math.Max(left, 0);
        top = Math.Max(top, 0);
        right = Math.Min(right, this.width - 1);
        bottom = Math.Min(bottom, this.height - 1);
        var (firstData, offsetData) = GetFindColorData(data, sim);
        var offsetLength = isOffset ? 9 : 1;
        return FindMultiColor(left, top, right, bottom, firstData, offsetData, offsetLength);
    }

    /// <summary>
    /// 多点找色
    /// </summary>
    /// <param name="bounds">查找范围</param>
    /// <param name="data">色组描述</param>
    /// <param name="sim">相似度, 0~100</param>
    /// <param name="isOffset">是否偏移查找</param>
    /// <returns></returns>
    public Point FindMultiColor(Rect bounds, short[][] data, int sim, bool isOffset)
        => FindMultiColor(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, data, sim, isOffset);

    /// <summary>
    /// 条件循环多点找色
    /// </summary>
    /// <param name="data">色组描述</param>
    /// <param name="sim">相似度, 0~100</param>
    /// <param name="isOffset">是否偏移查找</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="timelag">间隔时间</param>
    /// <param name="sign">跳出条件,true为找色成功时返回,false为找色失败时返回</param>
    /// <returns></returns>
    public Point FindMultiColorLoop(int left, int top, int right, int bottom, short[][] data, int sim, bool isOffset, int timeout, int timelag, bool sign)
    {
        var num = timeout / timelag;
        if (sign)
        {
            for (var i = 0; i < num; i++)
            {
                if (KeepScreen(true))
                {
                    var result = FindMultiColor(left, top, right, bottom, data, sim, isOffset);
                    if (result.X != -1)
                    {
                        return result;
                    }
                }
                Thread.Sleep(timelag);
            }
        }
        else
        {
            for (var i = 0; i < num; i++)
            {
                if (KeepScreen(true))
                {
                    var result = FindMultiColor(left, top, right, bottom, data, sim, isOffset);
                    if (result.X == -1)
                    {
                        return result;
                    }
                }
                Thread.Sleep(timelag);
            }
        }
        return FindMultiColor(left, top, right, bottom, data, sim, isOffset);
    }

    /// <summary>
    /// 条件循环多点找色
    /// </summary>
    /// <param name="bounds">查找范围</param>
    /// <param name="data">色组描述</param>
    /// <param name="sim">相似度, 0~100</param>
    /// <param name="isOffset">是否偏移查找</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="timelag">间隔时间</param>
    /// <param name="sign">跳出条件,true为找色成功时返回,false为找色失败时返回</param>
    /// <returns></returns>
    public Point FindMultiColorLoop(Rect bounds, short[][] data, int sim, bool isOffset, int timeout, int timelag, bool sign)
        => FindMultiColorLoop(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, data, sim, isOffset, timeout, timelag, sign);

    private unsafe List<Point> FindMultiColorEx(int left, int top, int right, int bottom, short[] firstData, short[][] offsetData)
    {
        var result = new List<Point>();
        var red = firstData[2];
        fixed (byte* screenDataPtr = &this.screenData[0])
        {
            fixed (short* redXPtr = &this.redXList[0], redYPtr = &this.redYList[0])
            {
                var mark = false;
                for (var j = 0; j <= firstData[5]; j++)
                {
                    var position = red + j;
                    if (position > 255 || position < 0) continue;
                    if (mark)
                    {
                        j = 0 - j; mark = false;
                    }
                    else
                    {
                        j = Math.Abs(j) + 1; mark = true;
                    }

                    var start = this.starts[position];
                    var end = this.ends[position];

                    for (var i = start; i < end; i++)
                    {
                        var x = redXPtr[i];
                        var y = redYPtr[i];
                        if (x >= left && x <= right && y >= top && y <= bottom)
                        {
                            var location = y * this.rowStride + x * this.pxFormat;
                            if (Math.Abs(screenDataPtr[location] - firstData[2]) <= firstData[5])
                            {
                                if (Math.Abs(screenDataPtr[location + 1] - firstData[3]) <= firstData[6])
                                {
                                    if (Math.Abs(screenDataPtr[location + 2] - firstData[4]) <= firstData[7])
                                    {
                                        if (CompareMultiColor(offsetData, x, y, 1))
                                        {
                                            result.Add(new Point(x, y));
                                        }
                                    }
                                }
                            }
                        }
                        else if (x > right && y > bottom)
                        {
                            break;
                        }
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 多点找色ex
    /// </summary>
    /// <param name="data">色组描述</param>
    /// <param name="sim">相似度, 0~100</param>
    /// <param name="filterNum">过滤半径, 默认-1, 去除重叠区域</param>
    /// <returns>返回所有坐标</returns>
    public List<Point> FindMultiColorEx(int left, int top, int right, int bottom, short[][] data, int sim, int filterNum = -1)
    {
        List<Point> result = new();

        left = Math.Max(left, 0);
        top = Math.Max(top, 0);
        right = Math.Min(right, this.width - 1);
        bottom = Math.Min(bottom, this.height - 1);
        var (firstData, offsetData) = GetFindColorData(data, sim);
        var points = FindMultiColorEx(left, top, right, bottom, firstData, offsetData);

        var filterX = filterNum;
        var filterY = filterNum;
        if (filterNum == -1)
        {
            var _left = 0;
            var _top = 0;
            var _right = filterNum;
            var _bottom = filterNum;
            for (var i = 1; i < data.Length; i++)
            {
                _left = Math.Min(_left, data[i][0]);
                _top = Math.Min(_top, data[i][1]);
                _right = Math.Max(_right, data[i][0]);
                _bottom = Math.Max(_bottom, data[i][1]);
            }
            filterX = _right - _left;
            filterY = _bottom - _top;
        }

        for (int i = 0, len = points.Count; i < len; i++)
        {
            var point = points[i];
            var isOverlap = false;
            for (int j = 0, size = result.Count; j < size; j++)
            {
                var temp = result[j];
                if (Math.Abs(point.X - temp.X) < filterX && Math.Abs(point.Y - temp.Y) < filterY)
                {
                    isOverlap = true;
                    break;
                }
            }
            if (!isOverlap) result.Add(point);
        }
        return result;
    }

    /// <summary>
    /// 多点找色ex
    /// </summary>
    /// <param name="bounds">查找范围</param>
    /// <param name="data">色组描述</param>
    /// <param name="sim">相似度, 0~100</param>
    /// <param name="filterNum">过滤半径, 默认-1, 去除重叠区域</param>
    /// <returns>返回所有坐标</returns>
    public List<Point> FindMultiColorEx(Rect bounds, short[][] data, int sim, int filterNum = -1)
        => FindMultiColorEx(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, data, sim, filterNum);
    #endregion

    #region 找图
    private unsafe bool CompareImage(WrapImage image, int offsetX, int offsetY, int similarity)
    {
        var data = image.GetFindImageData();
        fixed (byte* screenDataPtr = &this.screenData[0])
        {
            var position = 0;
            var offsetWidth = Math.Min(image.Width + offsetX, this.width);
            var offsetHeight = Math.Min(image.Height + offsetY, this.height);
            for (var y = offsetY; y < offsetHeight; y++)
            {
                var location = y * this.rowStride + offsetX * this.pxFormat;
                for (var x = offsetX; x < offsetWidth; x++, position++, location += this.pxFormat)
                {
                    if (data[position][3] == 1) continue;

                    if (Math.Abs(screenDataPtr[location] - data[position][0]) > similarity
                        || Math.Abs(screenDataPtr[location + 1] - data[position][1]) > similarity
                        || Math.Abs(screenDataPtr[location + 2] - data[position][2]) > similarity)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 找图
    /// </summary>
    /// <param name="image">目标图像</param>
    /// <param name="sim">相似度</param>
    /// <returns></returns>
    public unsafe Point FindImage(int left, int top, int right, int bottom, WrapImage image, int sim)
    {
        left = Math.Max(left, 0);
        top = Math.Max(top, 0);
        right = Math.Min(right, this.width);
        bottom = Math.Min(bottom, this.height);
        var data = image.GetFindImageData();
        var red = data[0][0];
        var similarity = Round(255 - 255 * (sim / 100.0));
        fixed (byte* screenDataPtr = &this.screenData[0])
        {
            fixed (short* redXPtr = &this.redXList[0], redYPtr = &this.redYList[0])
            {
                var mark = false;
                for (var j = 0; j <= similarity; j++)
                {
                    var position = red + j;
                    if (position > 255 || position < 0) continue;
                    if (mark)
                    {
                        j = 0 - j; mark = false;
                    }
                    else
                    {
                        j = Math.Abs(j) + 1; mark = true;
                    }

                    var start = this.starts[position];
                    var end = this.ends[position];

                    for (var i = start; i < end; i++)
                    {
                        var x = redXPtr[i];
                        var y = redYPtr[i];
                        if (x >= left && x < right && y >= top && y < bottom)
                        {
                            var location = y * this.rowStride + x * this.pxFormat;
                            if (Math.Abs(screenDataPtr[location] - data[0][0]) <= similarity)
                            {
                                if (Math.Abs(screenDataPtr[location + 1] - data[0][1]) <= similarity)
                                {
                                    if (Math.Abs(screenDataPtr[location + 2] - data[0][2]) <= similarity)
                                    {
                                        if (CompareImage(image, x, y, similarity))
                                        {
                                            return new Point(x, y);
                                        }
                                    }
                                }
                            }
                        }
                        else if (x >= right && y >= bottom)
                        {
                            break;
                        }
                    }
                }
            }
        }
        return new Point(-1, -1);
    }

    /// <summary>
    /// 找图
    /// </summary>
    /// <param name="bounds">查找范围</param>
    /// <param name="image">目标图像</param>
    /// <param name="sim">相似度</param>
    /// <returns></returns>
    public Point FindImage(Rect bounds, WrapImage image, int sim)
        => FindImage(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, image, sim);

    /// <summary>
    /// 条件循环找图
    /// </summary>
    /// <param name="image">目标图像</param>
    /// <param name="sim">相似度</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="timelag">间隔时间</param>
    /// <param name="sign">跳出条件,true为找图成功时返回,false为找图失败时返回</param>
    /// <returns></returns>
    public Point FindImageLoop(int left, int top, int right, int bottom, WrapImage image, int sim, int timeout, int timelag, bool sign)
    {
        var num = timeout / timelag;
        if (sign)
        {
            for (var i = 0; i < num; i++)
            {
                if (KeepScreen(true))
                {
                    var result = FindImage(left, top, right, bottom, image, sim);
                    if (result.X != -1)
                    {
                        return result;
                    }
                }
                Thread.Sleep(timelag);
            }
        }
        else
        {
            for (var i = 0; i < num; i++)
            {
                if (KeepScreen(true))
                {
                    var result = FindImage(left, top, right, bottom, image, sim);
                    if (result.X == -1)
                    {
                        return result;
                    }
                }
                Thread.Sleep(timelag);
            }
        }
        return FindImage(left, top, right, bottom, image, sim);
    }

    /// <summary>
    /// 条件循环找图
    /// </summary>
    /// <param name="bounds">查找范围</param>
    /// <param name="image">目标图像</param>
    /// <param name="sim">相似度</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="timelag">间隔时间</param>
    /// <param name="sign">跳出条件,true为找图成功时返回,false为找图失败时返回</param>
    /// <returns></returns>
    public Point FindImageLoop(Rect bounds, WrapImage image, int sim, int timeout, int timelag, bool sign)
    => FindImageLoop(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, image, sim, timeout, timelag, sign);

    /// <summary>
    /// 找图ex
    /// </summary>
    /// <param name="image">目标图像</param>
    /// <param name="sim">相似度</param>
    /// <returns>返回所有坐标</returns>
    public unsafe List<Point> FindImageEx(int left, int top, int right, int bottom, WrapImage image, int sim, int filterNum = -1)
    {
        var result = new List<Point>();
        var points = new List<Point>();
        left = Math.Max(left, 0);
        top = Math.Max(top, 0);
        right = Math.Min(right, this.width);
        bottom = Math.Min(bottom, this.height);
        var data = image.GetFindImageData();
        var red = data[0][0];
        var similarity = Round(255 - 255 * (sim / 100.0));
        fixed (byte* screenDataPtr = &this.screenData[0])
        {
            fixed (short* redXPtr = &this.redXList[0], redYPtr = &this.redYList[0])
            {
                var mark = false;
                for (var j = 0; j <= similarity; j++)
                {
                    var position = red + j;
                    if (position > 255 || position < 0) continue;
                    if (mark)
                    {
                        j = 0 - j; mark = false;
                    }
                    else
                    {
                        j = Math.Abs(j) + 1; mark = true;
                    }

                    var start = this.starts[position];
                    var end = this.ends[position];

                    for (var i = start; i < end; i++)
                    {
                        var x = redXPtr[i];
                        var y = redYPtr[i];
                        if (x >= left && x < right && y >= top && y < bottom)
                        {
                            var location = y * this.rowStride + x * this.pxFormat;
                            if (Math.Abs(screenDataPtr[location] - data[0][0]) <= similarity)
                            {
                                if (Math.Abs(screenDataPtr[location + 1] - data[0][1]) <= similarity)
                                {
                                    if (Math.Abs(screenDataPtr[location + 2] - data[0][2]) <= similarity)
                                    {
                                        if (CompareImage(image, x, y, similarity))
                                        {
                                            result.Add(new Point(x, y));
                                        }
                                    }
                                }
                            }
                        }
                        else if (x >= right && y >= bottom)
                        {
                            break;
                        }
                    }
                }
            }
        }

        var filterX = filterNum;
        var filterY = filterNum;
        if (filterNum == -1)
        {
            var _left = 0;
            var _top = 0;
            var _right = filterNum;
            var _bottom = filterNum;
            for (var i = 1; i < data.Length; i++)
            {
                _left = Math.Min(_left, data[i][0]);
                _top = Math.Min(_top, data[i][1]);
                _right = Math.Max(_right, data[i][0]);
                _bottom = Math.Max(_bottom, data[i][1]);
            }
            filterX = _right - _left;
            filterY = _bottom - _top;
        }

        for (int i = 0, len = points.Count; i < len; i++)
        {
            var point = points[i];
            var isOverlap = false;
            for (int j = 0, size = result.Count; j < size; j++)
            {
                var temp = result[j];
                if (Math.Abs(point.X - temp.X) < filterX && Math.Abs(point.Y - temp.Y) < filterY)
                {
                    isOverlap = true;
                    break;
                }
            }
            if (!isOverlap) result.Add(point);
        }
        return result;
    }

    /// <summary>
    /// 找图ex
    /// </summary>
    /// <param name="bounds">查找范围</param>
    /// <param name="image">目标图像</param>
    /// <param name="sim">相似度</param>
    /// <returns>返回所有坐标</returns>
    public List<Point> FindImageEx(Rect bounds, WrapImage image, int sim)
        => FindImageEx(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, image, sim);
    #endregion
}
