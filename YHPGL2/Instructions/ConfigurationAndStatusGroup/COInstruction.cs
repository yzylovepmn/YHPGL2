using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class COInstruction : Instruction
    {
        public COInstruction(string comment)
        {
            _comment = comment;
        }

        public override InstructionType Type { get { return InstructionType.CO; } }

        public override bool AllowInPolygonMode { get { return true; } }

        public string Comment { get { return _comment; } }
        private string _comment;

        public override void Execute(States states)
        {
            // Do Noting
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", base.ToString(), _comment);
        }
    }
}