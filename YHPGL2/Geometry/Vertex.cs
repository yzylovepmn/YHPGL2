using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public struct Vertex
    {
        public Vertex(Point point, bool isStroked)
        {
            Point = point;
            IsStroked = isStroked;
        }

        public Point Point;
        public bool IsStroked;
    }
}