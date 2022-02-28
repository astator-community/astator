using System;
using System.Collections.Generic;
using System.Drawing;
namespace astator.Core.Graphics
{
    /// <summary>
    /// 图色锚点查找类
    /// </summary>
    public class AnchorGraphicHelper
    {
        private readonly GraphicHelper baseGraphicHelper;
        private readonly double multiple;
        private readonly int left, top, right, bottom, center, devWidth, devHeight;

        /// <summary>
        /// 静态构造函数
        /// </summary>
        /// <param name="devWidth">开发分辨率的宽</param>
        /// <param name="devHeight">开发分辨率的高</param>
        /// <param name="left">运行分辨率布局 startX</param>
        /// <param name="top">运行分辨率布局 startY</param>
        /// <param name="right">运行分辨率布局 endX</param>
        /// <param name="bottom">运行分辨率布局 endY</param>
        /// <returns>AnchorGraphicHelper, 当初始化失败时返回null</returns>
        public static AnchorGraphicHelper Create(int devWidth, int devHeight, int left, int top, int right, int bottom)
        {
            var helper = new AnchorGraphicHelper(devWidth, devHeight, left, top, right, bottom);
            if (helper.baseGraphicHelper is not null)
            {
                return helper;
            }
            return null;
        }

        private AnchorGraphicHelper(int devWidth, int devHeight, int left, int top, int right, int bottom)
        {
            this.baseGraphicHelper = GraphicHelper.Create();
            this.devWidth = devWidth;
            this.devHeight = devHeight;
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            this.center = Round((this.right - this.left + 1.0) / 2) + this.left - 1;
            this.multiple = (this.bottom - this.top + 1.0) / this.devHeight;
        }

        /// <summary>
        /// 获取截图数据
        /// </summary>
        /// <param name="sign">是否需要调用多点找色</param>
        /// <returns></returns>
        public bool KeepScreen(bool sign)
        {
            return this.baseGraphicHelper.KeepScreen(sign);
        }

        /// <summary>
        /// 获取所有r值对应的坐标
        /// </summary>
        public void GetRedList()
        {
            this.baseGraphicHelper.GetRedList();
        }

        /// <summary>
        /// 四舍六入五成双
        /// </summary>
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
        /// 获取与运行分辨率相关的范围
        /// </summary>
        /// <returns></returns>
        public Rect GetBounds(int left, int top, int right, int bottom, params AnchorMode[] mode)
        {
            var smode = mode[0];
            var emode = mode.Length > 1 ? mode[1] : smode;
            var sPoint = GetPoint(left, top, smode);
            var ePoint = GetPoint(right, bottom, emode);
            return new Rect(sPoint.X, sPoint.Y, ePoint.X, ePoint.Y);
        }

        /// <summary>
        /// 获取与运行分辨率相关的坐标
        /// </summary>
        /// <param name="x">开发分辨率的x</param>
        /// <param name="y">开发分辨率的y</param>
        /// <param name="mode">坐标对齐方式</param>
        /// <returns></returns>
        public Point GetPoint(int x, int y, AnchorMode mode)
        {
            var result = new Point();
            if (mode == AnchorMode.Left || mode == AnchorMode.None)
            {
                result.X = Round(x * this.multiple) + this.left;
            }
            else if (mode == AnchorMode.Center)
            {
                result.X = Round(this.center - (this.devWidth / 2.0 - x - 1) * this.multiple);
            }
            else if (mode == AnchorMode.Right)
            {
                result.X = Round(this.right - (this.devWidth - x - 1) * this.multiple);
            }
            else if (mode == AnchorMode.RealPoint)
            {
                result.X = x;
                result.Y = y;
                return result;
            }
            result.Y = Round(y * this.multiple) + this.top;
            return result;
        }

        /// <summary>
        /// 获取指定像素数据
        /// </summary>
        /// <param name="x">开发分辨率的x</param>
        /// <param name="y">开发分辨率的y</param>
        /// <param name="mode">坐标对齐方式</param>
        /// <returns>rgb数组, 例如:"int[]{255,255,255}"</returns>
        public int[] GetPixel(int x, int y, AnchorMode mode)
        {
            var point = GetPoint(x, y, mode);
            return this.baseGraphicHelper.GetPixel(point.X, point.Y);
        }

