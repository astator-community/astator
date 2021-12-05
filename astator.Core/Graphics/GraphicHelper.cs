using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
namespace astator.Core.Graphics
{
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

        public static GraphicHelper Create()
        {
            var helper = new GraphicHelper();
            if (helper.Initialize())
            {
                return helper;
            }
            return null;
        }

        private GraphicHelper()
        {

        }

        private bool Initialize()
        {
            return ReInitialize();
        }

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

        public string GetPixelStr(int x, int y)
        {
            var result = GetPixel(x, y);
            return $"0x{result[0]:x2}{result[1]:x2}{result[2]:x2}";
        }

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

        public bool CompareColor(int[] description, int sim, int offset)
        {
            var offsetLength = offset == 0 ? 1 : 9;
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

        public bool CompareColorEx(int[][] description, int sim, int offset)
        {
            foreach (var temp in description)
            {
                var array = new int[9];
                Array.Copy(temp, array, 9);
                if (!CompareColor(array, sim, offset))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CompareColorExLoop(int[][] description, int sim, int offset, int timeout, int timelag, int sign)
        {
            var num = timeout / timelag;
            if (sign == 0)
            {
                for (var i = 0; i < num; i++)
                {
                    if (KeepScreen(false))
                    {
                        if (CompareColorEx(description, sim, offset))
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
                        if (!CompareColorEx(description, sim, offset))
                        {
                            return true;
                        }
                    }
                    Thread.Sleep(timelag);
                }
            }
            return false;
        }

        private Point FindMultiColor(int startX, int startY, int endX, int endY, int[] firstDescription, int[][] offsetDescription, int offsetLength)
        {
            var r = firstDescription[2];
            var list = this.redList[r];
            int location;
            for (int i = 0, length = this.steps[r] - 1; i < length; i += 2)
            {
                if (list[i] >= startX && list[i] <= endX && list[i + 1] >= startY && list[i + 1] <= endY)
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
                else if (list[i] > endX && list[i + 1] > endY)
                {
                    break;
                }
            }
            return new Point(-1, -1);
        }

        public Point FindMultiColor(int startX, int startY, int endX, int endY, int[][] description, int sim, int offset)
        {
            startX = Math.Max(startX, 0);
            startY = Math.Max(startY, 0);
            endX = Math.Min(endX, this.width - 1);
            endY = Math.Min(endY, this.height - 1);
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
            var offsetLength = offset == 0 ? 1 : 9;
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
                    var point = FindMultiColor(startX, startY, endX, endY, firstDescription, offsetDescription, offsetLength);
                    if (point.X != -1)
                    {
                        return point;
                    }
                }
            }
            return new Point(-1, -1);
        }

        public Point FindMultiColor(int[] range, int[][] description, int sim, int offset)
        {
            return FindMultiColor(range[0], range[1], range[2], range[3], description, sim, offset);
        }

        public Point FindMultiColorLoop(int startX, int startY, int endX, int endY, int[][] description, int sim, int offset, int timeout, int timelag, int sign)
        {
            var num = timeout / timelag;
            if (sign == 0)
            {
                for (var i = 0; i < num; i++)
                {
                    if (KeepScreen(true))
                    {
                        var result = FindMultiColor(startX, startY, endX, endY, description, sim, offset);
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
                        var result = FindMultiColor(startX, startY, endX, endY, description, sim, offset);
                        if (result.X == -1)
                        {
                            return result;
                        }
                    }
                    Thread.Sleep(timelag);
                }
            }
            return FindMultiColor(startX, startY, endX, endY, description, sim, offset);
        }

        public Point FindMultiColorLoop(int[] range, int[][] description, int sim, int offset, int timeout, int timelag, int sign)
        {
            return FindMultiColorLoop(range[0], range[1], range[2], range[3], description, sim, offset, timeout, timelag, sign);
        }

        private List<Point> FindMultiColorEx(int startX, int startY, int endX, int endY, int[] firstDescription, int[][] offsetDescription)
        {
            var r = firstDescription[2];
            var list = this.redList[r];
            int location;
            List<Point> result = new();
            for (int i = 0, length = this.steps[r] - 1; i < length; i += 2)
            {
                if (list[i] >= startX && list[i] <= endX && list[i + 1] >= startY && list[i + 1] <= endY)
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
                else if (list[i] > endX && list[i + 1] > endY)
                {
                    break;
                }
            }
            return result;
        }

        public List<Point> FindMultiColorEx(int startX, int startY, int endX, int endY, int[][] description, int sim, int filterNum)
        {
            startX = Math.Max(startX, 0);
            startY = Math.Max(startY, 0);
            endX = Math.Min(endX, this.width - 1);
            endY = Math.Min(endY, this.height - 1);
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
                    var temp = FindMultiColorEx(startX, startY, endX, endY, firstDescription, offsetDescription);
                    if (temp.Count > 0)
                    {
                        points.AddRange(temp);
                    }
                }
            }
            List<Point> result = new();
            for (int i = 0, len = points.Count; i < len; i++)
            {
                var point = points[i];
                var isOverlap = false;
                for (int j = 0, size = result.Count; j < size; j++)
                {
                    var temp = result[j];
                    if (Math.Abs(point.X - temp.X) < 5 && Math.Abs(point.Y - temp.Y) < filterNum)
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

        public List<Point> FindMultiColorEx(int startX, int startY, int endX, int endY, int[][] description, int sim)
        {
            return FindMultiColorEx(startX, startY, endX, endY, description, sim, 5);
        }

        public List<Point> FindMultiColorEx(int[] range, int[][] description, int sim, int filterNum)
        {
            return FindMultiColorEx(range[0], range[1], range[2], range[3], description, sim, filterNum);
        }

        public List<Point> FindMultiColorEx(int[] range, int[][] description, int sim)
        {
            return FindMultiColorEx(range[0], range[1], range[2], range[3], description, sim, 5);
        }

    }
}
