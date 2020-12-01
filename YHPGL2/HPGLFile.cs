using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    public sealed class HPGLFile
    {
        internal static double DefaultWidth = 800;  // mm
        internal static double DefaultHeight = 1000;// mm

        public HPGLFile()
        {
            _states = new States();
            _instructions = new List<Instruction>();
        }

        public HPGLFile(IEnumerable<Instruction> instructions)
        {
            _states = new States();
            _instructions = instructions.ToList();
        }

        public States States { get { return _states; } }
        private States _states;

        public IEnumerable<Instruction> Instructions { get { return _instructions; } }
        private List<Instruction> _instructions;

        public IEnumerable<IShape> Execute(Size area)
        {
            return _states.Execute(area, _instructions);
        }

        public void AddInstruction(Instruction instruction)
        {
            _instructions.Add(instruction);
        }

        public void AddInstructions(IEnumerable<Instruction> instructions)
        {
            _instructions.AddRange(instructions);
        }

        public void InsertInstruction(int index, Instruction instruction)
        {
            _instructions.Insert(index, instruction);
        }

        public void InsertInstructions(int index, IEnumerable<Instruction> instructions)
        {
            _instructions.InsertRange(index, instructions);
        }

        public void RemoveInstruction(Instruction instruction)
        {
            _instructions.Remove(instruction);
        }

        public void RemoveInstruction(int index)
        {
            _instructions.RemoveAt(index);
        }

        public void Clear()
        {
            _instructions.Clear();
        }

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