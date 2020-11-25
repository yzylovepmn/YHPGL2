using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class INInstruction : Instruction
    {
        public INInstruction(int? n)
        {
            _n = n;
        }

        public override InstructionType Type { get { return InstructionType.IN; } }

        public int? N { get { return _n; } }
        private int? _n;

        public override void Execute(States states)
        {
            states.IN();
        }
    }
}