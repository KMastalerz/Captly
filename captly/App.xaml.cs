using captly.Interfaces;
using captly.Model;
using captly.Services;
using captly.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;

namespace captly;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    // Define the ServiceProvider for dependency injection
    public static IServiceProvider ServiceProvider { get; private set; } = default!;
    public static IConfiguration Configuration { get; private set; } = default!;

    protected override void OnStartup(StartupEventArgs e)
    {
        // Initialize the dependency injection container
        var services = new ServiceCollection();

        CheckAppSettings();

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();

        //register configuration
        services.AddSingleton(Configuration);

        ConfigureServices(services);
        ConfigureViewModels(services);
        ConfigureViews(services);

        ServiceProvider = services.BuildServiceProvider();

        base.OnStartup(e);
    }

    private void CheckAppSettings()
    {
        var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

        // Check if appsettings.json exists
        if (!File.Exists(appSettingsPath))
        {
            // Create a default ApplicationConfiguration object
            var defaultConfig = new ApplicationConfiguration();

            // Serialize the default configuration to JSON
            var jsonContent = System.Text.Json.JsonSerializer.Serialize(
                new { ApplicationConfiguration = defaultConfig },
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
            );

            // Write to appsettings.json
            File.WriteAllText(appSettingsPath, jsonContent);
        }
    }
    private void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<IDisplayService, DisplayService>();
        services.AddSingleton<IApplicationCacheService, ApplicationCacheService>();
        services.AddSingleton<IApplicationCacheStateService, ApplicationCacheStateService>();
        services.AddSingleton<IApplicationConfigurationService, ApplicationConfigurationService>();
        services.AddSingleton<ITranslateStateService, TranslateStateService>();
        services.AddTransient<ISubtitleTranslationService, SubtitleTranslationService>();
        services.AddSingleton<IPromptService, PromptService>();
    }
    private void ConfigureViewModels(ServiceCollection services)
    {
        // Register ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<TranslateViewModel>();
        services.AddTransient<TranslateOptionsViewModel>();
    }
    private void ConfigureViews(ServiceCollection services)
    {
        // Register Views
        services.AddSingleton<MainWindow>();
        services.AddSingleton<Translate>();
        services.AddTransient<TranslateOptions>();
    }
}

