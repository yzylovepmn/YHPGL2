using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public class ACInstruction : Instruction
    {
        public ACInstruction(Point? point)
        {
            _point = point;
        }

        public override InstructionType Type { get { return InstructionType.AC; } }

        public Point? Point { get { return _point; } }
        private Point? _point;

        public override void Execute(States states)
        {
        }
    }
}