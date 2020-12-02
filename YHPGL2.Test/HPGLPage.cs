using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace YHPGL2.Test
{
    public class HPGLPage : UIElement
    {
        private static System.Windows.Media.Pen _pen;

        static HPGLPage()
        {
            _pen = new System.Windows.Media.Pen(Brushes.Black, 1);
        }

        public HPGLPage()
        {
            _transform = new MatrixTransform();
        }

        public IEnumerable<IShape> Shapes 
        { 
            get { return _shapes; } 
            set
            { 
                _shapes = value.ToList();
                InvalidateVisual();
            }
        }
        private List<IShape> _shapes;

        private MatrixTransform _transform;
        private Matrix _matrix;

        public void Zoom(double scaleX, double scaleY)
        {
            Zoom(scaleX, scaleY, 0, 0);
        }

        public void Zoom(double scaleX, double scaleY, double centerX, double centerY)
        {
            _matrix.ScaleAt(scaleX, scaleY, centerX, centerY);
            _transform.Matrix = _matrix;
            _pen.Thickness = 1 / _matrix.M11;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (_shapes != null)
            {
                drawingContext.PushTransform(_transform);
                var geo = new StreamGeometry();
                using (var stream = geo.Open())
                {
                    foreach (var shape in _shapes)
                        _DrawShape(stream, shape);
                }
                drawingContext.DrawGeometry(null, _pen, geo);
            }
        }

        private void _DrawShape(StreamGeometryContext stream, IShape shape)
        {
            switch (shape.Type)
            {
                case ShapeType.PolyLine:
                    {
                        var polyLine = (PolyLine)shape;
                        var points = polyLine.Points.Select(p => new Point(p.X / 40, RenderSize.Height - p.Y / 40));
                        stream.BeginFigure(points.First(), false, polyLine.IsClosed);
                        stream.PolyLineTo(points.Skip(1).ToList(), true, true);
                    }
                    break;
                case ShapeType.Polygon:
                    break;
                case ShapeType.Complex:
                    break;
            }
        }
    }
}