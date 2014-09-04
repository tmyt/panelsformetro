using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PanelLibrary
{
    // UniformGrid的なやつ
    public class UniformPanel : Panel
    {
        public double MaxItems
        {
            get { return (double)GetValue(MaxItemsProperty); }
            set { SetValue(MaxItemsProperty, value); }
        }

        public static readonly DependencyProperty MaxItemsProperty =
            DependencyProperty.Register("MaxItems", typeof(double), typeof(UniformPanel), new PropertyMetadata(double.NaN, OnMaxItemsChanged));

        private static void OnMaxItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 上限項目数が変更されたので再配置する
            ((UniformPanel)d).InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var ret = new Size(
                double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width,
                double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height);
            // 配置を計算する
            var rects = ArrangeElements(ret);
            for (var i = 0; i < rects.Length; ++i)
            {
                Children[i].Measure(new Size(rects[i].Width, rects[i].Height));
            }
            return ret;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var ret = new Size(
                double.IsInfinity(finalSize.Width) ? 0 : finalSize.Width,
                double.IsInfinity(finalSize.Height) ? 0 : finalSize.Height);
            // 配置を計算する
            var rects = ArrangeElements(ret);
            for (var i = 0; i < rects.Length; ++i)
            {
                Children[i].Arrange(rects[i]);
            }
            return ret;
        }

        private Rect[] ArrangeElements(Size availableSize)
        {
            var ret = new Size(
                double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width,
                double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height);
            var calculated = new Rect[Children.Count];

            var items = (int)(double.IsNaN(MaxItems) ? Children.Count : Math.Min(MaxItems, Children.Count));
            // 表示しない項目を0,0にする
            for (var i = items; i < Children.Count; ++i)
            {
                calculated[i] = new Rect(0, 0, 0, 0);
            }

            // 0,1だけ専用
            if (items == 0) return calculated;
            if (items == 1)
            {
                calculated[0] = new Rect(0, 0, ret.Width, ret.Height);
                return calculated;
            }

            // 必要な列と行を計算
            var cols = Math.Ceiling(Math.Sqrt(items));
            var rows = Math.Floor(Math.Sqrt(items) + 0.5);

            // 左端を配置
            var rest = (int)(items - (cols - 1) * rows);
            for (var i = 0; i < rest; ++i)
            {
                var width = Math.Max(0, ret.Width / cols - 2);
                var height = Math.Max(0, ret.Height / rest - 2);
                var top = i == 0 ? 0 : Math.Max(0, (ret.Height / rest) * i + 2);
                calculated[i] = new Rect(0, top, width, height);
            }

            // 残りを配置
            for (var i = rest; i < items; i++)
            {
                var col = (int)(((i - rest) + rows) / rows);
                var row = (int)((i - rest) % rows);
                var width = Math.Max(0, ret.Width / cols - 2);
                var height = Math.Max(0, ret.Height / rows - 2);
                var top = row == 0 ? 0 : Math.Max(0, (ret.Height / rows) * row + 2);
                var left = Math.Max(0, (ret.Width / cols) * col + 2);
                calculated[i] = new Rect(left, top, width, height);
            }
            return calculated;
        }
    }
}
