using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class Complex : IShape
    {
        public ShapeType Type { get { return ShapeType.Complex; } }

        public bool IsClosed { get { return true; } }
    }
}