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

        public double? LineWidth { get { return _lineWidth; } }
        private double? _lineWidth;

        public int? Pen { get { return _pen; } }
        private int? _pen;

        public override void Execute(States states)
        {
        }
    }
}