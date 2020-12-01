using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public class RAInstruction : Instruction
    {
        public RAInstruction(Point end)
        {
            _end = end;
        }

        public override InstructionType Type { get { return InstructionType.EA; } }

        public override bool AllowInPolygonMode { get { return false; } }

        public Point End { get { return _end; } }
        private Point _end;

        public override void Execute(States states)
        {
        }
    }
}