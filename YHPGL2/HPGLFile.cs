using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHPGL2
{
    public sealed class HPGLFile
    {
        internal static double DefaultWidth = 800;  // mm
        internal static double DefaultHeight = 1000;// mm

        private HPGLFile(IEnumerable<Instruction> instructions)
        {
            _states = new States();
            _instructions = instructions.ToList();
        }

        public States States { get { return _states; } }
        private States _states;

        public IEnumerable<Instruction> Instructions { get { return _instructions; } }
        private List<Instruction> _instructions;

        public static HPGLFile Load(string fileFullName)
        {
            using (var stream = File.OpenRead(fileFullName))
                return Load(stream);
        }

        public static HPGLFile Load(Stream stream)
        {
            var curP = stream.Position;
            stream.Position = 0;
            var reader = new StreamReader(stream);
            var file = new HPGLFile(HPGLParser.Parse(reader.ReadToEnd().ToUpper()));
            stream.Position = curP;
            return file;
        }
    }
}