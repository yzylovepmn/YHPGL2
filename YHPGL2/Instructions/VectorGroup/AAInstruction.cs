using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public class AAInstruction : Instruction
    {
        public AAInstruction(Point center, double sweepAngle, double chordAngle = 5)
        {
            _center = center;
            _sweepAngle = sweepAngle;
            _chordAngle = chordAngle;
        }

        public override InstructionType Type { get { return InstructionType.AA; } }

        public override bool AllowInPolygonMode { get { return true; } }

        public Point Center { get { return _center; } }
        private Point _center;

        public double SweepAngle { get { return _sweepAngle; } }
        private double _sweepAngle;

        public double ChordAngle { get { return _chordAngle; } }
        private double _chordAngle;

        public override void Execute(States states)
        {
            states.AA(_center, _sweepAngle, _chordAngle);
        }
    }
}