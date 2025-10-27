// App.xaml.cs
using Contracts;
using System.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Windows;

namespace DashboardApp;

public partial class App : Application
{
    public static CompositionHost Container { get; set; } = default!;
    public static string PluginsDir => Path.Combine(AppContext.BaseDirectory, "Widgets");
    public static string ShadowDir => Path.Combine(AppContext.BaseDirectory, "WidgetsCache");

    protected override void OnStartup(StartupEventArgs e)
    {
        Directory.CreateDirectory(PluginsDir);
        Directory.CreateDirectory(ShadowDir);

        Container = BuildContainer();
        base.OnStartup(e);
    }

    // Buduje kontener z DLL-i skopiowanych do ShadowDir
    public static CompositionHost BuildContainer()
    {
        var assemblies = new List<Assembly> { typeof(App).Assembly, typeof(IWidget).Assembly };

        foreach (var src in Directory.EnumerateFiles(PluginsDir, "*.dll"))
        {
            // unikalny folder na kopię – pozwala podmieniać pliki w Widgets
            var dstFolder = Path.Combine(ShadowDir,
                Path.GetFileNameWithoutExtension(src) + "_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dstFolder);

            var dst = Path.Combine(dstFolder, Path.GetFileName(src));
            File.Copy(src, dst, overwrite: true);

            var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(dst);
            assemblies.Add(asm);
        }

        return new ContainerConfiguration().WithAssemblies(assemblies).CreateContainer();
    }
}
