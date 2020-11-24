using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public class States
    {
        public Point P1 { get { return _p1; } }
        private Point _p1;

        public Point P2 { get { return _p2; } }
        private Point _p2;

        public bool IsPenDown { get { return _isPenDown; } }
        private bool _isPenDown;

        public bool IsRelative { get { return _isRelative; } }
        private bool _isRelative;

        public void Init()
        {

        }

        public void ToDefault()
        {

        }
    }
}