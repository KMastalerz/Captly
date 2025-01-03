using captly.Model;

namespace captly.Interfaces;
public interface ITranslateStateService
{
    SubtitlesView? SelectedSubtitle { get; set; }
}