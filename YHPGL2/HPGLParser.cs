using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YHPGL2
{
    internal class HPGLParser
    {
        internal const int MinInteger = -8388608;
        internal const int MaxInteger = 8388607;
        internal const int MinClampedInteger = short.MinValue;
        internal const int MaxClampedInteger = short.MaxValue;
        internal const double MinReal = -8388608.0000;
        internal const double MaxReal = 8388607.9999;
        internal const double MinClampedReal = -32768.0000;
        internal const double MaxClampedReal = 32767.9999;
        internal const char Separator = ',';
        internal const char Terminate = ';';

        private static int _index;
        private static string _code;

        private static void _BeforeParse(string code)
        {
            _code = code;
        }

        private static void _AfterParse()
        {
            _index = 0;
            _code = null;
        }

        internal static IEnumerable<Instruction> Parse(string code)
        {
            try
            {
                _BeforeParse(code);
                var insts = new List<Instruction>();
                while (!_IsEoF())
                {
                    var inst = _NextInstruction();
                    if (inst != null)
                        insts.Add(inst);
                }
                return insts;
            }
            catch (Exception e)
            {
            }
            finally
            {
                _AfterParse();
            }
            return null;
        }

        private static bool _IsEoF()
        {
            return _index < 0 || _index >= _code.Length;
        }

        private static Instruction _NextInstruction()
        {
            var type = InstructionType.Unknow;
            if (_TryGetInstructionType(out type))
            {
                var inst = default(Instruction);
                if (_TryIncreaseIndex())
                {
                    switch (type)
                    {
                        case InstructionType.CO:
                            inst = new COInstruction(_NextString());
                            break;
                        case InstructionType.DF:
                            inst = new DFInstruction();
                            break;
                        case InstructionType.IN:
                            inst = new INInstruction((int?)_NextNumber());
                            break;
                        case InstructionType.IP:
                        case InstructionType.IR:
                            {
                                var p1 = (Point?)_NextVector();
                                if (!p1.HasValue)
                                    inst = type == InstructionType.IP ? new IPInstruction() : (Instruction)new IRInstruction();
                                else
                                {
                                    var p2 = default(Point?);
                                    if (_SkipSeparator())
                                        p2 = (Point?)_NextVector();
                                    if (!p2.HasValue)
                                        inst = type == InstructionType.IP ? new IPInstruction(p1.Value) : (Instruction)new IRInstruction(p1.Value);
                                    else inst = type == InstructionType.IP ? new IPInstruction(p1.Value, p2.Value) : (Instruction)new IRInstruction(p1.Value, p2.Value);
                                }
                            }
                            break;
                        case InstructionType.IW:
                            {
                                var p1 = (Point?)_NextVector();
                                if (!p1.HasValue)
                                    inst = new IWInstruction();
                                else
                                {
                                    var p2 = default(Point?);
                                    if (_SkipSeparator())
                                    {
                                        p2 = (Point?)_NextVector();
                                        if (p2.HasValue)
                                            inst = new IWInstruction(p1.Value, p2.Value);
                                        else throw new ArgumentException("lost p2");
                                    }
                                    else throw new ArgumentException("lost p2");
                                }
                            }
                            break;
                        case InstructionType.PG:
                            inst = new PGInstruction((int?)_NextNumber());
                            break;
                        case InstructionType.RO:
                            {
                                var number = (int?)_NextNumber();
                                if (number.HasValue)
                                {
                                    var n = number.Value;
                                    if (n == 0 || n == 90 || n == 180 || n == 270)
                                        inst = new ROInstruction((RODirection)n);
                                    else throw new ArgumentException("angle is illegal");
                                }
                                else inst = new ROInstruction();
                            }
                            break;
                        case InstructionType.RP:
                            {
                                inst = new RPInstruction((int?)_NextNumber());
                                _SkipWhiteSpace();
                                if (_code[_index] == Terminate)
                                    _TryIncreaseIndex();
                                else throw new ArgumentException($"need Terminate {Terminate}");
                            }
                            break;
                        case InstructionType.SC:
                            {
                                var parameters = _NextParameters();
                                if (parameters.Count == 0)
                                    inst = new SCInstruction();
                                else
                                {
                                    if (parameters.Count == 4)
                                        inst = new SCInstruction(parameters[0], parameters[1], parameters[2], parameters[3], SCType.Anisotropic);
                                    else if (parameters.Count == 5)
                                    {
                                        var scType = (SCType)parameters[4];
                                        switch (scType)
                                        {
                                            case SCType.Anisotropic:
                                            case SCType.Isotropic:
                                                inst = new SCInstruction(parameters[0], parameters[1], parameters[2], parameters[3], scType);
                                                break;
                                            case SCType.PointFactor:
                                                inst = new SCInstruction(parameters[0], parameters[1], parameters[2], parameters[3]);
                                                break;
                                        }
                                    }
                                    else if (parameters.Count == 7)
                                    {
                                        var scType = (SCType)parameters[4];
                                        if (scType == SCType.Isotropic)
                                            inst = new SCInstruction(parameters[0], parameters[1], parameters[2], parameters[3], scType, parameters[5], parameters[6]);
                                        else throw new ArgumentException("Only Isotropic type support left and bottom parameter!");
                                    }

                                    if (inst != null)
                                    {
                                        var scInst = inst as SCInstruction;
                                        switch (scInst.SCType)
                                        {
                                            case SCType.Anisotropic:
                                            case SCType.Isotropic:
                                                if (scInst.XMin == scInst.XMax || scInst.YMin == scInst.YMax)
                                                    inst = null;
                                                break;
                                            case SCType.PointFactor:
                                                if (scInst.XFactor == 0 || scInst.YFactor == 0)
                                                    inst = null;
                                                break;
                                        }
                                    }
                                }
                            }
                            break;

                        case InstructionType.AA:
                            {
                                var parameters = _NextParameters();
                                if (parameters.Count == 3)
                                    inst = new AAInstruction(new Point(parameters[0], parameters[1]), Generator.FormatAngle(parameters[2]));
                                else if (parameters.Count == 4)
                                    inst = new AAInstruction(new Point(parameters[0], parameters[1]), Generator.FormatAngle(parameters[2]), Generator.FormatChordAngle(parameters[3]));
                            }
                            break;
                        case InstructionType.AR:
                            {
                                var parameters = _NextParameters();
                                if (parameters.Count == 3)
                                    inst = new ARInstruction(new Vector(parameters[0], parameters[1]), Generator.FormatAngle(parameters[2]));
                                else if (parameters.Count == 4)
                                    inst = new ARInstruction(new Vector(parameters[0], parameters[1]), Generator.FormatAngle(parameters[2]), Generator.FormatChordAngle(parameters[3]));
                            }
                            break;
                        case InstructionType.AT:
                            {
                                var parameters = _NextParameters();
                                if (parameters.Count == 4)
                                    inst = new ATInstruction(new Point(parameters[0], parameters[1]), new Point(parameters[2], parameters[3]));
                                else if (parameters.Count == 5)
                                    inst = new ATInstruction(new Point(parameters[0], parameters[1]), new Point(parameters[2], parameters[3]), Generator.FormatChordAngle(parameters[4]));
                            }
                            break;
                        case InstructionType.CI:
                            {
                                var parameters = _NextParameters();
                                if (parameters.Count == 1)
                                    inst = new CIInstruction(parameters[0]);
                                else if (parameters.Count == 2)
                                    inst = new CIInstruction(parameters[0], Generator.FormatChordAngle(parameters[1]));
                            }
                            break;
                        case InstructionType.PA:
                            inst = new PAInstruction(_NextVectors().Select(vec => (Point)vec));
                            break;
                        case InstructionType.PD:
                            inst = new PDInstruction(_NextVectors().Select(vec => (Point)vec));
                            break;
                        case InstructionType.PE:
                            break;
                        case InstructionType.PR:
                            inst = new PRInstruction(_NextVectors().Select(vec => (Point)vec));
                            break;
                        case InstructionType.PU:
                            inst = new PUInstruction(_NextVectors().Select(vec => (Point)vec));
                            break;
                        case InstructionType.RT:
                            {
                                var parameters = _NextParameters();
                                if (parameters.Count == 4)
                                    inst = new RTInstruction(new Vector(parameters[0], parameters[1]), new Vector(parameters[2], parameters[3]));
                                else if (parameters.Count == 5)
                                    inst = new RTInstruction(new Vector(parameters[0], parameters[1]), new Vector(parameters[2], parameters[3]), Generator.FormatChordAngle(parameters[4]));
                            }
                            break;

                        case InstructionType.EA:
                            {
                                var end = (Point?)_NextVector();
                                if (end.HasValue)
                                    inst = new EAInstruction(end.Value);
                                else throw new ArgumentException("Lost end point");
                            }
                            break;
                        case InstructionType.EP:
                            inst = new EPInstruction();
                            break;
                        case InstructionType.ER:
                            {
                                var end = _NextVector();
                                if (end.HasValue)
                                    inst = new ERInstruction(end.Value);
                                else throw new ArgumentException("Lost end point");
                            }
                            break;
                        case InstructionType.EW:
                            {
                                var parameters = _NextParameters();
                                if (parameters.Count == 3)
                                    inst = new EWInstruction(parameters[0], Math.IEEERemainder(parameters[1], 360), Generator.FormatAngle(parameters[2]));
                                else if (parameters.Count == 4)
                                    inst = new EWInstruction(parameters[0], Math.IEEERemainder(parameters[1], 360), Generator.FormatAngle(parameters[2]), Generator.FormatChordAngle(parameters[3]));
                            }
                            break;
                        case InstructionType.FP:
                            {
                                var mode = (int?)_NextNumber();
                                if (mode.HasValue && (mode.Value != 0 || mode.Value != 1))
                                    throw new ArgumentException("mode is illegal!");
                                inst = new FPInstruction(mode.HasValue ? (FillMode)mode.Value : FillMode.Even_Odd);
                            }
                            break;
                        case InstructionType.PM:
                            {
                                var code = (int?)_NextNumber();
                                if (code == null)
                                    inst = new PMInstruction();
                                else
                                {
                                    if (code < 0 || code > 2)
                                        throw new ArgumentOutOfRangeException();
                                    else inst = new PMInstruction(code.Value);
                                }
                            }
                            break;
                        case InstructionType.RA:
                            {
                                var end = (Point?)_NextVector();
                                if (end.HasValue)
                                    inst = new RAInstruction(end.Value);
                                else throw new ArgumentException("Lost end point");
                            }
                            break;
                        case InstructionType.RR:
                            {
                                var end = _NextVector();
                                if (end.HasValue)
                                    inst = new RRInstruction(end.Value);
                                else throw new ArgumentException("Lost end point");
                            }
                            break;
                        case InstructionType.WG:
                            {
                                var parameters = _NextParameters();
                                if (parameters.Count == 3)
                                    inst = new WGInstruction(parameters[0], Math.IEEERemainder(parameters[1], 360), Generator.FormatAngle(parameters[2]));
                                else if (parameters.Count == 4)
                                    inst = new WGInstruction(parameters[0], Math.IEEERemainder(parameters[1], 360), Generator.FormatAngle(parameters[2]), Generator.FormatChordAngle(parameters[3]));
                            }
                            break;

                        case InstructionType.AC:
                            inst = new ACInstruction((Point?)_NextVector());
                            break;
                        case InstructionType.FT:
                            {
                                var parameters = _NextParameters();
                                if (parameters.Count == 0)
                                    inst = new FTInstruction();
                                else
                                {
                                    var fillType = (int)parameters[0];
                                    if (fillType == 1 || fillType == 2 || fillType == 3 || fillType == 4 || fillType == 10 || fillType == 11)
                                    {
                                        if (parameters.Count == 1)
                                            inst = new FTInstruction(fillType);
                                        else if (parameters.Count == 2)
                                            inst = new FTInstruction(fillType, parameters[1]);
                                        else if (parameters.Count == 3)
                                            inst = new FTInstruction(fillType, parameters[1], parameters[2]);
                                    }
                                    else throw new ArgumentException("fillType not support");
                                }
                            }
                            break;
                        case InstructionType.LA:
                            {
                                var parameters = _NextParameters();
                                if (parameters.Count == 0)
                                    inst = new LAInstruction();
                                else
                                {
                                    var kind1 = default(int?);
                                    var value1 = default(int?);
                                    var kind2 = default(int?);
                                    var value2 = default(int?);
                                    var kind3 = default(int?);
                                    var value3 = default(double?);
                                    if (parameters.Count >= 2)
                                    {
                                        kind1 = (int)parameters[0];
                                        value1 = (int)parameters[1];
                                        if (kind1 != 1 || value1 < 1 || value1 > 4)
                                            throw new ArgumentOutOfRangeException();
                                    }
                                    if (parameters.Count >= 4)
                                    {
                                        kind2 = (int)parameters[2];
                                        value2 = (int)parameters[3];
                                        if (kind2 != 2 || value2 < 1 || value2 > 6)
                                            throw new ArgumentOutOfRangeException();
                                    }
                                    if (parameters.Count >= 6)
                                    {
                                        kind3 = (int)parameters[4];
                                        value3 = parameters[5];
                                        if (kind3 != 3 || value3 < 1 || value3 > 32767)
                                            throw new ArgumentOutOfRangeException();
                                    }
                                    if (parameters.Count == 2)
                                        inst = new LAInstruction((LineEnd)value1.Value);
                                    if (parameters.Count == 4)
                                        inst = new LAInstruction((LineEnd)value1.Value, (LineJoin)value2.Value);
                                    if (parameters.Count == 6)
                                        inst = new LAInstruction((LineEnd)value1.Value, (LineJoin)value2.Value, value3.Value);
                                }
                            }
                            break;
                        case InstructionType.LT:
                            {
                                var parameters = _NextParameters();
                                if (parameters.Count == 0)
                                    inst = new LTInstruction();
                                else
                                {
                                    var lineType = (int)parameters[0];
                                    var patternLength = parameters.Count > 1 ? parameters[1] : 4;
                                    var mode = parameters.Count > 2 ? (int)parameters[2] : 0;
                                    if (patternLength <= 0 || patternLength > 32767 || mode < 0 || mode > 1)
                                        throw new ArgumentOutOfRangeException();
                                    inst = new LTInstruction(lineType, patternLength, mode == 0);
                                }
                            }
                            break;
                        case InstructionType.PW:
                            {
                                var parameters = _NextParameters();
                                if (parameters.Count == 0)
                                    inst = new PWInstruction();
                                else
                                {
                                    var width = parameters[0];
                                    if (width <= 0)
                                        throw new ArgumentOutOfRangeException("width must larger than zero");
                                    if (parameters.Count == 1)
                                        inst = new PWInstruction(width);
                                    else if (parameters.Count == 2)
                                        inst = new PWInstruction(width, (int)parameters[1]);
                                }
                            }
                            break;
                        case InstructionType.RF:
                            break;
                        case InstructionType.SM:
                            break;
                        case InstructionType.SP:
                            {
                                var pen = (int?)_NextNumber();
                                if (pen.HasValue)
                                    inst = new SPInstruction(pen.Value);
                                else inst = new SPInstruction();
                            }
                            break;
                        case InstructionType.UL:
                            break;
                        case InstructionType.WU:
                            {
                                var lineWidthType = (int?)_NextNumber();
                                if (lineWidthType.HasValue)
                                {
                                    if (lineWidthType.Value > 1 || lineWidthType.Value < 0)
                                        throw new ArgumentOutOfRangeException();
                                    inst = new WUInstruction((LineWidthType)lineWidthType.Value);
                                }
                                else inst = new WUInstruction();
                            }
                            break;

                        case InstructionType.AD:
                            break;
                        case InstructionType.CF:
                            break;
                        case InstructionType.CP:
                            break;
                        case InstructionType.DI:
                            break;
                        case InstructionType.DR:
                            break;
                        case InstructionType.DT:
                            break;
                        case InstructionType.DV:
                            break;
                        case InstructionType.ES:
                            break;
                        case InstructionType.LB:
                            break;
                        case InstructionType.LO:
                            break;
                        case InstructionType.SA:
                            break;
                        case InstructionType.SD:
                            break;
                        case InstructionType.SI:
                            break;
                        case InstructionType.SL:
                            break;
                        case InstructionType.SR:
                            break;
                        case InstructionType.SS:
                            break;
                        case InstructionType.TD:
                            break;
                    }
                }
                return inst;
            }
            else return null;
        }

        private static bool _TryGetInstructionType(out InstructionType type)
        {
            type = InstructionType.Unknow;
            if (_Expect(char.IsLetter))
            {
                var c1 = _code[_index];
                if (_TryIncreaseIndex())
                {
                    var c2 = _code[_index];
                    return Enum.TryParse(string.Format("{0}{1}", c1, c2), out type);
                }
                else return false;
            }
            else return false;
        }

        private static string _NextString()
        {
            if (_SkipWhiteSpace())
            {
                if (_code[_index] == '"' && _TryIncreaseIndex())
                {
                    var chars = new List<char>();
                    while (true)
                    {
                        var c = _code[_index];
                        if (c != '"')
                            chars.Add(c);
                        else
                        {
                            _TryIncreaseIndex();
                            break;
                        }
                        if (!_TryIncreaseIndex())
                            throw new ArgumentException("Need a closing double quote");
                    }
                    return new string(chars.ToArray()).Replace("\"\"", "\"");
                }
            }
            return null;
        }

        private static IEnumerable<Vector> _NextVectors()
        {
            while (true)
            {
                var point = _NextVector();
                if (point.HasValue)
                    yield return point.Value;
                else break;
                if (!_SkipSeparator())
                    break;
            }
        }

        private static List<double> _NextParameters()
        {
            var parameters = new List<double>();
            while (true)
            {
                var para = _NextNumber();
                if (para.HasValue)
                {
                    parameters.Add(para.Value);
                    if (!_SkipSeparator()) break;
                }
                else break;
            }
            return parameters;
        }

        private static Vector? _NextVector()
        {
            var point = new Vector();
            var x = _NextNumber();
            if (x.HasValue)
            {
                point.X = x.Value;
                if (_SkipSeparator())
                {
                    var y = _NextNumber();
                    if (y.HasValue)
                    {
                        point.Y = y.Value;
                        return point;
                    }
                    throw new ArgumentException("Expect y coordnate");
                }
            }
            return null;
        }

        private static double? _NextNumber()
        {
            if (_SkipWhiteSpace())
            {
                var numbers = new List<char>();
                var flag1 = false;
                var flag2 = false;
                while (true)
                {
                    var c = _code[_index];
                    if (_IsReal(c))
                    {
                        if (_IsPlusOrSubtract(c))
                        {
                            if (flag1) break;
                            if (numbers.Count == 0)
                                flag1 = true;
                            else break;
                        }
                        if (c == '.')
                        {
                            if (flag2) throw new ArgumentException("Decimal point '.' can not be repeated!");
                            flag2 = true;
                        }
                        numbers.Add(c);
                    }
                    else break;
                    if (!_TryIncreaseIndex())
                        break;
                }
                if (numbers.Count > 0)
                {
                    var value = 0.0;
                    if (double.TryParse(new string(numbers.ToArray()), out value))
                    {
                        if (_IsValid(value))
                            return value;
                        else throw new ArgumentOutOfRangeException($"value {value} must be in [{MinReal}, {MaxReal}]!");
                    }
                    else return null;
                }
                else return null;
            }
            else return null;
        }

        private static bool _SkipSeparator()
        {
            return _Expect(c => !_IsSeparatorOrWhiteSpace(c));
        }

        private static bool _SkipWhiteSpace()
        {
            return _Expect(c => !char.IsWhiteSpace(c));
        }

        private static bool _Expect(Func<char, bool> func)
        {
            while (!func(_code[_index]))
            {
                if (!_TryIncreaseIndex())
                    break;
            }
            return !_IsEoF() && func(_code[_index]);
        }

        private static bool _TryIncreaseIndex()
        {
            if (_IsEoF()) return false;
            _index++;
            return !_IsEoF();
        }

        private static bool _IsSeparatorOrWhiteSpace(char c)
        {
            return c == Separator || char.IsWhiteSpace(c);
        }

        private static bool _IsInteger(char c)
        {
            return char.IsDigit(c) || _IsPlusOrSubtract(c);
        }

        private static bool _IsReal(char c)
        {
            return _IsInteger(c) || c == '.';
        }

        private static bool _IsPlusOrSubtract(char c)
        {
            return c == '-' || c == '+';
        }

        private static bool _IsValid(double value)
        {
            return value >= MinReal && value <= MaxReal;
        }

        private static bool _IsValid(int value)
        {
            return value >= MinInteger && value <= MaxInteger;
        }

        private static void _ClampInteger(ref int value)
        {
            value = Math.Max(MinClampedInteger, Math.Min(MaxClampedInteger, value));
        }

        private static void _ClampReal(ref double value)
        {
            value = Math.Max(MinClampedReal, Math.Min(MaxClampedReal, value));
        }
    }
}