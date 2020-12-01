using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class Pen
    {
        public const double DefaultWidthMetric = 0.35; // mm
        public const double DefaultWidthRelative = 0.1; // percentage of the diagonal distance from P1 to P2

        public Pen()
        {
            Init();
        }

        public double Width { get { return _width; } set { _width = value; } }
        private double _width;

        public LineWidthType WidthType { get { return _widthType; } set { _widthType = value; } }
        private LineWidthType _widthType;

        public void Init()
        {
            _widthType = LineWidthType.Metric;
            ToDefault();
        }

        public void ToDefault()
        {
            switch (_widthType)
            {
                case LineWidthType.Metric:
                    _width = DefaultWidthMetric;
                    break;
                case LineWidthType.Relative:
                    _width = DefaultWidthRelative;
                    break;
            }
        }
    }
}