using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class Pen
    {
        public const double DefaultWidth = 0.35; // mm

        public Pen()
        {
            Init();
        }

        public double Width { get { return _width; } set { _width = value; } }
        private double _width;

        public LineWidthType WidthType { get { return _widthType; } }
        private LineWidthType _widthType;

        public void Init()
        {
            ToDefault();
            _width = DefaultWidth;
            _widthType = LineWidthType.Metric;
        }

        public void ToDefault()
        {

        }
    }
}