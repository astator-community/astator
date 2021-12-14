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
        private byte[] screenData;
        private readonly short[][] redList = new short[256][];
        private readonly int[] steps = new int[256];
        private readonly int[] thresholdCapacity = new int[256];

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
                var byteBuf = image.GetPlanes()[0].Buffer;
                this.width = image.Width;
                this.height = image.Height;
                this.rowStride = image.GetPlanes()[0].RowStride;
                this.screenData = new byte[this.rowStride * this.height];
                byteBuf.Position(0);
                byteBuf.Get(this.screenData, 0, this.rowStride * this.height);
                Array.Fill(this.thresholdCapacity, 19456);
                for (var i = 0; i < 256; i++)
                {
                    Array.Resize(ref this.redList[i], 20480);
                }
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
                    GetRedList();
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
            if (image is null)
            {
                return false;
            }
            var byteBuf = image.GetPlanes()[0].Buffer;
            byteBuf.Position(0);
            byteBuf.Get(this.screenData, 0, this.rowStride * this.height);
            image.Close();
            return true;
        }

        /// <summary>
        /// 获取所有r值对应的坐标
        /// </summary>
        public void GetRedList()
        {
            Array.Fill(this.steps, 0);
            int location;
            for (var i = 0; i < this.height; i++)
            {
                location = this.rowStride * i;
                for (var j = 0; j < this.width; j++)
                {
                    int k = this.screenData[location];
                    var step = this.steps[k];
                    if (step >= this.thresholdCapacity[k])
                    {
                        this.thresholdCapacity[k] = this.thresholdCapacity[k] + 20480;
                        Array.Resize(ref this.redList[k], this.redList[k].Length + 20480);
                    }
                    this.redList[k][step] = (short)j;
                    this.redList[k][step + 1] = (short)i;
                    location += 4;
                    this.steps[k] = step + 2;
                }
            }
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
                return (int)num + 1;
            if (local / 10 <= 4)
                return (int)num;
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
                var location = x * 4 + y * this.rowStride;
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
        /// <returns>颜色字符串, 例如:"0xffffff"</returns>
        public int GetPixelHex(int x, int y)
        {
            var result = GetPixel(x, y);
            return (result[0] & 0xff) << 16 | (result[1] & 0xff) << 8 | (result[2] & 0xff);
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
                    var location = _x * 4 + _y * this.rowStride;
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

        private bool CompareColorEx(int[][] description, int x, int y, int offsetLength)
        {
            foreach (var temp in description)
            {
                var array = new int[9];
                Array.Copy(temp, array, 9);
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
        public bool CompareColorEx(int[][] description, int sim, bool isOffset)
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
        public bool CompareColorExLoop(int[][] description, int sim, bool isOffset, int timeout, int timelag, bool sign)
        {
            var num = timeout / timelag;
            if (sign)
            {
                for (var i = 0; i < num; i++)
                {
                    if (KeepScreen(false))
                    {
                        if (CompareColorEx(description, sim, isOffset))
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
                        if (!CompareColorEx(description, sim, isOffset))
                        {
                            return true;
                        }
                    }
                    Thread.Sleep(timelag);
                }
            }
            return false;
        }

        private Point FindMultiColor(int sx, int sy, int ex, int ey, int[] firstDescription, int[][] offsetDescription, int offsetLength)
        {
            var r = firstDescription[2];
            var list = this.redList[r];
            int location;
            for (int i = 0, length = this.steps[r] - 1; i < length; i += 2)
            {
                if (list[i] >= sx && list[i] <= ex && list[i + 1] >= sy && list[i + 1] <= ey)
                {
                    location = list[i + 1] * this.rowStride + list[i] * 4;
                    if (Math.Abs((this.screenData[location + 1] & 0xff) - firstDescription[3]) <= firstDescription[6])
                    {
                        if (Math.Abs((this.screenData[location + 2] & 0xff) - firstDescription[4]) <= firstDescription[7])
                        {
                            if (CompareColorEx(offsetDescription, list[i], list[i + 1], offsetLength))
                            {
                                return new Point(list[i], list[i + 1]);
                            }
                        }
                    }
                }
                else if (list[i] > ex && list[i + 1] > ey)
                {
                    break;
                }
            }
            return new Point(-1, -1);
        }

        /// <summary>
        /// 多点找色
        /// </summary>
        /// <param name="sx">查找范围: startX</param>
        /// <param name="sy">查找范围: startY</param>
        /// <param name="ex">查找范围: endX</param>
        /// <param name="ey">查找范围: endY</param>
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="isOffset">是否偏移查找</param>
        /// <returns></returns>
        public Point FindMultiColor(int sx, int sy, int ex, int ey, int[][] description, int sim, bool isOffset)
        {
            sx = Math.Max(sx, 0);
            sy = Math.Max(sy, 0);
            ex = Math.Min(ex, this.width - 1);
            ey = Math.Min(ey, this.height - 1);
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
                    var point = FindMultiColor(sx, sy, ex, ey, firstDescription, offsetDescription, offsetLength);
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
        /// <param name="range">查找范围数组, new int[]{sx, sy, ex, ey}</param>
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="isOffset">是否偏移查找</param>
        /// <returns></returns>
        public Point FindMultiColor(int[] range, int[][] description, int sim, bool isOffset)
        {
            return FindMultiColor(range[0], range[1], range[2], range[3], description, sim, isOffset);
        }

        /// <summary>
        /// 条件循环多点找色
        /// </summary>
        /// <param name="sx">查找范围: startX</param>
        /// <param name="sy">查找范围: startY</param>
        /// <param name="ex">查找范围: endX</param>
        /// <param name="ey">查找范围: endY</param>
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="isOffset">是否偏移查找</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="timelag">间隔时间</param>
        /// <param name="sign">跳出条件,true为找色成功时返回,false为找色失败时返回</param>
        /// <returns></returns>
        public Point FindMultiColorLoop(int sx, int sy, int ex, int ey, int[][] description, int sim, bool isOffset, int timeout, int timelag, bool sign)
        {
            var num = timeout / timelag;
            if (sign)
            {
                for (var i = 0; i < num; i++)
                {
                    if (KeepScreen(true))
                    {
                        var result = FindMultiColor(sx, sy, ex, ey, description, sim, isOffset);
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
                        var result = FindMultiColor(sx, sy, ex, ey, description, sim, isOffset);
                        if (result.X == -1)
                        {
                            return result;
                        }
                    }
                    Thread.Sleep(timelag);
                }
            }
            return FindMultiColor(sx, sy, ex, ey, description, sim, isOffset);
        }

        /// <summary>
        /// 条件循环多点找色
        /// </summary>
        /// <param name="range">查找范围数组, new int[]{sx, sy, ex, ey}</param>
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="isOffset">是否偏移查找</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="timelag">间隔时间</param>
        /// <param name="sign">跳出条件,true为找色成功时返回,false为找色失败时返回</param>
        /// <returns></returns>
        public Point FindMultiColorLoop(int[] range, int[][] description, int sim, bool isOffset, int timeout, int timelag, bool sign)
        {
            return FindMultiColorLoop(range[0], range[1], range[2], range[3], description, sim, isOffset, timeout, timelag, sign);
        }

        private List<Point> FindMultiColorEx(int sx, int sy, int ex, int ey, int[] firstDescription, int[][] offsetDescription)
        {
            var r = firstDescription[2];
            var list = this.redList[r];
            int location;
            List<Point> result = new();
            for (int i = 0, length = this.steps[r] - 1; i < length; i += 2)
            {
                if (list[i] >= sx && list[i] <= ex && list[i + 1] >= sy && list[i + 1] <= ey)
                {
                    location = list[i + 1] * this.rowStride + list[i] * 4;
                    if (Math.Abs((this.screenData[location + 1] & 0xff) - firstDescription[3]) <= firstDescription[6])
                    {
                        if (Math.Abs((this.screenData[location + 2] & 0xff) - firstDescription[4]) <= firstDescription[7])
                        {
                            if (CompareColorEx(offsetDescription, list[i], list[i + 1], 1))
                            {
                                result.Add(new Point(list[i], list[i + 1]));
                            }
                        }
                    }
                }
                else if (list[i] > ex && list[i + 1] > ey)
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 多点找色
        /// </summary>
        /// <param name="sx">查找范围: startX</param>
        /// <param name="sy">查找范围: startY</param>
        /// <param name="ex">查找范围: endX</param>
        /// <param name="ey">查找范围: endY</param>
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="filterNum">过滤半径, 默认-1, 去除重叠区域</param>
        /// <returns>返回所有坐标</returns>
        public List<Point> FindMultiColorEx(int sx, int sy, int ex, int ey, int[][] description, int sim, int filterNum = -1)
        {
            sx = Math.Max(sx, 0);
            sy = Math.Max(sy, 0);
            ex = Math.Min(ex, this.width - 1);
            ey = Math.Min(ey, this.height - 1);
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
                    var temp = FindMultiColorEx(sx, sy, ex, ey, firstDescription, offsetDescription);
                    if (temp.Count > 0)
                    {
                        points.AddRange(temp);
                    }
                }
            }

            List<Point> result = new();

            int filterX = filterNum;
            int filterY = filterNum;
            if (filterNum == -1)
            {
                int left = 0;
                int top = 0;
                int right = filterNum;
                int bottom = filterNum;
                for (int i = 1; i < description.Length; i++)
                {
                    left = Math.Min(left, description[i][0]);
                    top = Math.Min(top, description[i][1]);
                    right = Math.Max(right, description[i][0]);
                    bottom = Math.Max(bottom, description[i][1]);
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
        /// 多点找色
        /// </summary>
        /// <param name="range">查找范围数组, new int[]{sx, sy, ex, ey}</param>
        /// <param name="description">色组描述</param>
        /// <param name="sim">相似度, 0~100</param>
        /// <param name="filterNum">过滤半径, 默认-1, 去除重叠区域</param>
        /// <returns>返回所有坐标</returns>
        public List<Point> FindMultiColorEx(int[] range, int[][] description, int sim, int filterNum = -1)
        {
            return FindMultiColorEx(range[0], range[1], range[2], range[3], description, sim, filterNum);
        }

    }
}
