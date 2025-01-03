using captly.Model;

namespace captly.Interfaces;
internal interface IApplicationConfigurationService
{
    ApplicationConfiguration ApplicationConfiguration { get; set; }
    string TranslationPrompt { get; set; }
}