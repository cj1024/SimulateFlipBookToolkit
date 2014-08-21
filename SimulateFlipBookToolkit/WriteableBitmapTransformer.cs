﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimulateFlipBookToolkit
{

    public class WriteableBitmapTransformer
    {

        private readonly int _width, _height;

        private readonly int[] _pixels;

        private WriteableBitmapTransformer(int width, int height, int[] pixels)
        {
            if (width > 0 && height > 0 && width * height == pixels.Length)
            {
                _pixels = new int[pixels.Length];
                _width = width;
                _height = height;
                pixels.CopyTo(_pixels, 0);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public WriteableBitmapTransformer(WriteableBitmap bitmap)
            : this(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.Pixels)
        {

        }
        
        private int GetPixelAtPoint(int x, int y, int []pixels)
        {
            return pixels[y * _width + x];
        }

        private void SetPixelAtPoint(int x, int y, int pixel, int[] pixels)
        {
            //_pixels 不应该被操作，这里做一个检查防止代码漏洞
            System.Diagnostics.Debug.Assert(_pixels != pixels);
            if ( x >= 0 && x < _width && y >=0 && y < _height)
            {
                pixels[y * _width + x] = pixel;
            }
        }

        private int[] GenerateTransformedPixels(double a, double b, double c)
        {
            var pixels = new int[_pixels.Length];
            _pixels.CopyTo(pixels, 0);
            var transparent = ColorExtention.GetColorInteger(Colors.Transparent);
            var transformer = new PointTransformer(a, b, c);
            var point = new Point(0, 0);
            for (int j_ = 0; j_ < _height; j_++)
            {
                var j = _height - j_;
                for (int i = 0; i < _width; i++)
                {
                    point.X = i;
                    point.Y = j;
                    var check = a > 0 ? transformer.IsBelowAxis(point) : transformer.IsAboveAxis(point);
                    if (check)
                    {
                        var color = GetPixelAtPoint(i, j_, pixels);
                        var newPoint = transformer.TransformPoint(point);
                        SetPixelAtPoint(i, j_, transparent, pixels);
                        SetPixelAtPoint((int)newPoint.X, _height - (int)newPoint.Y, color, pixels);
                    }
                }
            }
            return pixels;
        }

        /// <summary>
        /// 以对称轴参数创建转换后的图片
        /// 公式为
        /// a*x + b*y + c = 0
        /// </summary>
        /// <param name="a">是否是垂直的</param>
        /// <param name="b">斜率</param>
        /// <param name="c">偏移</param>
        /// <param name="bitmap">需要填充的图像</param>>
        public void FillTransformedWriteableBitmap(double a, double b, double c, WriteableBitmap bitmap)
        {
            System.Diagnostics.Debug.Assert(bitmap.Pixels.Length == _pixels.Length);
            var pixels = GenerateTransformedPixels(a, b, c);
            pixels.CopyTo(bitmap.Pixels, 0);
        }

        /// <summary>
        /// 以对称轴参数创建转换后的图片
        /// 公式为
        /// a*x + b*y + c = 0
        /// </summary>
        /// <param name="a">是否是垂直的</param>
        /// <param name="b">斜率</param>
        /// <param name="c">偏移</param>
        /// <returns>创建好的图像</returns>
        public WriteableBitmap GenerateTransformedWriteableBitmap(double a, double b, double c)
        {
            var result = new WriteableBitmap(_width, _height);
            FillTransformedWriteableBitmap(a, b, c, result);
            return result;
        }

    }

}
