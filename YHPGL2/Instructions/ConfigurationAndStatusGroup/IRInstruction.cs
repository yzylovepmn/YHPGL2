using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public class IRInstruction : Instruction
    {
        public IRInstruction()
        {
            _p1 = null;
            _p2 = null;
        }

        public IRInstruction(Point p1)
        {
            _p1 = p1;
            _p2 = null;
        }

        public IRInstruction(Point p1, Point p2)
        {
            _p1 = p1;
            _p2 = p2;
        }

        public override InstructionType Type { get { return InstructionType.IR; } }

        public override bool AllowInPolygonMode { get { return false; } }

        public Point? P1 { get { return _p1; } }
        private Point? _p1;

        public Point? P2 { get { return _p2; } }
        private Point? _p2;

        public override void Execute(States states)
        {
            if (_p1 == null)
                states.IR();
            else if (_p2 == null)
                states.IR(_p1.Value);
            else states.IR(_p1.Value, _p2.Value);
        }
    }
}