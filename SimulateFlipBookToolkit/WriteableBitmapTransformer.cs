﻿using System;
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

        static readonly int Transparent = ColorExtention.GetColorInteger(Colors.Transparent);
        private const double MaskedShadowTheshold = 15;
        private const double NonMaskedShadowTheshold = 10;

        private void GenerateTransformedPixels(double a, double b, double c, int []pixels)
        {
            System.Diagnostics.Debug.Assert(pixels.Length == _pixels.Length);
            _pixels.CopyTo(pixels, 0);
            var transformer = new SimulateFlipBookWindowsPhoneRuntimeComponent.PointTransformer(a, b, c);
            for (int j_ = 0; j_ < _height; j_++)
            {
                var j = _height - j_;
                for (int i = 0; i < _width; i++)
                {
                    double pointX = i;
                    double pointY = j;
                    var distance = transformer.DistanceToAxis(pointX, pointY);
                    var needReflect = a > 0 ? distance > 0 : distance < 0;
                    var needShadow = b != 0 && !needReflect && (Math.Abs(distance) < MaskedShadowTheshold);
                    if (needShadow)
                    {
                        var theta = transformer.CalculateTheta(pointX, pointY);
                        var newPointX = transformer.TransformPointX(pointX, pointY, theta);
                        var newPointY = _height - transformer.TransformPointY(pointX, pointY, theta);
                        if (newPointX >= 0 && newPointX < _width && newPointY >= 0 && newPointY < _height)
                        {
                            
                        }
                        else
                        {
                            var color = GetPixelAtPoint(i, j_, pixels);
                            var colorBytes = ColorExtention.GetColorByte(color);
                            for (int k = 0; k < colorBytes.Length; k++)
                            {
                                colorBytes[k] = (byte)(colorBytes[k] * ((Math.Abs(distance) / MaskedShadowTheshold) * 0.8 + 0.2));
                            }
                            color = ColorExtention.GetColorInteger(colorBytes);
                            SetPixelAtPoint(i, j_, color, pixels);
                        }
                    }
                    else if (needReflect)
                    {
                        var color = GetPixelAtPoint(i, j_, pixels);
                        if (b != 0 && (Math.Abs(distance) < NonMaskedShadowTheshold))
                        {
                            var colorBytes = ColorExtention.GetColorByte(color);
                            for (int k = 0; k < colorBytes.Length; k++)
                            {
                                colorBytes[k] = (byte)(colorBytes[k] * ((Math.Abs(distance) / NonMaskedShadowTheshold) * 0.3 + 0.7));
                            }
                            color = ColorExtention.GetColorInteger(colorBytes);
                        }
                        var theta = transformer.CalculateTheta(pointX, pointY);
                        var newPointX = transformer.TransformPointX(pointX, pointY, theta);
                        var newPointY = transformer.TransformPointY(pointX, pointY, theta);
                        SetPixelAtPoint(i, j_, Transparent, pixels);
                        SetPixelAtPoint((int)newPointX, _height - (int)newPointY, color, pixels);
                        SetPixelAtPoint((int)newPointX - 1, _height - (int)newPointY, color, pixels);
                        SetPixelAtPoint((int)newPointX + 1, _height - (int)newPointY, color, pixels);
                        SetPixelAtPoint((int)newPointX, _height - (int)newPointY - 1, color, pixels);
                        SetPixelAtPoint((int)newPointX, _height - (int)newPointY + 1, color, pixels);
                    }
                }
            }
        }

        /// <summary>
        /// 以对称轴参数创建转换后的图片
        /// 公式为
        /// a*x + b*y + c = 0
        /// </summary>
        public void FillTransformedWriteableBitmap(double a, double b, double c, WriteableBitmap bitmap)
        {
            System.Diagnostics.Debug.Assert(bitmap.Pixels.Length == _pixels.Length);
            bitmap.Invalidate();
            GenerateTransformedPixels(a, b, c, bitmap.Pixels);
        }

        /// <summary>
        /// 以对称轴参数创建转换后的图片
        /// 公式为
        /// a*x + b*y + c = 0
        /// </summary>
        /// <returns>创建好的图像</returns>
        public WriteableBitmap GenerateTransformedWriteableBitmap(double a, double b, double c)
        {
            var result = new WriteableBitmap(_width, _height);
            FillTransformedWriteableBitmap(a, b, c, result);
            return result;
        }

    }

}
