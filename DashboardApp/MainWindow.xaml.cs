// DashboardApp/MainWindow.xaml.cs
using Contracts;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DashboardApp;

public partial class MainWindow : Window
{
    [Import] public IEventAggregator Bus { get; set; } = default!;
    [ImportMany] public IEnumerable<Lazy<IWidget, IDictionary<string, object>>> Widgets { get; set; } = default!;

    public MainWindow()
    {
        InitializeComponent();
        App.Container.SatisfyImports(this);

        LoadTabs();
        WatchPlugins();
    }

    private void LoadTabs()
    {
        Tabs.Items.Clear();
        foreach (var it in Widgets)
        {
            var header = it.Metadata.TryGetValue("Name", out var n) ? n?.ToString() : it.Value.Name;
            Tabs.Items.Add(new TabItem { Header = header, Content = it.Value.View });
        }
    }

    private void Send_Click(object sender, RoutedEventArgs e)
        => Bus.Publish(new DataSubmittedEvent(InputBox.Text));

    private void WatchPlugins()
    {
        var fsw = new FileSystemWatcher(App.PluginsDir, "*.dll")
        { EnableRaisingEvents = true, IncludeSubdirectories = false };
        fsw.Created += (_, __) => Dispatcher.Invoke(Recompose);
        fsw.Deleted += (_, __) => Dispatcher.Invoke(Recompose);
        fsw.Changed += (_, __) => Dispatcher.Invoke(Recompose);
        fsw.Renamed += (_, __) => Dispatcher.Invoke(Recompose);
    }

    private void Recompose()
    {
        // przebuduj kontener na podstawie aktualnych plików w Widgets (skopiowanych do cache)
        App.Container.Dispose();
        App.Container = App.BuildContainer();

        // ponownie wstrzyknij zależności do aktualnego okna i przeładuj zakładki
        App.Container.SatisfyImports(this);
        LoadTabs();
    }
}
