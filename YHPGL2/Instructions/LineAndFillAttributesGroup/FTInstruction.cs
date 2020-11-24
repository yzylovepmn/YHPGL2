using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class FTInstruction : Instruction
    {
        public FTInstruction()
        {
            _fillType = 1;
        }

        public FTInstruction(int fillType)
        {
            _fillType = fillType;
        }

        public FTInstruction(int fillType, double option1)
        {
            _fillType = fillType;
            _option1 = option1;
        }

        public FTInstruction(int fillType, double option1, double option2)
        {
            _fillType = fillType;
            _option1 = option1;
            _option2 = option2;
        }

        public override InstructionType Type { get { return InstructionType.FT; } }

        public int FillType { get { return _fillType; } }
        private int _fillType;

        public double? Option1 { get { return _option1; } }
        private double? _option1;

        public double? Option2 { get { return _option2; } }
        private double? _option2;

        public override void Execute(States states)
        {
        }
    }
}