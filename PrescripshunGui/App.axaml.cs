using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using PrescripshunGui.Util;
using PrescripshunGui.ViewModels;
using PrescripshunGui.Views;
using PrescripshunLib.Logging;

namespace PrescripshunGui;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Init();

        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Startup += (_, _) => GuiEvents.Get.OnApplicationBoot.Invoke([]);
            desktop.Exit += (_, _) => GuiEvents.Get.OnApplicationExit.Invoke([]);
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void Init()
    {
        LogHandler.Configure("gui");
        GuiEvents.Get.RegisterEvents();
        NetworkHandler.Init();
    }
}
