using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PanelLibrary
{
    public class WrapPanel : Panel
    {
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(WrapPanel), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as WrapPanel;
            if (panel == null) return;
            panel.InvalidateArrange();
            panel.UpdateLayout();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var sz = new Size(finalSize.Width, finalSize.Height);
            if (Double.IsPositiveInfinity(finalSize.Width)) sz.Width = 0;
            if (Double.IsPositiveInfinity(finalSize.Height)) sz.Height = 0;
            double x = 0, xx = 0, y = 0, yy = 0;
            if (Children.Any(_ => _.Visibility == Visibility.Visible))
            {
                foreach (var client in Children.Cast<FrameworkElement>())
                {
                    var w = client.DesiredSize.Width + client.Margin.Left + client.Margin.Right;
                    var h = client.DesiredSize.Height + client.Margin.Top + client.Margin.Bottom;
                    if (client.Visibility == Visibility.Collapsed) continue;
                    switch (Orientation)
                    {
                        case Orientation.Vertical:
                            h = double.IsNaN(h) ? client.Margin.Top + client.Margin.Bottom : h;
                            w = double.IsNaN(w) ? xx : w;
                            if (y + h > sz.Height)
                            {
                                y = 0;
                                x = xx;
                            }
                            xx = Math.Max(double.IsNaN(xx) ? 0 : xx, x + w);
                            client.Arrange(new Rect(x + Math.Max(0, (xx - x - w) / 2), y, w, h));
                            y += h;
                            sz.Width = Math.Max(sz.Width, x + w);
                            sz.Height = Math.Max(sz.Height, y);
                            break;
                        case Orientation.Horizontal:
                            w = double.IsNaN(w) ? 0 : w;
                            h = double.IsNaN(h) ? client.Margin.Left + client.Margin.Right : h;
                            if (x + w > sz.Width)
                            {
                                x = 0;
                                y = yy;
                            }
                            yy = Math.Max(double.IsNaN(yy) ? 0 : yy, y + h);
                            client.Arrange(new Rect(x, y + Math.Max(0, (yy - y - h) / 2), w, h));
                            x += w;
                            sz.Width = Math.Max(sz.Width, x);
                            sz.Height = Math.Max(sz.Height, y + h);
                            break;
                    }
                }
            }
            else
            {
                switch (Orientation)
                {
                    case Orientation.Vertical:
                        sz.Height = 0;
                        break;
                    case Orientation.Horizontal:
                        sz.Width = 0;
                        break;
                }
            }
            return sz;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var sz = new Size(availableSize.Width, availableSize.Height);
            if (double.IsInfinity(sz.Width)) sz.Width = 0;
            if (double.IsInfinity(sz.Height)) sz.Height = 0;
            double x = 0, xx = 0, y = 0, yy = 0;
            if (Children.Any(_ => _.Visibility == Visibility.Visible))
            {
                foreach (var client in Children.Cast<FrameworkElement>())
                {
                    client.Measure(availableSize);
                    var w = client.DesiredSize.Width + client.Margin.Left + client.Margin.Right;
                    var h = client.DesiredSize.Height + client.Margin.Top + client.Margin.Bottom;
                    if (client.Visibility == Visibility.Collapsed) continue;
                    switch (Orientation)
                    {
                        case Orientation.Vertical:
                            h = double.IsNaN(h) ? client.Margin.Top + client.Margin.Bottom : h;
                            w = double.IsNaN(w) ? xx : w;
                            if (y + h > availableSize.Height)
                            {
                                y = 0;
                                x = xx;
                            }
                            xx = Math.Max(double.IsNaN(xx) ? 0 : xx, x + w);
                            y += h;
                            sz.Width = Math.Max(sz.Width, x + w);
                            sz.Height = Math.Max(sz.Height, y);
                            break;
                        case Orientation.Horizontal:
                            w = double.IsNaN(w) ? 0 : w;
                            h = double.IsNaN(h) ? client.Margin.Left + client.Margin.Right : h;
                            if (x + w > availableSize.Width)
                            {
                                x = 0;
                                y = yy;
                            }
                            yy = Math.Max(double.IsNaN(yy) ? 0 : yy, y + h);
                            x += w;
                            sz.Width = Math.Max(sz.Width, x);
                            sz.Height = Math.Max(sz.Height, y + h);
                            break;
                    }
                }
            }
            else
            {
                switch (Orientation)
                {
                    case Orientation.Vertical:
                        sz.Height = 0;
                        break;
                    case Orientation.Horizontal:
                        sz.Width = 0;
                        break;
                }
            }
            return sz;
        }
    }
}