using captly.Interfaces;
using captly.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;

namespace captly.Services;
public class AppConfigurationService : IAppConfigurationService
{
    public ApplicationConfiguration ApplicationConfiguration { get; private set; }
    public AppConfigurationService(IConfiguration configuration)
    {
        ApplicationConfiguration = configuration.GetSection("ApplicationConfiguration").Get<ApplicationConfiguration>() ?? new();
    }

    public async Task SaveConfiguration(ApplicationConfiguration applicationConfiguration)
    {
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string appSettingsPath = Path.Combine(appDirectory, "appsettings.json");

        ApplicationConfiguration = applicationConfiguration;

        await File.WriteAllTextAsync(appSettingsPath, JsonConvert.SerializeObject(applicationConfiguration, Formatting.Indented));
    }

    public async Task<IEnumerable<WhisperOption>?> ReadWhisperOptions()
    {
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string sharedDirectory = Path.Combine(appDirectory, "Shared");
        string whisperFile = Path.Combine(sharedDirectory, "whisper-options.json");

        if (!File.Exists(whisperFile))
            return null;

        string optionsJson = await File.ReadAllTextAsync(whisperFile);

        try
        {
            return JsonConvert.DeserializeObject<IEnumerable<WhisperOption>>(optionsJson);
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveWhisperOptions(IEnumerable<WhisperOption> whisperOptions)
    {
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string sharedDirectory = Path.Combine(appDirectory, "Shared");
        string whisperPath = Path.Combine(sharedDirectory, "whisper-options.json");

        if (!Directory.Exists(sharedDirectory))
        {
            Directory.CreateDirectory(sharedDirectory);
        }

        await File.WriteAllTextAsync(whisperPath, JsonConvert.SerializeObject(whisperOptions, Formatting.Indented));
    }

    public async Task<string?> ReadTranslationPrompt()
    {
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string sharedDirectory = Path.Combine(appDirectory, "Shared");
        string translationPromptPath = Path.Combine(sharedDirectory, "translation-prompt.txt");

        if (!File.Exists(translationPromptPath))
            return null;

        return await File.ReadAllTextAsync(translationPromptPath);
    }
}
