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
            _polygonCache = new List<Point>();
            _pens = new Dictionary<int, Pen>();
            for (int i = 1; i <= 8; i++)
                _pens.Add(i, new Pen());
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

        #region RO Inst
        public RODirection Direction { get { return _direction; } }
        private RODirection _direction;

        private Matrix _roTransform;
        #endregion

        #region Polygon
        public bool IsPolygonMode { get { return _isPolygonMode; } }
        private bool _isPolygonMode;

        private List<Point> _polygonCache;
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

        public void IN()
        {
            DF();

        }

        public void DF()
        {
            AC();
            IW();
            LA();
            LT();
            PA();
            PM(0);
            PM(2);
            SC();
        }

        public void PM()
        {
            PM(0);
        }

        public void PM(int funcCode)
        {
            switch (funcCode)
            {
                case 0:
                    {
                        _isPolygonMode = true;
                        _polygonCache.Clear();
                    }
                    break;
                case 1:
                case 2:
                    {
                        _ClosePolygon();
                        if (funcCode == 2)
                            _isPolygonMode = false;
                    }
                    break;
            }
        }

        private void _ClosePolygon()
        {

        }

        public void PA(IEnumerable<Point> points = null)
        {
            _isRelative = false;
            if (points == null) return;
        }

        public void IW()
        {
            _softClip = _hardClip;
        }

        public void IW(Point p1, Point p2)
        {
            _softClip = new Rect(p1 * _scaleTransform, p2 * _scaleTransform);
        }

        public void LA()
        {
            _lineEnds = LineEnd.Butt;
            _lineJoins = LineJoin.Mitered;
            _miterLimit = 5;
        }

        public void LA(LineEnd lineEnds, LineJoin lineJoins, double miterLimit)
        {
            _lineEnds = lineEnds;
            _lineJoins = lineJoins;
            _miterLimit = miterLimit;
        }

        public void LT()
        {
            _isLineTypeDefault = true;
        }

        public void LT(int lineType)
        {
            _isLineTypeDefault = false;
            _lineType = lineType;
        }

        public void LT(int lineType, double patternLength)
        {
            LT(lineType);
            _patternLength = patternLength;
        }

        public void LT(int lineType, double patternLength, bool isRelative)
        {
            LT(lineType, patternLength);
            _isLineTypeRelative = isRelative;
        }

        public void AC(Point? point = null)
        {
            if (point == null)
                _fillOrigin = _hardClip.Location;
            else _fillOrigin = point.Value * _scaleTransform;
        }

        public void SC()
        {
            _isScaleMode = false;
            _scaleTransform = Matrix.Identity;
        }

        public void SC(SCType scType, double para1, double para2, double para3, double para4, double left, double bottom)
        {
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
        }

        public void SP(int pen)
        {
            if (pen == 0)
            {
                _currentPen = null;
                return;
            }

            pen = (pen - 1) / 8 + 1;
            _currentPen = _pens[pen];
        }

        public void RO(RODirection direction)
        {
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
        }
    }
}