        /// <summary>
        /// 获取指定像素数据
        /// </summary>
        /// <param name="x">开发分辨率的x</param>
        /// <param name="y">开发分辨率的y</param>
        /// <param name="mode">坐标对齐方式</param>
        /// <returns>颜色字符串, 例如:"0xffffff"</returns>
        public string GetPixelString(int x, int y, AnchorMode mode)
        {
            var point = GetPoint(x, y, mode);
            return this.baseGraphicHelper.GetPixelStr(point.X, point.Y);
        }

        /// <summary>
        /// 获取指定像素数据
        /// </summary>
        /// <param name="x">开发分辨率的x</param>
        /// <param name="y">开发分辨率的y</param>
        /// <param name="mode">坐标对齐方式</param>
        /// <returns>颜色字符串, 例如:0xffffff</returns>
        public int GetPixelHex(int x, int y, AnchorMode mode)
        {
            var point = GetPoint(x, y, mode);
            return this.baseGraphicHelper.GetPixelHex(point.X, point.Y);
        }

        /// <summary>
        /// 解析与运行分辨率相关的比色色组描述
        /// </summary>
        /// <param name="description">锚点色组字符串</param>
        /// <returns></returns>
        public int[][] ParseCmpColorString(string description)
        {
            var desc = description.Split(",");
            var devWidth = int.Parse(desc[0].Trim());
            var devHeight = int.Parse(desc[1].Trim());
            var multiple = (this.bottom - this.top + 1.0) / devHeight;

            var descSpan = desc.AsSpan(2);
            var result = new int[descSpan.Length][];
            for (var i = 0; i < descSpan.Length; i++)
            {
                result[i] = new int[9];
                var currentDesc = descSpan[i].Trim().Split("|");
                switch (currentDesc[0])
                {
                    case "left":
                    case "none":
                        result[i][0] = Round(int.Parse(currentDesc[1]) * multiple) + this.left;
                        break;
                    case "center":
                        result[i][0] = Round(this.center - (devWidth / 2.0 - int.Parse(currentDesc[1]) - 1) * multiple);
                        break;
                    case "right":
                        result[i][0] = Round(this.right - (devWidth - int.Parse(currentDesc[1]) - 1) * multiple);
                        break;
                }
                result[i][1] = Round(int.Parse(currentDesc[2]) * multiple) + this.top;
                var color = Convert.ToInt32(currentDesc[3], 16);
                result[i][2] = (color & 0xff0000) >> 16;
                result[i][3] = (color & 0xff00) >> 8;
                result[i][4] = color & 0xff;
                result[i][5] = result[i][6] = result[i][7] = result[i][8] = 0;
                if (currentDesc.Length >= 5)
                {
                    if (currentDesc[4].StartsWith("0x"))
                    {
                        var offsetColor = Convert.ToInt32(currentDesc[4], 16);
                        result[i][5] = (offsetColor & 0xff0000) >> 16;
                        result[i][6] = (offsetColor & 0xff00) >> 8;
                        result[i][7] = offsetColor & 0xff;
                    }
                    else
                    {
                        result[i][8] = int.Parse(currentDesc[4]);
                    }

                    if (currentDesc.Length == 6)
                    {
                        result[i][8] = int.Parse(currentDesc[5]);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 解析与运行分辨率相关的找色色组描述
        /// </summary>
        /// <param name="description">锚点色组字符串</param>
        /// <returns></returns>
        public int[][] ParseFindColorString(string description)
        {
            var desc = description.Split(",");
            var devWidth = int.Parse(desc[0].Trim());
            var devHeight = int.Parse(desc[1].Trim());
            var multiple = (this.bottom - this.top + 1.0) / devHeight;

            var descSpan = desc.AsSpan(2);
            var result = new int[descSpan.Length][];


            {
                result[0] = new int[9];
                var currentDesc = descSpan[0].Trim().Split("|");
                switch (currentDesc[0])
                {
                    case "left":
                    case "none":
                        result[0][0] = Round(int.Parse(currentDesc[1]) * multiple) + this.left;
                        break;
                    case "center":
                        result[0][0] = Round(this.center - (devWidth / 2.0 - int.Parse(currentDesc[1]) - 1) * multiple);
                        break;
                    case "right":
                        result[0][0] = Round(this.right - (devWidth - int.Parse(currentDesc[1]) - 1) * multiple);
                        break;
                }
                result[0][1] = Round(int.Parse(currentDesc[2]) * multiple) + this.top;
                var color = Convert.ToInt32(currentDesc[3], 16);
                result[0][2] = (color & 0xff0000) >> 16;
                result[0][3] = (color & 0xff00) >> 8;
                result[0][4] = color & 0xff;
                result[0][5] = result[0][6] = result[0][7] = result[0][8] = 0;
                if (currentDesc.Length >= 5)
                {
                    var offsetColor = Convert.ToInt32(currentDesc[4], 16);
                    result[0][5] = (offsetColor & 0xff0000) >> 16;
                    result[0][6] = (offsetColor & 0xff00) >> 8;
                    result[0][7] = offsetColor & 0xff;
                }
            }

            for (var i = 1; i < descSpan.Length; i++)
            {
                result[i] = new int[9];
                var currentDesc = descSpan[i].Trim().Split("|");
                switch (currentDesc[0])
                {
                    case "left":
                    case "none":
                        result[i][0] = Round(int.Parse(currentDesc[1]) * multiple) + this.left - result[0][0];
                        break;
                    case "center":
                        result[i][0] = Round(this.center - (devWidth / 2.0 - int.Parse(currentDesc[1]) - 1) * multiple) - result[0][0];
                        break;
                    case "right":
                        result[i][0] = Round(this.right - (devWidth - int.Parse(currentDesc[1]) - 1) * multiple) - result[0][0];
                        break;
                }
                result[i][1] = Round(int.Parse(currentDesc[2]) * multiple) + this.top - result[0][1];
                var color = Convert.ToInt32(currentDesc[3], 16);
                result[i][2] = (color & 0xff0000) >> 16;
                result[i][3] = (color & 0xff00) >> 8;
                result[i][4] = color & 0xff;
                result[i][5] = result[i][6] = result[i][7] = result[i][8] = 0;
                if (currentDesc.Length >= 5)
                {
                    if (currentDesc[4].StartsWith("0x"))
                    {
                        var offsetColor = Convert.ToInt32(currentDesc[4], 16);
                        result[i][5] = (offsetColor & 0xff0000) >> 16;
                        result[i][6] = (offsetColor & 0xff00) >> 8;
                        result[i][7] = offsetColor & 0xff;
                    }
                    else
                    {
                        result[i][8] = int.Parse(currentDesc[4]);
                    }

                    if (currentDesc.Length == 6)
                    {
                        result[i][8] = int.Parse(currentDesc[5]);
                    }
                }
            }
            return result;
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
            return this.baseGraphicHelper.CompareColor(description, sim, isOffset);
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
            return this.baseGraphicHelper.CompareMultiColor(description, sim, isOffset);
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
            return this.baseGraphicHelper.CompareMultiColorLoop(description, sim, isOffset, timeout, timelag, sign);
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
            return this.baseGraphicHelper.FindMultiColor(left, top, right, bottom, description, sim, isOffset);
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
        {
            return this.baseGraphicHelper.FindMultiColor(bounds, description, sim, isOffset);
        }

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
            return this.baseGraphicHelper.FindMultiColorLoop(left, top, right, bottom, description, sim, isOffset, timeout, timelag, sign);
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
        {
            return this.baseGraphicHelper.FindMultiColorLoop(bounds, description, sim, isOffset, timeout, timelag, sign);
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
            return this.baseGraphicHelper.FindMultiColorEx(left, top, right, bottom, description, sim, filterNum);
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
        {
            return this.baseGraphicHelper.FindMultiColorEx(bounds, description, sim, filterNum);
        }

    }
}
