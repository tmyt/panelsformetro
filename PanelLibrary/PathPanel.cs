using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace PanelLibrary
{
    public class PathPanel : Panel
    {
        private const int N = 16;

        public Path Path
        {
            get { return (Path)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(Path), typeof(PathPanel), new PropertyMetadata(null, OnPathChangedHandler));

        private static void OnPathChangedHandler(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var pathPanel = dependencyObject as PathPanel;
            if (pathPanel != null) pathPanel.OnPathChanged();
        }

        protected void OnPathChanged()
        {
            InvalidateArrange();
        }

        // Ref: http://geom.web.fc2.com/geometry/bezier/cubic.html
        private Point GetPoint(double t, Point start, Point ctrl1, Point ctrl2, Point end)
        {
            var tp = 1 - t;
            var x = t * t * t * end.X + 3 * t * t * tp * ctrl2.X + 3 * t * tp * tp * ctrl1.X + tp * tp * tp * start.X;
            var y = t * t * t * end.Y + 3 * t * t * tp * ctrl2.Y + 3 * t * tp * tp * ctrl1.Y + tp * tp * tp * start.Y;
            return new Point(x, y);
        }

        private double CalcDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        private double CalcBezierLength()
        {
            var geometry = (PathGeometry)Path.Data;
            var p = geometry.Figures[0].StartPoint;
            var l = 0.0;
            foreach (var segment in geometry.Figures[0].Segments.Cast<BezierSegment>())
            {
                var q = p;
                for(var i = 1; i <= N; ++i)
                {
                    var r = GetPoint(1.0 / N * i, p, segment.Point1, segment.Point2, segment.Point3);
                    l += CalcDistance(q, r);
                    q = r;
                }
                p = segment.Point3;
            }
            return l;
        }

        private Point GetApproxPoint(double t)
        {
            var geometry = (PathGeometry)Path.Data;
            var p = geometry.Figures[0].StartPoint;
            var l = 0.0;
            if (Math.Abs(t - 0.0) < 0.0000001) return p;
            foreach (var segment in geometry.Figures[0].Segments.Cast<BezierSegment>())
            {
                var q = p;
                for (var i = 1; i <= N; ++i)
                {
                    var r = GetPoint(1.0 / N * i, p, segment.Point1, segment.Point2, segment.Point3);
                    l += CalcDistance(q, r);
                    if (l >= t) return r;
                    q = r;
                }
                p = segment.Point3;
            }
            return p;
        }

        private Point GetCenterPoint(Point p, Size r)
        {
            return new Point(r.Width / 2 + p.X, r.Height / 2 + p.Y);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            CalcBezierLength();
            foreach (var child in Children)
            {
                child.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0) return base.ArrangeOverride(finalSize);
            // 等間隔にコントロールを配置する
            var totalLength = CalcBezierLength();
            var segmentLength = totalLength / Children.Count;
            var t = 0.0;
            foreach (var child in Children)
            {
                child.Arrange(new Rect(GetCenterPoint(GetApproxPoint(t), child.DesiredSize), child.DesiredSize));
                t += segmentLength;
            }
            return base.ArrangeOverride(finalSize);
        }
    }
}
