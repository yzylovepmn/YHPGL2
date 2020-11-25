using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public enum LineEnd
    {
        Butt = 1,
        Square,
        Triangular,
        Round
    }

    public enum LineJoin
    {
        Mitered = 1,
        Mitered_Beveled,
        Triangular,
        Round,
        Beveled,
        NoApplied
    }

    public class LAInstruction : Instruction
    {
        public LAInstruction()
        {
            _lineEnds = LineEnd.Butt;
            _lineJoins = LineJoin.Mitered;
            _miterLimit = 5;
        }

        public LAInstruction(LineEnd lineEnds)
        {
            _lineEnds = lineEnds;
            _lineJoins = LineJoin.Mitered;
            _miterLimit = 5;
        }

        public LAInstruction(LineEnd lineEnds, LineJoin lineJoins)
        {
            _lineEnds = lineEnds;
            _lineJoins = lineJoins;
            _miterLimit = 5;
        }

        public LAInstruction(LineEnd lineEnds, LineJoin lineJoins, double miterLimit)
        {
            _lineEnds = lineEnds;
            _lineJoins = lineJoins;
            _miterLimit = miterLimit;
        }

        public override InstructionType Type { get { return InstructionType.LA; } }

        public LineEnd LineEnds { get { return _lineEnds; } }
        private LineEnd _lineEnds;

        public LineJoin LineJoins { get { return _lineJoins; } }
        private LineJoin _lineJoins;

        public double MiterLimit { get { return _miterLimit; } }
        private double _miterLimit;

        public override void Execute(States states)
        {
            states.LA(_lineEnds, _lineJoins, _miterLimit);
        }
    }
}