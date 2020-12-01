using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public class ATInstruction : Instruction
    {
        public ATInstruction(Point inter, Point end, double chordAngle = 5)
        {
            _inter = inter;
            _end = end;
            _chordAngle = chordAngle;
        }

        public override InstructionType Type { get { return InstructionType.AT; } }

        public override bool AllowInPolygonMode { get { return true; } }

        public Point Inter { get { return _inter; } }
        private Point _inter;

        public Point End { get { return _end; } }
        private Point _end;

        public double ChordAngle { get { return _chordAngle; } }
        private double _chordAngle;

        public override void Execute(States states)
        {
            states.AT(_inter, _end, _chordAngle);
        }
    }
}