using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CafeApp.Models;
using CafeApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;

namespace CafeApp;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; }
    
    public new static App Current => (App)Application.Current!;

    public static User? CurrentUser { get; set; }
    
    public static Shift? CurrentShift { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Services = ConfigureServices();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    public static IServiceProvider ConfigureServices()
    {
        var collection = new ServiceCollection();
        collection.AddDbContext<CafeDbContext>();
        
        ExcelPackage.License.SetNonCommercialPersonal("Even1s");

        var services = collection.BuildServiceProvider();
        
        return services;
    }
}