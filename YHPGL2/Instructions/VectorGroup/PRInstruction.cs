﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public class PRInstruction : Instruction
    {
        public PRInstruction(IEnumerable<Point> points)
        {
            _points = points.ToList();
        }

        public override InstructionType Type { get { return InstructionType.PR; } }

        public override bool AllowInPolygonMode { get { return true; } }

        public IEnumerable<Point> Points { get { return _points; } }
        private List<Point> _points;

        public override void Execute(States states)
        {
            states.PR(_points);
        }
    }
}