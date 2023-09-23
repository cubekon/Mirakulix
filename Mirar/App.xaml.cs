using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

using Mirar.Activation;
using Mirar.Contracts.Services;
using Mirar.Core.Contracts.Services;
using Mirar.Core.Services;
using Mirar.Helpers;
using Mirar.Models;
using Mirar.Notifications;
using Mirar.Services;
using Mirar.ViewModels;
using Mirar.Views;
using Mirar.Views.Projector;
using Windows.Devices.Display.Core;

namespace Mirar;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging

    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static ProjectorWindow ProjectorWindow { get; } = new ProjectorWindow();

    public static UIElement? AppTitlebar
    {
        get; set;
    }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers
            services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

            // Services
            services.AddSingleton<IAppNotificationService, AppNotificationService>();
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IDisplayService, DisplayService>();
            services.AddSingleton<IDisplayWatcherService, DisplayWatcherService>();
            services.AddTransient<IWebViewService, WebViewService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<ISampleDataService, SampleDataService>();
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<WebViewModel>();
            services.AddTransient<WebPage>();
            services.AddTransient<TableViewModel>();
            services.AddTransient<TablePage>();
            services.AddTransient<GridDetailViewModel>();
            services.AddTransient<GridDetailPage>();
            services.AddTransient<GridViewModel>();
            services.AddTransient<GridPage>();
            services.AddTransient<ListViewModel>();
            services.AddTransient<ListPage>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            // Projector
            services.AddTransient<PictureFrame>();
            services.AddTransient<DemoFrame>();

            services.AddTransient<ProjectorViewModel>();

            // Configuration
            //services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));

            services.Configure<LocalSettingsOptions>(options =>
            {
                options.ApplicationDataFolder = "Mirar\\AppData";
                options.LocalSettingsFile = "test.json";
            });


        }).
        Build();

        App.GetService<IAppNotificationService>().Initialize();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        // TODO: Show App Notification at a different spot.
        //App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));

        /* 
         * Activate the app window.
         * Using ActivationService
         Task:
            - Load Theme
            - Reset MainWindow Content
            - Load Shell
            - Activate MainWindow
        */
        await App.GetService<IDisplayWatcherService>().InitializeAsync();

        await App.GetService<IActivationService>().ActivateAsync(args);
    }

    
}
