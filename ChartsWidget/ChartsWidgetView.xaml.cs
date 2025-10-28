using System;
using System.Linq;
using System.Windows.Controls;

namespace ChartsWidget
{
    public partial class ChartsWidgetView : UserControl
    {
        public ChartsWidgetView() => InitializeComponent();

        private sealed record Bar(double Height, string Label);

        public void Render(double[] values)
        {
            const double maxPixels = 160.0;
            if (values == null || values.Length == 0)
            {
                Bars.ItemsSource = Array.Empty<Bar>();
                return;
            }

            var max = values.Max();
            if (max <= 0) max = 1;

            var data = values.Select(v =>
            {
                var h = v <= 0 ? 2.0 : (v / max) * maxPixels;
                return new Bar(Math.Max(2.0, h), v.ToString("0.##"));
            }).ToArray();

            Bars.ItemsSource = data;
        }
    }
}
