using captly.Interfaces;
using captly.Model;

namespace captly.Services;
internal class SubtitleTranslationService(IApplicationCacheStateService applicationCacheStateService)
{
    public async Task StartTranslation(SubtitlesView subtitlesView)
    {
        var subtitleCache = applicationCacheStateService.OpenedSubtitles.FirstOrDefault(s => s.Name == subtitlesView.Name && s.Language == subtitlesView.Language);
        // check state of translation 
        

        // get setup for translation from cache

        // read current file cache

        // set state to translating

        // start | re-start translation clock

        // stream file 

        // exclude already translated ID's if exist and setup is set to continue from last translation

        // loop through each subtitle with step 15 to request for translation, bit by bit.

        // after each request, save progress to cache

        // if paused, stop translation clock and set state to paused

        // if error, stop translation clock, set state to error and throw translation error

        // if completed, stop translation clock, set state to completed and save translation to cache
    }

    public async Task PauseTranslation()
    {
        // cancel token set to cancell request
    }
}
