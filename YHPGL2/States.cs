using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace YHPGL2
{
    public class States
    {
        public States()
        {
            Reset();
        }

        public Size WorkArea { get { return _workArea; } }
        private Size _workArea;

        public Rect HardClip { get { return _hardClip; } }
        private Rect _hardClip;

        public Rect SoftClip { get { return _softClip; } }
        private Rect _softClip;

        private Dictionary<int, Pen> _pens;

        public Pen CurrentPen { get { return _currentPen; } }
        private Pen _currentPen;

        public Point CurrentPenLocation { get { return _currentPenLocation; } }
        private Point _currentPenLocation;

        private List<IShape> _cachedShapes;
        private List<Point> _cachedPolyLine;

        public Point P1 { get { return _p1; } }
        private Point _p1;

        public Point P2 { get { return _p2; } }
        private Point _p2;

        public Point FillOrigin { get { return _fillOrigin; } }
        private Point _fillOrigin;

        public bool IsPenDown { get { return _isPenDown; } }
        private bool _isPenDown;

        public bool IsRelative { get { return _isRelative; } }
        private bool _isRelative;

        private Matrix _totalTransform;
        private Matrix _totalReverseTransform;

        #region RO Inst
        public RODirection Direction { get { return _direction; } }
        private RODirection _direction;

        private Matrix _roTransform;
        #endregion

        #region Polygon
        public bool IsPolygonMode { get { return _isPolygonMode; } }
        private bool _isPolygonMode;

        private List<Vertex> _subPolygonCache;
        private List<List<Vertex>> _polygonCache;
        #endregion

        #region Scale
        public bool IsScaleMode { get { return _isScaleMode; } }
        private bool _isScaleMode;

        private SCType _scType;
        private Vector _scaleVec1;
        private Vector _scaleVec2;
        private Vector _scaleVec3;

        private Matrix _scaleTransform;
        #endregion

        #region Line Attributes
        public LineEnd LineEnds { get { return _lineEnds; } }
        private LineEnd _lineEnds;

        public LineJoin LineJoins { get { return _lineJoins; } }
        private LineJoin _lineJoins;

        public double MiterLimit { get { return _miterLimit; } }
        private double _miterLimit;
        #endregion

        #region Line Type
        public bool IsLineTypeDefault { get { return _isLineTypeDefault; } }
        private bool _isLineTypeDefault;

        public int LineType { get { return _lineType; } }
        private int _lineType;

        public double PatternLength { get { return _patternLength; } }
        private double _patternLength;

        public bool IsLineTypeRelative { get { return _isLineTypeRelative; } }
        private bool _isLineTypeRelative;
        #endregion

        public void Reset()
        {
            _cachedShapes = new List<IShape>();
            _cachedPolyLine = new List<Point>();
            _subPolygonCache = new List<Vertex>();
            _polygonCache = new List<List<Vertex>>();
            _pens = new Dictionary<int, Pen>();
            for (int i = 1; i <= 8; i++)
                _pens.Add(i, new Pen());
            _currentPen = null;
            _currentPenLocation = new Point();

            _hardClip.Size = _workArea;
            _softClip = _hardClip;
            _p1 = _hardClip.Location;
            _p2 = _p1 + new Vector(_hardClip.Width, _hardClip.Height);
            _fillOrigin = new Point();
            _isPenDown = false;
            _isRelative = false;
            _isPolygonMode = false;
            _isScaleMode = false;
            _direction = RODirection._0;
            _roTransform = Matrix.Identity;
            _totalTransform = Matrix.Identity;
            _totalReverseTransform = Matrix.Identity;
            _scaleTransform = Matrix.Identity;

            _scType = SCType.Unknown;
            _scaleVec1 = new Vector();
            _scaleVec2 = new Vector();
            _scaleVec3 = new Vector();

            LA();
            LT();
        }

        internal IEnumerable<IShape> Execute(Size area, IEnumerable<Instruction> insts)
        {
            _workArea = area;
            Reset();
            foreach (var inst in insts)
                inst.Execute(this);
            if (_cachedPolyLine.Count > 0)
                _cachedShapes.Add(new PolyLine(_cachedPolyLine));
            return _cachedShapes;
        }

        internal void IN()
        {
            PM(0);
            PM(2);
            RO();
            PU();
            IP();
            WU();
            PW();
            DF();
            PA(new List<Point>() { new Point() });
        }

        internal void DF()
        {
            PM(0);
            PM(2);
            AC();
            IW();
            LA();
            LT();
            PA();
            SC();
        }

        internal void PM()
        {
            PM(0);
        }

        internal void PM(int funcCode)
        {
            switch (funcCode)
            {
                case 0:
                    {
                        if (_isPolygonMode) return;
                        _isPolygonMode = true;
                        _ClearPolygon();
                        _subPolygonCache.Add(new Vertex(_currentPenLocation, _isPenDown));
                    }
                    break;
                case 1:
                case 2:
                    {
                        if (!_isPolygonMode) return;
                        _ClosePolygon();
                        if (funcCode == 2)
                            _isPolygonMode = false;
                    }
                    break;
            }
        }

        private void _ClosePolygon()
        {
            if (_subPolygonCache.Count < 3)
            {
                _subPolygonCache = new List<Vertex>();
                return;
            }
            _subPolygonCache.Add(new Vertex(_subPolygonCache[0].Point, _isPenDown));
            _polygonCache.Add(_subPolygonCache);
            _subPolygonCache = new List<Vertex>();
        }

        private void _ClearPolygon()
        {
            _polygonCache = new List<List<Vertex>>();
            _subPolygonCache = new List<Vertex>();
        }

        internal void PA(IEnumerable<Point> points = null)
        {
            _isRelative = false;
            _ToPoints(points);
        }

        internal void PR(IEnumerable<Point> points = null)
        {
            _isRelative = true;
            _ToPoints(points);
        }

        internal void PU(IEnumerable<Point> points = null)
        {
            _isPenDown = false;
            _ToPoints(points);
        }

        internal void PD(IEnumerable<Point> points = null)
        {
            if (!_isPenDown)
            {
                if (_isPolygonMode)
                {
                    if (_subPolygonCache.Count > 0)
                    {
                        var vertex = _subPolygonCache.Last();
                        vertex = new Vertex(vertex.Point, true);
                        _subPolygonCache[_subPolygonCache.Count - 1] = vertex;
                    }
                    //else _subPolygonCache.Add(new Vertex(_currentPenLocation, true));
                }
                else
                {
                    if (_cachedPolyLine.Count == 0)
                        _cachedPolyLine.Add(_currentPenLocation);
                    else
                    {
                        if (_cachedPolyLine.Last() != _currentPenLocation)
                        {
                            _cachedShapes.Add(new PolyLine(_cachedPolyLine));
                            _cachedPolyLine = new List<Point>();
                            _cachedPolyLine.Add(_currentPenLocation);
                        }
                    }
                }
            }
            _isPenDown = true;
            _ToPoints(points);
        }

        private void _ToPoints(IEnumerable<Point> points)
        {
            if (points == null || points.Count() == 0) return;
            
            if (_isRelative)
            {
                var vecs = points.Select(p => (Vector)p * _totalTransform);
                foreach (var vec in vecs)
                {
                    _currentPenLocation += vec;
                    if (_isPolygonMode)
                        _subPolygonCache.Add(new Vertex(_currentPenLocation, _isPenDown));
                    else if (_isPenDown)
                        _cachedPolyLine.Add(_currentPenLocation);
                }
            }
            else
            {
                points = points.Select(p => p * _totalTransform);
                if (_isPolygonMode)
                    _subPolygonCache.AddRange(points.Select(p => new Vertex(p, _isPenDown)));
                else if (_isPenDown)
                    _cachedPolyLine.AddRange(points);
                _currentPenLocation = points.Last();
            }
        }

        internal void EA(Point end)
        {
            if (_isPolygonMode) return;
            _ToRect(_currentPenLocation, end * _totalTransform);
        }

        internal void ER(Vector vec)
        {
            if (_isPolygonMode) return;
            _ToRect(_currentPenLocation, _currentPenLocation + vec * _totalTransform);
        }

        internal void AA(Point center, double sweepAngle, double chordAngle)
        {
            if (sweepAngle == 0) return;
            var sweepRadian = Generator.ToRadian(sweepAngle);
            var startPoint = _currentPenLocation * _totalReverseTransform;
            var vec = startPoint - center;
            var radius = vec.Length;
            var startRadian = Math.Atan2(vec.Y, vec.X);
            if (_isPolygonMode || _isPenDown)
            {
                var points = Generator.GenerateArcs(startRadian, sweepRadian, Generator.ToRadian(chordAngle)).Skip(1).Select(v => (center + radius * v) * _totalTransform);
                if (_isPolygonMode)
                    _subPolygonCache.AddRange(points.Select(p => new Vertex(p, _isPenDown)));
                else _cachedPolyLine.AddRange(points);
                _currentPenLocation = points.Last();
            }
            else
            {
                var endRadian = startRadian + sweepRadian;
                var end = center + radius * new Vector(Math.Cos(endRadian), Math.Sin(endRadian));
                _currentPenLocation = end * _totalTransform;
            }
        }

        internal void AR(Vector incr, double sweepAngle, double chordAngle)
        {
            if (sweepAngle == 0) return;
            var startPoint = _currentPenLocation * _totalReverseTransform;
            var center = startPoint + incr;
            AA(center, sweepAngle, chordAngle);
        }

        internal void AT(Point inter, Point end, double chordAngle)
        {
            if (_isPolygonMode || _isPenDown)
            {
                var start = _currentPenLocation * _totalReverseTransform;
                if (start == end && start == inter)
                    return;
                if (inter == start || inter == end)
                {
                    if (_isPolygonMode)
                        _subPolygonCache.Add(new Vertex(end * _totalTransform, _isPenDown));
                    else _cachedPolyLine.Add(end * _totalTransform);
                }
                else if (start == end)
                {
                    var vec = (start - inter) / 2;
                    var center = start - vec;
                    AA(center, 360, chordAngle);
                }
                else
                {
                    var center = Generator.CalcCenterByThreePoints(start, inter, end);
                    if (center.HasValue)
                    {
                        var vec1 = start - center.Value;
                        var vec2 = end - center.Value;
                        var sweepAngle = Vector.AngleBetween(vec1, vec2);
                        AA(center.Value, sweepAngle, chordAngle);
                    }
                }
            }
            _currentPenLocation = end * _totalTransform;
        }

        internal void RT(Vector inter, Vector end, double chordAngle)
        {
            var start = _currentPenLocation * _totalReverseTransform;
            AT(inter + start, end + start, chordAngle);
        }

        internal void CI(double radius, double chordAngle)
        {
            if (radius <= 0) return;
            var center = _currentPenLocation * _totalReverseTransform;
            var points = Generator.GenerateArcs(0, Math.PI * 2, Generator.ToRadian(chordAngle)).Select(v => (center + radius * v) * _totalTransform);
            if (_isPolygonMode)
            {
                _ClosePolygon();
                _polygonCache.Add(points.Select(p => new Vertex(p, true)).ToList());
            }
            else _cachedShapes.Add(new PolyLine(points));
        }

        internal void EW(double radius, double startAngle, double sweepAngle, double chordAngle)
        {
            if (_isPolygonMode) return;
            _ClearPolygon();

            var points = new List<Point>();
            if (radius == 0)
                points.Add(_currentPenLocation);
            else
            {
                var center = _currentPenLocation * _totalReverseTransform;
                if (radius < 0)
                {
                    radius = -radius;
                    startAngle += 180;
                }
                var startRadian = Generator.ToRadian(startAngle);

                if (sweepAngle == 0)
                {
                    var start = center + new Vector(Math.Cos(startRadian), Math.Sin(startRadian)) * radius;
                    points.Add(_currentPenLocation);
                    points.Add(start * _totalTransform);
                }
                else
                {
                    points.Add(_currentPenLocation);
                    points.AddRange(Generator.GenerateArcs(startRadian, Generator.ToRadian(sweepAngle), Generator.ToRadian(chordAngle)).Select(v => (center + v * radius) * _totalTransform));
                    points.Add(_currentPenLocation);
                }
            }

            _polygonCache.Add(points.Select(p => new Vertex(p, true)).ToList());
            _cachedShapes.Add(new PolyLine(points));
        }

        internal void EP()
        {
            if (_isPolygonMode) return;
            if (_polygonCache.Count == 0) return;
            foreach (var polygon in _polygonCache)
            {
                var points = new List<Point>();
                foreach (var vertex in polygon)
                {
                    if (vertex.IsStroked)
                        points.Add(vertex.Point);
                    else if (points.Count > 0)
                    {
                        _cachedShapes.Add(new PolyLine(points));
                        points = new List<Point>();
                    }
                }
            }
        }

        private void _ToRect(Point start, Point end)
        {
            _ClearPolygon();

            var points = new List<Point>();
            if (start == end)
                points.Add(start);
            else if (start.X == end.X || start.Y == end.Y)
            {
                points.Add(start);
                points.Add(end);
            }
            else
            {
                var vec = end - start;
                points.Add(start);
                points.Add(start + new Vector(vec.X, 0));
                points.Add(start + vec);
                points.Add(start + new Vector(0, vec.Y));
                points.Add(start);
            }
            _polygonCache.Add(points.Select(p => new Vertex(p, true)).ToList());
            _cachedShapes.Add(new PolyLine(points));
        }

        internal void IP()
        {
            if (_isPolygonMode) return;
            _p1 = _hardClip.Location;
            _p2 = _hardClip.Location + new Vector(_hardClip.Width, _hardClip.Height);
            _UpdateScaleTransform();
        }

        internal void IP(Point p1)
        {
            if (_isPolygonMode) return;
            var vec = _p2 - _p1;
            _p1 = p1;
            _p2 = _p1 + vec;
            _UpdateScaleTransform();
        }

        internal void IP(Point p1, Point p2)
        {
            if (_isPolygonMode) return;
            if (p1.X == p2.X)
                p2.X++;
            if (p1.Y == p2.Y)
                p2.Y++;
            _p1 = p1;
            _p2 = p2;
            _UpdateScaleTransform();
        }

        internal void IR()
        {
            IP();
        }

        internal void IR(Point p1)
        {
            if (_isPolygonMode) return;
            var vec = _p2 - _p1;
            _p1 = _hardClip.Location + new Vector(_hardClip.Width * p1.X / 100, _hardClip.Height * p1.Y / 100);
            _p2 = _p1 + vec;
            _UpdateScaleTransform();
        }

        internal void IR(Point p1, Point p2)
        {
            if (_isPolygonMode) return;
            if (p1.X == p2.X)
                p2.X++;
            if (p1.Y == p2.Y)
                p2.Y++;
            _p1 = _hardClip.Location + new Vector(_hardClip.Width * p1.X / 100, _hardClip.Height * p1.Y / 100);
            _p2 = _hardClip.Location + new Vector(_hardClip.Width * p2.X / 100, _hardClip.Height * p2.Y / 100);
            _UpdateScaleTransform();
        }

        internal void IW()
        {
            if (_isPolygonMode) return;
            _softClip = _hardClip;
        }

        internal void IW(Point p1, Point p2)
        {
            if (_isPolygonMode) return;
            _softClip = new Rect(p1 * _scaleTransform, p2 * _scaleTransform);
        }

        internal void LA()
        {
            if (_isPolygonMode) return;
            _lineEnds = LineEnd.Butt;
            _lineJoins = LineJoin.Mitered;
            _miterLimit = 5;
        }

        internal void LA(LineEnd lineEnds, LineJoin lineJoins, double miterLimit)
        {
            if (_isPolygonMode) return;
            _lineEnds = lineEnds;
            _lineJoins = lineJoins;
            _miterLimit = miterLimit;
        }

        internal void LT()
        {
            if (_isPolygonMode) return;
            _isLineTypeDefault = true;
        }

        internal void LT(int lineType)
        {
            if (_isPolygonMode) return;
            _isLineTypeDefault = false;
            _lineType = lineType;
        }

        internal void LT(int lineType, double patternLength)
        {
            if (_isPolygonMode) return;
            LT(lineType);
            _patternLength = patternLength;
        }

        internal void LT(int lineType, double patternLength, bool isRelative)
        {
            if (_isPolygonMode) return;
            LT(lineType, patternLength);
            _isLineTypeRelative = isRelative;
        }

        internal void PW()
        {
            if (_isPolygonMode) return;
            foreach (var pen in _pens.Values)
                pen.ToDefault();
        }

        internal void PW(double lineWidth)
        {
            if (_isPolygonMode) return;
            foreach (var pen in _pens.Values)
                pen.Width = lineWidth;
        }

        internal void PW(int pen, double lineWidth)
        {
            if (_isPolygonMode) return;
            if (_pens.ContainsKey(pen))
                _pens[pen].Width = lineWidth;
        }

        internal void WU(LineWidthType lineWidthType = LineWidthType.Metric)
        {
            if (_isPolygonMode) return;
            foreach (var pen in _pens.Values)
                pen.WidthType = lineWidthType;
        }

        internal void AC(Point? point = null)
        {
            if (_isPolygonMode) return;
            if (point == null)
                _fillOrigin = _hardClip.Location;
            else _fillOrigin = point.Value * _totalTransform;
        }

        internal void SC()
        {
            if (_isPolygonMode) return;
            _isScaleMode = false;
            _scaleTransform = Matrix.Identity;
            _UpdateTotalTransform();
        }

        internal void SC(SCType scType, double para1, double para2, double para3, double para4, double left, double bottom)
        {
            if (_isPolygonMode) return;
            _isScaleMode = true;
            _scType = scType;
            _scaleVec1 = new Vector(para1, para3);
            _scaleVec2 = new Vector(para2, para4);
            _scaleVec3 = new Vector(left, bottom);
            _UpdateScaleTransform();
        }

        private void _UpdateScaleTransform()
        {
            _scaleTransform = Matrix.Identity;
            _scaleTransform.Translate(_scaleVec1.X, _scaleVec1.Y);
            var scaleOrigin = _p1;
            switch (_scType)
            {
                case SCType.Anisotropic:
                    {
                        var xFactor = (_p2.X - _p1.X) / (_scaleVec2.X - _scaleVec1.X);
                        var yFactor = (_p2.Y - _p1.Y) / (_scaleVec2.Y - _scaleVec1.Y);
                        _scaleTransform.Scale(xFactor, yFactor);
                    }
                    break;
                case SCType.Isotropic:
                    {
                        var xSpan = _scaleVec2.X - _scaleVec1.X;
                        var ySpan = _scaleVec2.Y - _scaleVec1.Y;
                        var xFactor = (_p2.X - _p1.X) / xSpan;
                        var yFactor = (_p2.Y - _p1.Y) / ySpan;
                        var scale = xFactor;
                        if (xFactor < yFactor)
                            scaleOrigin = new Point(_p1.X, _p1.Y + (yFactor - xFactor) * ySpan * _scaleVec3.Y / 100);
                        else
                        {
                            scale = yFactor;
                            scaleOrigin = new Point(_p1.X + (xFactor - yFactor) * xSpan * _scaleVec3.X / 100, _p1.Y);
                        }
                        _scaleTransform.Scale(scale, scale);
                    }
                    break;
                case SCType.PointFactor:
                    _scaleTransform.Scale(_scaleVec2.X, _scaleVec2.Y);
                    break;
            }
            _scaleTransform.Translate(scaleOrigin.X, scaleOrigin.Y);
            _UpdateTotalTransform();
        }

        internal void SP(int pen)
        {
            if (_isPolygonMode) return;
            if (pen == 0)
            {
                _currentPen = null;
                return;
            }

            pen = (pen - 1) / 8 + 1;
            _currentPen = _pens[pen];
        }

        internal void RO(RODirection direction = RODirection._0)
        {
            if (_isPolygonMode) return;
            if (_direction != direction)
            {
                _direction = direction;
                _OnRODirectionChanged();
            }
        }

        private void _OnRODirectionChanged()
        {
            _roTransform = Matrix.Identity;
            switch (_direction)
            {
                case RODirection._90:
                    _roTransform.Rotate(90);
                    _roTransform.Translate(_workArea.Width, 0);
                    break;
                case RODirection._180:
                    _roTransform.Rotate(180);
                    _roTransform.Translate(_workArea.Width, _workArea.Height);
                    break;
                case RODirection._270:
                    _roTransform.Rotate(270);
                    _roTransform.Translate(0, _workArea.Height);
                    break;
            }

            switch (_direction)
            {
                case RODirection._0:
                case RODirection._180:
                    _hardClip.Size = _workArea;
                    break;
                case RODirection._90:
                case RODirection._270:
                    _hardClip.Size = new Size(_workArea.Height, _workArea.Width);
                    break;
            }
            _UpdateTotalTransform();
        }

        private void _UpdateTotalTransform()
        {
            _totalTransform = _scaleTransform * _roTransform;
            _totalReverseTransform = _totalTransform;
            _totalReverseTransform.Invert();
        }
    }
}