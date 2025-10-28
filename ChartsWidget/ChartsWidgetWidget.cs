// ChartsWidget/ChartsWidgetWidget.cs
using Contracts;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;

namespace ChartsWidget
{
    [Export(typeof(IWidget))]
    [ExportMetadata("Name", "Wykres słupkowy")]
    public sealed class ChartsWidgetWidget : IWidget
    {
        private readonly ChartsWidgetView _view = new();

        public string Name => "Wykres słupkowy";
        public object View => _view;

        [ImportingConstructor]
        public ChartsWidgetWidget(IEventAggregator bus)
        {
            bus.Subscribe<DataSubmittedEvent>(e =>
            {
                var nums = ParseNumbers(e.Data);
                Application.Current.Dispatcher.Invoke(() => _view.Render(nums));
            });
        }

        private static double[] ParseNumbers(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return [];

            var matches = Regex.Matches(text, @"-?\d+(?:[.,]\d+)?");
            var list = new List<double>(matches.Count);

            foreach (Match m in matches)
            {
                var t = m.Value.Replace(',', '.');
                if (double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) &&
                    double.IsFinite(d))
                {
                    list.Add(Math.Max(0, d));
                }
            }

            return [.. list];
        }
    }
}
