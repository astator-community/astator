using System;
using System.Collections.Generic;
using System.Drawing;
namespace astator.Core.Graphics
{
    public class AnchorGraphicHelper
    {
        private static readonly int NONE = -1;
        private static readonly int LEFT = 0;
        private static readonly int CENTER = 1;
        private static readonly int RIGHT = 2;
        private readonly GraphicHelper baseGraphicHelper;
        private readonly double multiple;
        private readonly int left, top, right, bottom, center, devWidth, devHeight;
        public AnchorGraphicHelper(int devWidth, int devHeight, int left, int top, int right, int bottom)
        {
            this.baseGraphicHelper = new GraphicHelper();
            this.devWidth = devWidth;
            this.devHeight = devHeight;
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            this.center = Round((this.right - this.left + 1.0) / 2) + this.left - 1;
            this.multiple = (this.bottom - this.top + 1.0) / this.devHeight;
        }
        public AnchorGraphicHelper(int[] description) : this(description[0], description[1], description[2], description[3], description[4], description[5])
        { }
        public void GetRedList()
        {
            this.baseGraphicHelper.GetRedList();
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
        public int[] GetRange(int sx, int sy, int smode, int ex, int ey, int emode)
        {
            var result = new int[4];
            var startPoint = GetPoint(sx, sy, smode);
            var endPoint = GetPoint(ex, ey, emode);
            result[0] = startPoint.X;
            result[1] = startPoint.Y;
            result[2] = endPoint.X;
            result[3] = endPoint.Y;
            return result;
        }
        public int[] GetRange(int sx, int sy, int ex, int ey, int mode)
        {
            return GetRange(sx, sy, mode, ex, ey, mode);
        }
        public Point GetPoint(int x, int y, int mode)
        {
            Point result = new();
            if (mode == LEFT || mode == NONE)
            {
                result.X = Round(x * this.multiple) + this.left;
            }
            else if (mode == CENTER)
            {
                result.X = Round(this.center - (this.devWidth / 2.0 - x - 1) * this.multiple);
            }
            else if (mode == RIGHT)
            {
                result.X = Round(this.right - (this.devWidth - x - 1) * this.multiple);
            }
            result.Y = Round(y * this.multiple) + this.top;
            return result;
        }
        public int[] GetPixel(int x, int y, int mode)
        {
            var point = GetPoint(x, y, mode);
            return this.baseGraphicHelper.GetPixel(point.X, point.Y);
        }
        public String GetPixelStr(int x, int y, int mode)
        {
            var point = GetPoint(x, y, mode);
            return this.baseGraphicHelper.GetPixelStr(point.X, point.Y);
        }
        public int GetPixelHex(int x, int y, int mode)
        {
            var point = GetPoint(x, y, mode);
            return this.baseGraphicHelper.GetPixelHex(point.X, point.Y);
        }
        public int[][] GetCmpColorArray(int devWidth, int devHeight, int[][] description)
        {
            var multiple = (this.bottom - this.top + 1.0) / devHeight;
            var result = new int[description.Length][];
            for (var i = 0; i < description.Length; i++)
            {
                result[i] = new int[9];
                if (description[i][0] == LEFT || description[i][0] == NONE)
                {
                    result[i][0] = Round(description[i][1] * multiple) + this.left;
                }
                else if (description[i][0] == CENTER)
                {
                    result[i][0] = Round(this.center - (devWidth / 2.0 - description[i][1] - 1) * multiple);
                }
                else if (description[i][0] == RIGHT)
                {
                    result[i][0] = Round(this.right - (devWidth - description[i][1] - 1) * multiple);
                }
                result[i][1] = Round(description[i][2] * multiple) + this.top;
                result[i][2] = (description[i][3] & 0xff0000) >> 16;
                result[i][3] = (description[i][3] & 0xff00) >> 8;
                result[i][4] = description[i][3] & 0xff;
                result[i][5] = result[i][6] = result[i][7] = result[i][8] = 0;
                if (description[i].Length >= 5)
                {
                    if (description[i].Length == 6)
                    {
                        result[i][8] = description[i][5];
                    }
                    else if (description[i][4] == 1)
                    {
                        result[i][8] = description[i][4];
                    }
                    else
                    {
                        result[i][5] = (description[i][4] & 0xff0000) >> 16;
                        result[i][6] = (description[i][4] & 0xff00) >> 8;
                        result[i][7] = description[i][4] & 0xff;
                    }
                }
            }
            return result;
        }
        public int[][] GetFindColorArray(int devWidth, int devHeight, int[][] description)
        {
            var result = new int[description.Length][];
            result[0] = new int[9];
            var multiple = (this.bottom - this.top + 1.0) / devHeight;
            if (description[0][0] == LEFT || description[0][0] == NONE)
            {
                result[0][0] = Round(description[0][1] * multiple) + this.left;
            }
            else if (description[0][0] == CENTER)
            {
                result[0][0] = Round(this.center - (devWidth / 2.0 - description[0][1] - 1) * multiple);
            }
            else if (description[0][0] == RIGHT)
            {
                result[0][0] = Round(this.right - (devWidth - description[0][1] - 1) * multiple);
            }
            result[0][1] = Round(description[0][2] * multiple);
            result[0][2] = (description[0][3] & 0xff0000) >> 16;
            result[0][3] = (description[0][3] & 0xff00) >> 8;
            result[0][4] = description[0][3] & 0xff;
            result[0][5] = result[0][6] = result[0][7] = 0;
            if (description[0].Length == 5)
            {
                result[0][5] = (description[0][4] & 0xff0000) >> 16;
                result[0][6] = (description[0][4] & 0xff00) >> 8;
                result[0][7] = description[0][4] & 0xff;
            }
            for (var i = 1; i < description.Length; i++)
            {
                result[i] = new int[9];
                if (description[i][0] == LEFT || description[i][0] == NONE)
                {
                    result[i][0] = Round(description[i][1] * multiple) + this.left - result[0][0];
                }
                else if (description[i][0] == CENTER)
                {
                    result[i][0] = Round(this.center - (devWidth / 2.0 - description[i][1] - 1) * multiple) - result[0][0];
                }
                else if (description[i][0] == RIGHT)
                {
                    result[i][0] = Round(this.right - (devWidth - description[i][1] - 1) * multiple) - result[0][0];
                }
                result[i][1] = Round(description[i][2] * multiple) + this.top - result[0][1];
                result[i][2] = (description[i][3] & 0xff0000) >> 16;
                result[i][3] = (description[i][3] & 0xff00) >> 8;
                result[i][4] = description[i][3] & 0xff;
                result[i][5] = result[i][6] = result[i][7] = result[i][8] = 0;
                if (description[i].Length >= 5)
                {
                    if (description[i].Length == 6)
                    {
                        result[i][8] = description[i][5];
                    }
                    else if (description[i][4] == 1)
                    {
                        result[i][8] = description[i][4];
                    }
                    else
                    {
                        result[i][5] = (description[i][4] & 0xff0000) >> 16;
                        result[i][6] = (description[i][4] & 0xff00) >> 8;
                        result[i][7] = description[i][4] & 0xff;
                    }
                }
            }
            return result;
        }
        public bool CompareColor(int[] description, int sim, int offset)
        {
            return this.baseGraphicHelper.CompareColor(description, sim, offset);
        }
        public bool CompareColorEx(int[][] description, int sim, int offset)
        {
            return this.baseGraphicHelper.CompareColorEx(description, sim, offset);
        }
        public bool CompareColorExLoop(int[][] description, int sim, int offset, int timeout, int timelag, int sign)
        {
            return this.baseGraphicHelper.CompareColorExLoop(description, sim, offset, timeout, timelag, sign);
        }
        public Point FindMultiColor(int startX, int startY, int endX, int endY, int[][] description, int sim, int offset)
        {
            return this.baseGraphicHelper.FindMultiColor(startX, startY, endX, endY, description, sim, offset);
        }
        public Point FindMultiColor(int[] range, int[][] description, int sim, int offset)
        {
            return this.baseGraphicHelper.FindMultiColor(range[0], range[1], range[2], range[3], description, sim, offset);
        }
        public Point FindMultiColorLoop(int startX, int startY, int endX, int endY, int[][] description, int sim, int offset, int timeout, int timelag, int sign)
        {
            return this.baseGraphicHelper.FindMultiColorLoop(startX, startY, endX, endY, description, sim, offset, timeout, timelag, sign);
        }
        public Point FindMultiColorLoop(int[] range, int[][] description, int sim, int offset, int timeout, int timelag, int sign)
        {
            return this.baseGraphicHelper.FindMultiColorLoop(range[0], range[1], range[2], range[3], description, sim, offset, timeout, timelag, sign);
        }
        public List<Point> FindMultiColorEx(int startX, int startY, int endX, int endY, int[][] description, int sim)
        {
            return this.baseGraphicHelper.FindMultiColorEx(startX, startY, endX, endY, description, sim);
        }
        public List<Point> FindMultiColorEx(int[] range, int[][] description, int sim)
        {
            return this.baseGraphicHelper.FindMultiColorEx(range[0], range[1], range[2], range[3], description, sim);
        }
    }
}
