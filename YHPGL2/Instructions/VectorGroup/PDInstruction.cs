using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public class PDInstruction : Instruction
    {
        public PDInstruction(IEnumerable<Vector> vecs)
        {
            _vecs = vecs.ToList();
        }

        public override InstructionType Type { get { return InstructionType.PD; } }

        public IEnumerable<Vector> Vecs { get { return _vecs; } }
        private List<Vector> _vecs;

        public override void Execute(States states)
        {
        }
    }
}