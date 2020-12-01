using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public enum RODirection
    {
        _0 = 0,
        _90 = 90,
        _180 = 180,
        _270 = 270
    }

    public class ROInstruction : Instruction
    {
        public ROInstruction()
        {
            _direction = RODirection._0;
        }

        public ROInstruction(RODirection direction)
        {
            _direction = direction;
        }

        public override InstructionType Type { get { return InstructionType.RO; } }

        public override bool AllowInPolygonMode { get { return false; } }

        public RODirection Direction { get { return _direction; } }
        private RODirection _direction;

        public override void Execute(States states)
        {
            states.RO(_direction);
        }
    }
}