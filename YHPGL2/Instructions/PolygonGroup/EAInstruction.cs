using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public class EAInstruction : Instruction
    {
        public EAInstruction(Point end)
        {
            _end = end;
        }

        public override InstructionType Type { get { return InstructionType.EA; } }

        public Point End { get { return _end; } }
        private Point _end;

        public override void Execute(States states)
        {
        }
    }
}