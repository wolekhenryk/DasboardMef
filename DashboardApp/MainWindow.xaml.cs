// DashboardApp/MainWindow.xaml.cs
using Contracts;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DashboardApp;

public partial class MainWindow : Window
{
    private FileSystemWatcher? _fsw;
    private bool _isRecomposing;
    private readonly DispatcherTimer _debounceTimer;

    public MainWindow()
    {
        InitializeComponent();


        _debounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        _debounceTimer.Tick += (_, __) => { _debounceTimer.Stop(); Recompose(); };

        LoadTabs();
        WatchPlugins();
    }
    private IEventAggregator ResolveBus()
        => App.Container.GetExport<IEventAggregator>();

    private IEnumerable<Lazy<IWidget, IDictionary<string, object>>> ResolveWidgets()
        => App.Container.GetExports<Lazy<IWidget, IDictionary<string, object>>>();

    private void LoadTabs()
    {
        foreach (var item in Tabs.Items.Cast<TabItem>().ToList())
            item.Content = null;

        Tabs.Items.Clear();

        foreach (var it in ResolveWidgets())
        {
            var header = it.Metadata.TryGetValue("Name", out var n) ? n?.ToString() : it.Value.Name;
            Tabs.Items.Add(new TabItem { Header = header, Content = it.Value.View });
        }
    }

    private void Send_Click(object sender, RoutedEventArgs e)
        => ResolveBus().Publish(new DataSubmittedEvent(InputBox.Text));

    private void WatchPlugins()
    {
        _fsw = new FileSystemWatcher(App.PluginsDir, "*.dll")
        {
            IncludeSubdirectories = false,
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
        };

        void trigger(object? _, FileSystemEventArgs __)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        _fsw.Created += trigger;
        _fsw.Changed += trigger;
        _fsw.Deleted += trigger;
        _fsw.Renamed += (_, __) => trigger(_, null!);
    }

    private void Recompose()
    {
        if (_isRecomposing) return;
        _isRecomposing = true;

        try
        {
            App.Container.Dispose();
            App.Container = App.BuildContainer();

            LoadTabs();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Recompose failed:\n{ex.Message}", "MEF", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            _isRecomposing = false;
        }
    }
}
