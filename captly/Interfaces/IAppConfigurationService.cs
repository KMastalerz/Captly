using captly.Models;

namespace captly.Interfaces;
public interface IAppConfigurationService
{
    ApplicationConfiguration ApplicationConfiguration { get; }

    Task<IEnumerable<WhisperOption>?> ReadWhisperOptions();
    Task SaveConfiguration(ApplicationConfiguration applicationConfiguration);
    Task SaveWhisperOptions(IEnumerable<WhisperOption> whisperOptions);
    Task<string?> ReadTranslationPrompt();
}