using System;
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

        private void GenerateTransformedPixels(double a, double b, double c, int []pixels)
        {
            System.Diagnostics.Debug.Assert(pixels.Length == _pixels.Length);
            _pixels.CopyTo(pixels, 0);
            var transparent = ColorExtention.GetColorInteger(Colors.Transparent);
            var transformer = new SimulateFlipBookWindowsPhoneRuntimeComponent.PointTransformer(a, b, c);
            for (int j_ = 0; j_ < _height; j_++)
            {
                var j = _height - j_;
                for (int i = 0; i < _width; i++)
                {
                    double pointX = i;
                    double pointY = j;
                    var needReflect = a > 0 ? transformer.IsBelowAxis(pointX, pointY) : transformer.IsAboveAxis(pointX, pointY);
                    if (needReflect)
                    {
                        var color = GetPixelAtPoint(i, j_, pixels);
                        var theta = transformer.CalculateTheta(pointX, pointY);
                        var newPointX = transformer.TransformPointX(pointX, pointY, theta);
                        var newPointY = transformer.TransformPointY(pointX, pointY, theta);
                        SetPixelAtPoint(i, j_, transparent, pixels);
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
