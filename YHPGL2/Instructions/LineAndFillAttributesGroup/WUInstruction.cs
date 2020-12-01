using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public enum LineWidthType
    {
        Metric,
        Relative
    }

    public class WUInstruction : Instruction
    {
        public WUInstruction()
        {
            _lineWidthType = LineWidthType.Metric;
        }

        public WUInstruction(LineWidthType lineWidthType)
        {
            _lineWidthType = lineWidthType;
        }

        public override InstructionType Type { get { return InstructionType.WU; } }

        public override bool AllowInPolygonMode { get { return false; } }

        public LineWidthType LineWidthType { get { return _lineWidthType; } }
        private LineWidthType _lineWidthType;

        public override void Execute(States states)
        {
            states.WU(_lineWidthType);
        }
    }
}