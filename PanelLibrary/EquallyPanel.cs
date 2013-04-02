using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PanelLibrary
{
    public class EquallyPanel : Panel
    {
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(EquallyPanel), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var equallyPanel = d as EquallyPanel;
            if (equallyPanel == null) return;
            equallyPanel.InvalidateArrange();
            equallyPanel.UpdateLayout();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var sz = new Size(availableSize.Width, availableSize.Height);
            if (Double.IsPositiveInfinity(availableSize.Width)) sz.Width = 0;
            if (Double.IsPositiveInfinity(availableSize.Height)) sz.Height = 0;

            var c = Children.Count(client => client.Visibility == Visibility.Visible);
            if (c > 0)
            {
                foreach (var client in Children.Where(client => client.Visibility != Visibility.Collapsed))
                {
                    switch (Orientation)
                    {
                        case Orientation.Vertical:
                            client.Measure(new Size(sz.Width, sz.Height / c));
                            break;
                        case Orientation.Horizontal:
                            client.Measure(new Size(sz.Width / c, sz.Height));
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

        protected override Size ArrangeOverride(Size finalSize)
        {
            var sz = new Size(finalSize.Width, finalSize.Height);
            if (Double.IsPositiveInfinity(finalSize.Width)) sz.Width = 0;
            if (Double.IsPositiveInfinity(finalSize.Height)) sz.Height = 0;

            double x = 0, y = 0;
            var c = Children.Count(client => client.Visibility == Visibility.Visible);
            if (c > 0)
            {
                foreach (var client in Children.Where(client => client.Visibility != Visibility.Collapsed))
                {
                    switch (Orientation)
                    {
                        case Orientation.Vertical:
                            client.Arrange(new Rect(x, y, sz.Width, sz.Height / c));
                            y += sz.Height / c;
                            break;
                        case Orientation.Horizontal:
                            client.Arrange(new Rect(x, y, sz.Width / c, sz.Height));
                            x += sz.Width / c;
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