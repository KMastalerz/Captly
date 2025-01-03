using captly.Interfaces;
using captly.Services;
using captly.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using SW = System.Windows;

namespace captly;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : SW.Application
{
    // Define the ServiceProvider for dependency injection
    public static IServiceProvider ServiceProvider { get; private set; } = default!;
    public static IConfiguration Configuration { get; private set; } = default!;

    protected override void OnStartup(SW.StartupEventArgs e)
    {
        // Initialize the dependency injection container
        var services = new ServiceCollection();

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();

        ConfigureServices(services);
        ConfigureViewModels(services);
        ConfigureViews(services);

        ServiceProvider = services.BuildServiceProvider();

        base.OnStartup(e);
    }

    private void ConfigureServices(ServiceCollection services)
    {

        // Register HttpClient with the API base address from appsettings
        //services.AddTransient<AuthenticatedHttpClientHandler>();
        //services.AddHttpClient("ApiClient", client =>
        //{
        //    var apiBaseAddress = Configuration.GetValue<string>("ApiBaseAddress");
        //    if (!string.IsNullOrEmpty(apiBaseAddress))
        //        client.BaseAddress = new Uri(apiBaseAddress);
        //}).AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

        //services.AddHttpClient("ApiBaseClient", client =>
        //{
        //    var apiBaseAddress = Configuration.GetValue<string>("ApiBaseAddress");
        //    if (!string.IsNullOrEmpty(apiBaseAddress))
        //        client.BaseAddress = new Uri(apiBaseAddress);
        //});

        services.AddTransient<ISubtitleTranslationService, SubtitleTranslationService>();
        services.AddTransient<IPromptService, PromptService>();
        //services.AddSingleton<IAuthService, AuthService>();
        //services.AddSingleton<IDesignerService, DesignerService>();
        //services.AddSingleton<IWebPageStateService, WebPageStateService>();
        //services.AddSingleton<IWindowService, WindowService>();
        //services.AddSingleton<IWebPageEditService, WebPageEditService>();
    }
    private void ConfigureViewModels(ServiceCollection services)
    {
        // Register ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<TranslateViewModel>();

    }

    private void ConfigureViews(ServiceCollection services)
    {
        // Register Views
        services.AddSingleton<MainWindow>();
        services.AddSingleton<Translate>();
    }
}

