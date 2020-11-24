using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public class IPInstruction : Instruction
    {
        public IPInstruction(Point? p1 = null, Point? p2 = null)
        {
            _p1 = p1;
            _p2 = p2;
        }

        public override InstructionType Type { get { return InstructionType.IP; } }

        public Point? P1 { get { return _p1; } }
        private Point? _p1;

        public Point? P2 { get { return _p2; } }
        private Point? _p2;

        public override void Execute(States states)
        {

        }
    }
}