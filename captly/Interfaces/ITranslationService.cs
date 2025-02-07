using captly.Models;
using captly.ViewModels;

namespace captly.Interfaces;
public interface ITranslationService
{
    Task TranslateFile(TranslationViewModel translationViewModel, CancellationToken cancellationToken);
    TranslationSetup GetGlobalTranslationSetup();
}