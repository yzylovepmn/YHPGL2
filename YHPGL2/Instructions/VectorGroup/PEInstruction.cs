using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class PEInstruction : Instruction
    {
        public override InstructionType Type { get { return InstructionType.PE; } }

        public override void Execute(States states)
        {
        }
    }
}