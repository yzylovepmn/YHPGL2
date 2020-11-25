using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public enum SCType
    {
        Unknown = -1,
        Anisotropic,
        Isotropic,
        PointFactor
    }

    public class SCInstruction : Instruction
    {
        public SCInstruction()
        {
            _scType = SCType.Unknown;
        }

        public SCInstruction(double xMin, double xFactor, double yMin, double yFactor)
        {
            _xMin = xMin;
            _xMax = xFactor;
            _yMin = yMin;
            _yMax = yFactor;
            _scType = SCType.PointFactor;

            if (_xMax == 0 || _yMax == 0)
                throw new ArgumentException();
        }

        public SCInstruction(double xMin, double xMax, double yMin, double yMax, SCType type = SCType.Anisotropic, double left = 50, double bottom = 50)
        {
            _xMin = xMin;
            _xMax = xMax;
            _yMin = yMin;
            _yMax = yMax;
            _scType = type;
            _left = left;
            _bottom = bottom;

            if (_xMin == _xMax || _yMin == _yMax)
                throw new ArgumentException();
        }

        public override InstructionType Type { get { return InstructionType.SC; } }

        public SCType SCType { get { return _scType; } }
        private SCType _scType;

        public double XMin { get { return _xMin; } }
        private double _xMin;

        public double YMin { get { return _yMin; } }
        private double _yMin;

        public double XMax { get { return _xMax; } }
        private double _xMax;

        public double YMax { get { return _yMax; } }
        private double _yMax;

        public double XFactor { get { return _xMax; } }

        public double YFactor { get { return _yMax; } }

        public double Left { get { return _left; } }
        private double _left;

        public double Bottom { get { return _bottom; } }
        private double _bottom;

        public override void Execute(States states)
        {
            if (_scType == SCType.Unknown)
                states.SC();
            else states.SC(_scType, _xMin, _xMax, _yMin, _yMax, _left, _bottom);
        }
    }
}