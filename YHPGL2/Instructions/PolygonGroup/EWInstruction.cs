using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class EWInstruction : Instruction
    {
        public EWInstruction(double radius, double startAngle, double sweepAngle, double chordAngle = 5)
        {
            _radius = radius;
            _startAngle = startAngle;
            _sweepAngle = sweepAngle;
            _chordAngle = chordAngle;
        }

        public override InstructionType Type { get { return InstructionType.EW; } }

        public double Radius { get { return _radius; } }
        private double _radius;

        public double StartAngle { get { return _startAngle; } }
        private double _startAngle;

        public double SweepAngle { get { return _sweepAngle; } }
        private double _sweepAngle;

        public double ChordAngle { get { return _chordAngle; } }
        private double _chordAngle;

        public override void Execute(States states)
        {
        }
    }
}