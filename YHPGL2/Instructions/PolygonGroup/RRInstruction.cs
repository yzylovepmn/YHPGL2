using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public class RRInstruction : Instruction
    {
        public RRInstruction(Vector end)
        {
            _end = end;
        }

        public override InstructionType Type { get { return InstructionType.RR; } }

        public override bool AllowInPolygonMode { get { return false; } }

        public Vector End { get { return _end; } }
        private Vector _end;

        public override void Execute(States states)
        {
        }
    }
}