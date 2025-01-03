using captly.Interfaces;
using captly.Model;

namespace captly.Services;
internal class TranslateStateService : ITranslateStateService
{
    public SubtitlesView? SelectedSubtitle { get; set; } = null;
}
