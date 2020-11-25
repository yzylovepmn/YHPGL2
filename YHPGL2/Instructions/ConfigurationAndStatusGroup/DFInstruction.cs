using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class DFInstruction : Instruction
    {
        public override InstructionType Type { get { return InstructionType.DF; } }

        public override void Execute(States states)
        {
            states.DF();
        }
    }
}