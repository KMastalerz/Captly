using captly.Core;
using captly.Interfaces;
using captly.Model;

namespace captly.Views;
public class TranslateOptionsViewModel(IPromptService promptService, IApplicationConfigurationService applicationConfigurationService, ITranslateStateService translateStateService): BaseViewModel
{
    public string SelectedLanguage
    {
        get => translateStateService.SelectedSubtitle!.Language;
        set => SetSubProperty(translateStateService, s => s.SelectedSubtitle!.Language, (s, v) => s.SelectedSubtitle!.Language = v, value);
    }
    public List<string> Languages { get; set; } = applicationConfigurationService.Languages.Select(l=>l.NativeName).ToList();
    public Prompts Prompts { get;set;} = promptService.Prompts;
}
