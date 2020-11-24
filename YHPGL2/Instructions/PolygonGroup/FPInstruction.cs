using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public enum FillMode
    {
        Even_Odd,
        Non_Zero
    }

    public class FPInstruction : Instruction
    {
        public FPInstruction()
        {
            _fillMode = FillMode.Even_Odd;
        }

        public FPInstruction(FillMode fillMode)
        {
            _fillMode = fillMode;
        }

        public override InstructionType Type { get { return InstructionType.FP; } }

        public FillMode FillMode { get { return _fillMode; } }
        private FillMode _fillMode;

        public override void Execute(States states)
        {
        }
    }
}