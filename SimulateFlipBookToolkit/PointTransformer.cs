using System.Windows;

namespace SimulateFlipBookToolkit
{
    public class PointTransformer
    {

        private readonly double _a, _b, _c;

        /// <summary>
        /// 以对称轴参数创建转换器
        /// 公式为
        /// a*x + b*y + c = 0
        /// </summary>
        /// <param name="a">是否是垂直的</param>
        /// <param name="b">斜率</param>
        /// <param name="c">偏移</param>
        public PointTransformer(double a, double b, double c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        /// <summary>
        /// 转换点坐标
        /// </summary>
        /// <param name="point">原始点</param>
        /// <returns>转换后的点</returns>
        public Point TransformPoint(Point point)
        {
            var theta = (point.X*_a + point.Y*_b + _c)/(_a*_a + _b*_b);
            var x = point.X - 2*_a*theta;
            var y = point.Y - 2*_b*theta;
            return new Point(x, y);
        }

        /// <summary>
        /// 点是否在对称轴上方/左方
        /// </summary>
        /// <param name="point">点</param>
        /// <returns>点是否在对称轴上方/左方</returns>
        public bool IsAboveAxis(Point point)
        {
            return _a*point.X + _b*point.Y + _c < 0;
        }

        /// <summary>
        /// 点是否在对称轴下方/右方
        /// </summary>
        /// <param name="point">点</param>
        /// <returns>点是否在对称轴下方/右方</returns>
        public bool IsBelowAxis(Point point)
        {
            return _a*point.X + _b*point.Y + _c > 0;
        }

    }
}
