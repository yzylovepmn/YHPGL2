using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public abstract class Instruction
    {
        public abstract InstructionType Type { get; }

        public abstract void Execute(States states);

        public override string ToString()
        {
            return Type.ToString();
        }
    }
}