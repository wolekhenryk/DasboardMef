// TextWidget/TextWidgetWidget.cs
using Contracts;
using System.Composition;
using System.Windows;

namespace TextWidget;

[Export(typeof(IWidget))]
[ExportMetadata("Name", "Analizator tekstu")]
public sealed class TextWidgetWidget : IWidget
{
    private readonly TextWidgetView _view = new();
    public string Name => "Analizator tekstu";
    public object View => _view;

    [ImportingConstructor]
    public TextWidgetWidget(IEventAggregator bus)
    {
        bus.Subscribe<DataSubmittedEvent>(e =>
        {
            Application.Current.Dispatcher.Invoke(() => _view.ShowStats(e.Data));
        });
    }
}