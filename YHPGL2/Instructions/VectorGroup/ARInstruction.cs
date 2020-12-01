using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public class ARInstruction : Instruction
    {
        public ARInstruction(Vector incr, double sweepAngle, double chordAngle = 5)
        {
            _incr = incr;
            _sweepAngle = sweepAngle;
            _chordAngle = chordAngle;
        }

        public override InstructionType Type { get { return InstructionType.AR; } }

        public override bool AllowInPolygonMode { get { return true; } }

        public Vector Incr { get { return _incr; } }
        private Vector _incr;

        public double SweepAngle { get { return _sweepAngle; } }
        private double _sweepAngle;

        public double ChordAngle { get { return _chordAngle; } }
        private double _chordAngle;

        public override void Execute(States states)
        {
            states.AR(_incr, _sweepAngle, _chordAngle);
        }
    }
}