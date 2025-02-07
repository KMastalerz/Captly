using captly.Interfaces;
using captly.Models;
using captly.Services;
using captly.Views.Transcription;
using captly.Views.Translation;
using captly.Views.WhisperOptionsSetup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace captly;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = default!;
    public static IConfiguration Configuration { get; private set; } = default!;
    public static T GetService<T>() where T : class
    {
        return ServiceProvider.GetService(typeof(T)) as T ?? throw new InvalidOperationException($"Service {typeof(T)} not found.");
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();

        CheckAppSettings();

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();

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

        if (!File.Exists(appSettingsPath))
        {
            var defaultConfig = new ApplicationConfiguration();

            var jsonContent = JsonSerializer.Serialize(
                new { ApplicationConfiguration = defaultConfig },
                new JsonSerializerOptions { WriteIndented = true }
            );

            File.WriteAllText(appSettingsPath, jsonContent);
        }
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<IAppConfigurationService, AppConfigurationService>();
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<ISubtitileService, SubtitileService>();
        services.AddSingleton<ITranscriptionService, TranscriptionService>();
        services.AddSingleton<ITranslationService, TranslationService>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    private void ConfigureViewModels(ServiceCollection services)
    {
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<TranslationListViewModel>();
        services.AddSingleton<TranscriptionListViewModel>();
        services.AddSingleton<WhisperOptionsSetupViewModel>();
    }

    private void ConfigureViews(ServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<TranslationList>();
        services.AddSingleton<TranscriptionList>();
        services.AddSingleton<WhisperOptionsSetup>();
    }
}
