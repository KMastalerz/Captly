using captly.Interfaces;
using captly.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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

        //read Languages
        string languagesDirectory = Path.Combine(sharedDirectory, "languages-list.json");
        string json = File.ReadAllText(languagesDirectory);
        Languages = JsonConvert.DeserializeObject<List<Language>>(json) ?? [];

    }
    public ApplicationConfiguration ApplicationConfiguration { get; set; }
    public string TranslationPrompt { get; set; }
    public List<Language> Languages { get; set; }
}
