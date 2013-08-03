using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PanelLibrary
{
    public class ProportionalStackPanel : Panel
    {
        public static double GetPosition(DependencyObject obj)
        {
            return (double)obj.GetValue(PositionProperty);
        }

        public static void SetPosition(DependencyObject obj, double value)
        {
            obj.SetValue(PositionProperty, value);
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.RegisterAttached("Position", typeof(double), typeof(ProportionalStackPanel), new PropertyMetadata(0.0));

        public Orientation Orientation { get; set; }

        public ProportionalStackPanel()
        {
            Orientation = Orientation.Horizontal;
        }

        // Calculate Actual Size
        protected override Size MeasureOverride(Size availableSize)
        {
            var size = new Size();
            if (Orientation == Orientation.Horizontal)
            {
                var height = Height;
                var childrenHeight = 0.0;
                foreach (var c in Children)
                {
                    if (c is FrameworkElement)
                    {
                        ((FrameworkElement)c).Measure(availableSize);
                        childrenHeight = Math.Max(childrenHeight, ((FrameworkElement)c).DesiredSize.Height);
                    }
                }
                if (double.IsInfinity(height) || double.IsNaN(height))
                {
                    height = childrenHeight;
                }
                size = new Size(availableSize.Width, height);
            }
            else if (Orientation == Orientation.Vertical)
            {
                var width = Width;
                var childrenWidth = 0.0;
                foreach (var c in Children)
                {
                    if (c is FrameworkElement)
                    {
                        ((FrameworkElement)c).Measure(availableSize);
                        childrenWidth = Math.Max(childrenWidth, ((FrameworkElement)c).DesiredSize.Width);
                    }
                }
                if (double.IsInfinity(width) || double.IsNaN(width))
                {
                    width = childrenWidth;
                }
                size = new Size(width, availableSize.Height);
            }
            return size;
        }

        // Layout children
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var c in Children)
            {
                if (!(c is FrameworkElement)) continue;
                var e = (FrameworkElement)c;
                var pos = GetPosition(e);
                pos *= Orientation == Orientation.Horizontal ? (finalSize.Width - e.DesiredSize.Width) : (finalSize.Height - e.DesiredSize.Height);
                var size = e.DesiredSize;
                Point loc;
                if (Orientation == Orientation.Horizontal)
                {
                    loc = new Point(pos, 0);
                }
                else
                {
                    loc = new Point(0, pos);
                }
                e.Arrange(new Rect(loc, size));
            }
            return finalSize;
        }
    }
}
