using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class PMInstruction : Instruction
    {
        public PMInstruction() { }

        public PMInstruction(int funcCode)
        {
            _funcCode = funcCode;
        }

        public override InstructionType Type { get { return InstructionType.PM; } }

        public override bool AllowInPolygonMode { get { return true; } }

        public int FuncCode { get { return _funcCode; } }
        private int _funcCode;

        public override void Execute(States states)
        {
            states.PM(_funcCode);
        }
    }
}