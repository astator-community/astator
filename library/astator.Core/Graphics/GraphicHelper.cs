using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace astator.Core.Graphics
{
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

        private short[] redList;
        private readonly int[] marks = new int[512];

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
                this.redList = new short[this.width * this.height * 2];
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
                for (var i = 0; i < this.height; i++)
                {
                    var location = this.rowStride * i;
                    for (var j = 0; j < this.width; j++, location += this.pxFormat)
                    {
                        var k = ptr[location];
                        lens[k]++;
                    }
                }

                var mark = 0;
                for (var i = 0; i < 256; i++)
                {
                    this.marks[i * 2] = mark;
                    this.marks[i * 2 + 1] = mark;
                    mark += lens[i] * 2;
                }

                for (short i = 0; i < this.height; i++)
                {
                    var location = this.rowStride * i;
                    for (short j = 0; j < this.width; j++, location += this.pxFormat)
                    {
                        var k = ptr[location];
                        this.redList[this.marks[k * 2 + 1]] = j;
                        this.redList[this.marks[k * 2 + 1] + 1] = i;
                        this.marks[k * 2 + 1] += 2;
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

        /// <summary>
        /// 解析比色色组描述
        /// </summary>
        /// <param name="description">色组字符串</param>
        /// <returns></returns>
        public int[][] ParseCmpColorString(string description)
        {
            var desc = description.Split(",");
            var result = new int[desc.Length][];
            for (var i = 0; i < desc.Length; i++)
            {
                result[i] = new int[9];
                var currentDesc = desc[i].Trim().Split("|");
                result[i][0] = int.Parse(currentDesc[0]);
                result[i][1] = int.Parse(currentDesc[1]);

                var color = Convert.ToInt32(currentDesc[2], 16);
                result[i][2] = (color & 0xff0000) >> 16;
                result[i][3] = (color & 0xff00) >> 8;
                result[i][4] = color & 0xff;
                result[i][5] = result[i][6] = result[i][7] = result[i][8] = 0;
                if (currentDesc.Length >= 4)
                {
                    if (currentDesc[3].StartsWith("0x"))
                    {
                        var offsetColor = Convert.ToInt32(currentDesc[3], 16);
                        result[i][5] = (offsetColor & 0xff0000) >> 16;
                        result[i][6] = (offsetColor & 0xff00) >> 8;
                        result[i][7] = offsetColor & 0xff;
                    }
                    else
                    {
                        result[i][8] = int.Parse(currentDesc[3]);
                    }

                    if (currentDesc.Length == 5)
                    {
                        result[i][8] = int.Parse(currentDesc[4]);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 解析找色色组描述
        /// </summary>
        /// <param name="description">色组字符串</param>
        /// <returns></returns>
        public int[][] ParseFindColorString(string description)
        {
            var desc = description.Split(",");
            var result = new int[desc.Length][];

            {
                result[0] = new int[9];
                var currentDesc = desc[0].Trim().Split("|");
                result[0][0] = int.Parse(currentDesc[0]);
                result[0][1] = int.Parse(currentDesc[1]);

                var color = Convert.ToInt32(currentDesc[2], 16);
                result[0][2] = (color & 0xff0000) >> 16;
                result[0][3] = (color & 0xff00) >> 8;
                result[0][4] = color & 0xff;
                result[0][5] = result[0][6] = result[0][7] = result[0][8] = 0;
                if (currentDesc.Length >= 4)
                {
                    if (currentDesc[3].StartsWith("0x"))
                    {
                        var offsetColor = Convert.ToInt32(currentDesc[3], 16);
                        result[0][5] = (offsetColor & 0xff0000) >> 16;
                        result[0][6] = (offsetColor & 0xff00) >> 8;
                        result[0][7] = offsetColor & 0xff;
                    }
                }
            }

            for (var i = 1; i < desc.Length; i++)
            {
                result[i] = new int[9];
                var currentDesc = desc[i].Trim().Split("|");
                result[i][0] = int.Parse(currentDesc[0]) - result[0][0];
                result[i][1] = int.Parse(currentDesc[1]) - result[0][1];

                var color = Convert.ToInt32(currentDesc[2], 16);
                result[i][2] = (color & 0xff0000) >> 16;
                result[i][3] = (color & 0xff00) >> 8;
                result[i][4] = color & 0xff;
                result[i][5] = result[i][6] = result[i][7] = result[i][8] = 0;
                if (currentDesc.Length >= 4)
                {
                    if (currentDesc[3].StartsWith("0x"))
                    {
                        var offsetColor = Convert.ToInt32(currentDesc[3], 16);
                        result[i][5] = (offsetColor & 0xff0000) >> 16;
                        result[i][6] = (offsetColor & 0xff00) >> 8;
                        result[i][7] = offsetColor & 0xff;
                    }
                    else
                    {
                        result[i][8] = int.Parse(currentDesc[3]);
                    }

                    if (currentDesc.Length == 5)
                    {
                        result[i][8] = int.Parse(currentDesc[4]);
                    }
                }
            }
            return result;
        }

        private bool CompareColor(int[] description, int offsetX, int offsetY, int offsetLength)
        {
            var x = description[0] + offsetX;
            var y = description[1] + offsetY;
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
                    if (Math.Abs((this.screenData[location] & 0xff) - description[2]) <= description[5])
                    {
                        if (Math.Abs((this.screenData[location + 1] & 0xff) - description[3]) <= description[6])
                        {
                            if (Math.Abs((this.screenData[location + 2] & 0xff) - description[4]) <= description[7])
                            {
                                return description[8] == 0;
                            }
                        }
                    }
                }
            }
            return description[8] == 1;
        }

        /// <summary>
        /// 单点比色
        /// </summary>
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="isOffset">是否偏移查找</param>
        /// <returns></returns>
        public bool CompareColor(int[] description, int sim, bool isOffset)
        {
            var offsetLength = isOffset ? 9 : 1;
            var similarity = Round(255 - 255 * (sim / 100.0));
            var similarity_R = similarity + description[5];
            var similarity_G = similarity + description[6];
            var similarity_B = similarity + description[7];
            var temp = new int[]{
                description[0],
                description[1],
                description[2],
                description[3],
                description[4],
                similarity_R,
                similarity_G,
                similarity_B,
                description[8]
            };
            return CompareColor(temp, 0, 0, offsetLength);
        }

        private bool CompareMultiColor(int[][] description, int x, int y, int offsetLength)
        {
            foreach (var array in description)
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
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="isOffset">是否偏移查找</param>
        /// <returns></returns>
        public bool CompareMultiColor(int[][] description, int sim, bool isOffset)
        {
            foreach (var temp in description)
            {
                var array = new int[9];
                Array.Copy(temp, array, 9);
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
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度</param>
        /// <param name="isOffset">是否偏移查找</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="timelag">间隔时间</param>
        /// <param name="sign">跳出条件,true为找色成功时返回,false为找色失败时返回</param>
        /// <returns></returns>
        public bool CompareMultiColorLoop(int[][] description, int sim, bool isOffset, int timeout, int timelag, bool sign)
        {
            var num = timeout / timelag;
            if (sign)
            {
                for (var i = 0; i < num; i++)
                {
                    if (KeepScreen(false))
                    {
                        if (CompareMultiColor(description, sim, isOffset))
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
                        if (!CompareMultiColor(description, sim, isOffset))
                        {
                            return true;
                        }
                    }
                    Thread.Sleep(timelag);
                }
            }
            return false;
        }

        private unsafe Point FindMultiColor(int left, int top, int right, int bottom, int[] firstDescription, int[][] offsetDescription, int offsetLength)
        {
            var red = firstDescription[2];
            fixed (short* ptr = &this.redList[this.marks[red * 2]])
            {
                for (int i = 0, end = this.marks[red * 2 + 1] - this.marks[red * 2]; i < end; i += 2)
                {
                    var x = ptr[i];
                    var y = ptr[i + 1];
                    if (x >= left && x <= right && y >= top && y <= bottom)
                    {
                        var location = y * this.rowStride + x * this.pxFormat;
                        if (Math.Abs((this.screenData[location + 1] & 0xff) - firstDescription[3]) <= firstDescription[6])
                        {
                            if (Math.Abs((this.screenData[location + 2] & 0xff) - firstDescription[4]) <= firstDescription[7])
                            {
                                if (CompareMultiColor(offsetDescription, x, y, offsetLength))
                                {
                                    return new Point(x, y);
                                }
                            }
                        }
                    }
                    else if (x > right && y > bottom)
                    {
                        break;
                    }
                }
                return new Point(-1, -1);
            }
        }

        /// <summary>
        /// 多点找色
        /// </summary>
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="isOffset">是否偏移查找</param>
        /// <returns></returns>
        public Point FindMultiColor(int left, int top, int right, int bottom, int[][] description, int sim, bool isOffset)
        {
            left = Math.Max(left, 0);
            top = Math.Max(top, 0);
            right = Math.Min(right, this.width - 1);
            bottom = Math.Min(bottom, this.height - 1);
            var firstDescription = new int[9];
            Array.Copy(description[0], firstDescription, 9);
            var offsetDescription = new int[description.Length - 1][];
            var similarity = Round(255 - 255 * (sim / 100.0));
            firstDescription[5] = similarity + description[0][5];
            firstDescription[6] = similarity + description[0][6];
            firstDescription[7] = similarity + description[0][7];
            for (var j = 0; j < offsetDescription.Length; j++)
            {
                offsetDescription[j] = new int[9];
                Array.Copy(description[j + 1], offsetDescription[j], 9);
                offsetDescription[j][5] = similarity + description[j + 1][5];
                offsetDescription[j][6] = similarity + description[j + 1][6];
                offsetDescription[j][7] = similarity + description[j + 1][7];
            }
            var offsetLength = isOffset ? 9 : 1;
            var step = true;
            for (var i = 0; i < firstDescription[5]; i++)
            {
                int num;
                if (step)
                {
                    num = description[0][2] + i;
                    if (i != 0)
                    {
                        i--;
                        step = false;
                    }
                }
                else
                {
                    num = description[0][2] - i;
                    step = true;
                }
                if (num < 256 && num > -1)
                {
                    firstDescription[2] = num;
                    var point = FindMultiColor(left, top, right, bottom, firstDescription, offsetDescription, offsetLength);
                    if (point.X != -1)
                    {
                        return point;
                    }
                }
            }
            return new Point(-1, -1);
        }

        /// <summary>
        /// 多点找色
        /// </summary>
        /// <param name="bounds">查找范围</param>
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="isOffset">是否偏移查找</param>
        /// <returns></returns>
        public Point FindMultiColor(Rect bounds, int[][] description, int sim, bool isOffset)
            => FindMultiColor(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, description, sim, isOffset);

        /// <summary>
        /// 条件循环多点找色
        /// </summary>
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="isOffset">是否偏移查找</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="timelag">间隔时间</param>
        /// <param name="sign">跳出条件,true为找色成功时返回,false为找色失败时返回</param>
        /// <returns></returns>
        public Point FindMultiColorLoop(int left, int top, int right, int bottom, int[][] description, int sim, bool isOffset, int timeout, int timelag, bool sign)
        {
            var num = timeout / timelag;
            if (sign)
            {
                for (var i = 0; i < num; i++)
                {
                    if (KeepScreen(true))
                    {
                        var result = FindMultiColor(left, top, right, bottom, description, sim, isOffset);
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
                        var result = FindMultiColor(left, top, right, bottom, description, sim, isOffset);
                        if (result.X == -1)
                        {
                            return result;
                        }
                    }
                    Thread.Sleep(timelag);
                }
            }
            return FindMultiColor(left, top, right, bottom, description, sim, isOffset);
        }

        /// <summary>
        /// 条件循环多点找色
        /// </summary>
        /// <param name="bounds">查找范围</param>
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="isOffset">是否偏移查找</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="timelag">间隔时间</param>
        /// <param name="sign">跳出条件,true为找色成功时返回,false为找色失败时返回</param>
        /// <returns></returns>
        public Point FindMultiColorLoop(Rect bounds, int[][] description, int sim, bool isOffset, int timeout, int timelag, bool sign)
            => FindMultiColorLoop(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, description, sim, isOffset, timeout, timelag, sign);

        private unsafe List<Point> FindMultiColorEx(int left, int top, int right, int bottom, int[] firstDescription, int[][] offsetDescription)
        {
            var result = new List<Point>();
            var red = firstDescription[2];
            fixed (short* ptr = &this.redList[this.marks[red * 2]])
            {
                for (int i = 0, end = this.marks[red * 2 + 1] - this.marks[red * 2]; i < end; i += 2)
                {
                    var x = ptr[i];
                    var y = ptr[i + 1];
                    if (x >= left && x <= right && y >= top && y <= bottom)
                    {
                        var location = y * this.rowStride + x * this.pxFormat;
                        if (Math.Abs((this.screenData[location + 1] & 0xff) - firstDescription[3]) <= firstDescription[6])
                        {
                            if (Math.Abs((this.screenData[location + 2] & 0xff) - firstDescription[4]) <= firstDescription[7])
                            {
                                if (CompareMultiColor(offsetDescription, x, y, 1))
                                {
                                    result.Add(new Point(this.redList[i], this.redList[i + 1]));
                                }
                            }
                        }
                    }
                    else if (x > right && y > bottom)
                    {
                        break;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 多点找色ex
        /// </summary>
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="filterNum">过滤半径, 默认-1, 去除重叠区域</param>
        /// <returns>返回所有坐标</returns>
        public List<Point> FindMultiColorEx(int left, int top, int right, int bottom, int[][] description, int sim, int filterNum = -1)
        {
            left = Math.Max(left, 0);
            top = Math.Max(top, 0);
            right = Math.Min(right, this.width - 1);
            bottom = Math.Min(bottom, this.height - 1);
            var firstDescription = new int[9];
            Array.Copy(description[0], firstDescription, 9);
            var offsetDescription = new int[description.Length - 1][];
            var similarity = Round(255 - 255 * (sim / 100.0));
            firstDescription[5] = similarity + description[0][5];
            firstDescription[6] = similarity + description[0][6];
            firstDescription[7] = similarity + description[0][7];
            for (var j = 0; j < offsetDescription.Length; j++)
            {
                offsetDescription[j] = new int[9];
                Array.Copy(description[j + 1], offsetDescription[j], 9);
                offsetDescription[j][5] = similarity + description[j + 1][5];
                offsetDescription[j][6] = similarity + description[j + 1][6];
                offsetDescription[j][7] = similarity + description[j + 1][7];
            }
            List<Point> points = new();
            var step = true;
            for (var i = 0; i < firstDescription[5]; i++)
            {
                int num;
                if (step)
                {
                    num = description[0][2] + i;
                    if (i != 0)
                    {
                        i--;
                        step = false;
                    }
                }
                else
                {
                    num = description[0][2] - i;
                    step = true;
                }
                if (num < 256 && num > -1)
                {
                    firstDescription[2] = num & 0xff;
                    var temp = FindMultiColorEx(left, top, right, bottom, firstDescription, offsetDescription);
                    if (temp.Count > 0)
                    {
                        points.AddRange(temp);
                    }
                }
            }

            List<Point> result = new();

            var filterX = filterNum;
            var filterY = filterNum;
            if (filterNum == -1)
            {
                var _left = 0;
                var _top = 0;
                var _right = filterNum;
                var _bottom = filterNum;
                for (var i = 1; i < description.Length; i++)
                {
                    _left = Math.Min(_left, description[i][0]);
                    _top = Math.Min(_top, description[i][1]);
                    _right = Math.Max(_right, description[i][0]);
                    _bottom = Math.Max(_bottom, description[i][1]);
                }
                filterX = right - left;
                filterY = bottom - top;
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
                if (!isOverlap)
                {
                    result.Add(point);
                }
            }
            return result;
        }

        /// <summary>
        /// 多点找色ex
        /// </summary>
        /// <param name="bounds">查找范围</param>
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="filterNum">过滤半径, 默认-1, 去除重叠区域</param>
        /// <returns>返回所有坐标</returns>
        public List<Point> FindMultiColorEx(Rect bounds, int[][] description, int sim, int filterNum = -1)
            => FindMultiColorEx(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, description, sim, filterNum);

        /// <summary>
        /// 找图
        /// </summary>
        /// <param name="image">目标图像</param>
        /// <param name="sim">相似度</param>
        /// <returns></returns>
        public Point FindImage(int left, int top, int right, int bottom, WrapImage image, int sim)
            => FindMultiColor(left, top, right, bottom, image.GetDescription(), sim, false);

        /// <summary>
        /// 找图
        /// </summary>
        /// <param name="bounds">查找范围</param>
        /// <param name="image">目标图像</param>
        /// <param name="sim">相似度</param>
        /// <returns></returns>
        public Point FindImage(Rect bounds, WrapImage image, int sim)
            => FindMultiColor(bounds, image.GetDescription(), sim, false);

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
            => FindMultiColorLoop(left, top, right, bottom, image.GetDescription(), sim, false, timeout, timelag, sign);

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
        => FindMultiColorLoop(bounds, image.GetDescription(), sim, false, timeout, timelag, sign);

        /// <summary>
        /// 找图ex
        /// </summary>
        /// <param name="image">目标图像</param>
        /// <param name="sim">相似度</param>
        /// <returns>返回所有坐标</returns>
        public List<Point> FindImageEx(int left, int top, int right, int bottom, WrapImage image, int sim)
            => FindMultiColorEx(left, top, right, bottom, image.GetDescription(), sim);

        /// <summary>
        /// 找图ex
        /// </summary>
        /// <param name="bounds">查找范围</param>
        /// <param name="image">目标图像</param>
        /// <param name="sim">相似度</param>
        /// <returns>返回所有坐标</returns>
        public List<Point> FindImageEx(Rect bounds, WrapImage image, int sim)
            => FindMultiColorEx(bounds, image.GetDescription(), sim);

    }
}
