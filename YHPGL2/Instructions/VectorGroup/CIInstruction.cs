using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class CIInstruction : Instruction
    {
        public CIInstruction(double radius, double chordAngle = 5)
        {
            _radius = radius;
            _chordAngle = chordAngle;
        }

        public override InstructionType Type { get { return InstructionType.CI; } }

        public double Radius { get { return _radius; } }
        private double _radius;

        public double ChordAngle { get { return _chordAngle; } }
        private double _chordAngle;

        public override void Execute(States states)
        {

        }
    }
}