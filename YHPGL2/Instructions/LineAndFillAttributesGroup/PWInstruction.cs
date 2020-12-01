using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class PWInstruction : Instruction
    {
        public PWInstruction()
        {
        }

        public PWInstruction(double lineWidth)
        {
            _lineWidth = lineWidth;
        }

        public PWInstruction(double lineWidth, int pen)
        {
            _lineWidth = lineWidth;
            _pen = pen;
        }

        public override InstructionType Type { get { return InstructionType.PW; } }

        public override bool AllowInPolygonMode { get { return false; } }

        public double? LineWidth { get { return _lineWidth; } }
        private double? _lineWidth;

        public int? Pen { get { return _pen; } }
        private int? _pen;

        public override void Execute(States states)
        {
            if (_lineWidth == null)
                states.PW();
            else if (_pen == null)
                states.PW(_lineWidth.Value);
            else states.PW(_pen.Value, _lineWidth.Value);
        }
    }
}