using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public class RTInstruction : Instruction
    {
        public RTInstruction(Vector inter, Vector end, double chordAngle = 5)
        {
            _inter = inter;
            _end = end;
            _chordAngle = chordAngle;
        }

        public override InstructionType Type { get { return InstructionType.RT; } }

        public override bool AllowInPolygonMode { get { return true; } }

        public Vector Inter { get { return _inter; } }
        private Vector _inter;

        public Vector End { get { return _end; } }
        private Vector _end;

        public double ChordAngle { get { return _chordAngle; } }
        private double _chordAngle;

        public override void Execute(States states)
        {
            states.RT(_inter, _end, _chordAngle);
        }
    }
}