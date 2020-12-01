using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public enum ShapeType
    {
        PolyLine,
        Polygon,
        Complex,
    }

    public interface IShape
    {
        ShapeType Type { get; }

        bool IsClosed { get; }
    }
}