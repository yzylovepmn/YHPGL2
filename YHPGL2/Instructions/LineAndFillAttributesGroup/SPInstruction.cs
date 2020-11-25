using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class SPInstruction : Instruction
    {
        public SPInstruction() { }

        public SPInstruction(int pen) 
        {
            _pen = pen;
        }

        public override InstructionType Type { get { return InstructionType.SP; } }

        public int Pen { get { return _pen; } }
        private int _pen;

        public override void Execute(States states)
        {
            states.SP(_pen);
        }
    }
}