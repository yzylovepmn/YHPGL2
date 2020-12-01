using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public class LTInstruction : Instruction
    {
        public LTInstruction()
        {
            _lineType = null;
            _patternLength = 4;
            _isRelative = true;
        }

        public LTInstruction(int lineType)
        {
            _lineType = lineType;
            _patternLength = 4;
            _isRelative = true;
        }

        public LTInstruction(int lineType, double patternLength)
        {
            _lineType = lineType;
            _patternLength = patternLength;
            _isRelative = true;
        }

        public LTInstruction(int lineType, double patternLength, bool isRelative)
        {
            _lineType = lineType;
            _patternLength = patternLength;
            _isRelative = isRelative;
        }

        public override InstructionType Type { get { return InstructionType.LT; } }

        public override bool AllowInPolygonMode { get { return false; } }

        public int? LineType { get { return _lineType; } }
        private int? _lineType;

        public double PatternLength { get { return _patternLength; } }
        private double _patternLength;

        public bool IsRelative { get { return _isRelative; } }
        private bool _isRelative;

        public override void Execute(States states)
        {
            if (_lineType == null)
                states.LT();
            else states.LT(_lineType.Value, _patternLength, _isRelative);
        }
    }
}