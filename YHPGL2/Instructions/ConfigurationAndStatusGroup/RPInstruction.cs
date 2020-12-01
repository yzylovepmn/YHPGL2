using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class RPInstruction : Instruction
    {
        public RPInstruction(int? n = null)
        {
            _n = n;
        }

        public override InstructionType Type { get { return InstructionType.RP; } }

        public override bool AllowInPolygonMode { get { return false; } }

        public int? N { get { return _n; } }
        private int? _n;

        public override void Execute(States states)
        {

        }
    }
}