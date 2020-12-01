using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public struct PolyLine : IShape
    {
        public PolyLine(IEnumerable<Point> points)
        {
            _points = points.ToList();
            _isClosed = _points.Count > 3 && _points[0] == _points[_points.Count - 1];
        }

        public ShapeType Type { get { return ShapeType.PolyLine; } }

        public bool IsClosed { get { return _isClosed; } }
        private bool _isClosed;

        public IEnumerable<Point> Points { get { return _points; } }
        private List<Point> _points;
    }
}