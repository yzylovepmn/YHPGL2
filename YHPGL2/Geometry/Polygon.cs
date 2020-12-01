using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public struct Polygon : IShape
    {
        public Polygon(IEnumerable<Vertex> vertice)
        {
            _vertice = vertice.ToList();
        }

        public ShapeType Type { get { return ShapeType.Polygon; } }

        public bool IsClosed { get { return true; } }

        public IEnumerable<Vertex> Vertice { get { return _vertice; } }
        private List<Vertex> _vertice;
    }
}