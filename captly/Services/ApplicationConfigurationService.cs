using captly.Interfaces;
using captly.Model;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace captly.Services;
internal class ApplicationConfigurationService : IApplicationConfigurationService
{
    public ApplicationConfigurationService(IConfiguration configuration)
    {
        //read ApplicationConfiguration from config
        ApplicationConfiguration = configuration.GetSection("ApplicationConfiguration").Get<ApplicationConfiguration>() ?? new();

        //read TranslationPrompt
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string sharedDirectory = Path.Combine(appDirectory, "Shared");
        string translationPromptDirectory = Path.Combine(sharedDirectory, "translation-prompt.txt");
        TranslationPrompt = File.ReadAllText(translationPromptDirectory);
    }
    public ApplicationConfiguration ApplicationConfiguration { get; set; }
    public string TranslationPrompt { get; set; }
}
