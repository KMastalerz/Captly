using captly.Model;

namespace captly.Interfaces;
public interface IApplicationConfigurationService
{
    ApplicationConfiguration ApplicationConfiguration { get; set; }
    string TranslationPrompt { get; set; }
    List<Language> Languages { get; set; }
}