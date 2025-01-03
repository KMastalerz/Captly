using captly.Enums;
using captly.Exceptions;
using captly.Extensions;
using captly.Interfaces;
using captly.Model;
using Newtonsoft.Json;
using OpenAI.Chat;
using System.IO;

namespace captly.Services;
internal class SubtitleTranslationService(IApplicationCacheStateService applicationCacheStateService, IApplicationConfigurationService applicationConfigurationService) : ISubtitleTranslationService
{
    public async Task StartTranslation(SubtitlesView subtitlesView)
    {
        try
        {
            var subtitlesCache = applicationCacheStateService.OpenedSubtitles.FirstOrDefault(s => s.Name == subtitlesView.Name && s.Language == subtitlesView.Language);

            // check state of translation 
            var initialID = subtitlesCache?.LastTranslatedID ?? 0;

            // get setup for translation from cache
            var streamReader = new StreamReader(subtitlesView.Path, subtitlesView.Encoding);
            var subtitles = streamReader.SrtToList();

            var subtitlesLength = subtitles.Count;

            // remove id's already translated 
            subtitles = subtitles.Where(s => s.Index > initialID).ToList();

            // set state to translating
            await subtitlesView.UpdateStatus(TranslationStatus.Translating);

            // start | re-start translation clock
            _ = subtitlesView.StartElapsedTimeTracking();

            // read current file config
            var subtitlesConfig = subtitlesView.SubtitlesSetup ?? applicationConfigurationService.ApplicationConfiguration.SubtitlesSetup;
            var language = string.IsNullOrEmpty(subtitlesView.Language) ? applicationConfigurationService.ApplicationConfiguration.DefaultLanguage : subtitlesView.Language;

            // create new communication chat and history
            var client = new ChatClient(
                model: applicationConfigurationService.ApplicationConfiguration.OpenAiApiModel, 
                apiKey: applicationConfigurationService.ApplicationConfiguration.OpenAiApiKey);
            var conversationHistory = new List<ChatMessage>();

            // loop through each subtitle with step 15 to request for translation, bit by bit.
            const int chunkSize = 15;

            // read translation prompt
            var translationPrompt = applicationConfigurationService.TranslationPrompt.Replace("{@Language}", language);

            for (int i = 0; i < subtitles.Count(); i += chunkSize)
            {
                var generatedSubtitles = new List<Subtitle>();
                // after each request, save progress to cache
                var jsonToTranslate = JsonConvert.SerializeObject(subtitles.Skip(i).Take(chunkSize));

                conversationHistory.Add(new UserChatMessage($"{translationPrompt}\n{jsonToTranslate}"));

                bool success = false;
                int retryCount = 0;

                while (!success && retryCount < 5) // Retry logic for each chunk
                {
                    await subtitlesView.UpdateRequestCount();

                    ChatCompletion completion = await client.CompleteChatAsync(conversationHistory.TakeLast(3), cancellationToken: subtitlesView.TranslationToken);

                    await subtitlesView.UpdateInputTokenCount(subtitlesView.InputTokenCount += completion.Usage.InputTokenCount);
                    await subtitlesView.UpdateOutputTokenCount(subtitlesView.OutputTokenCount += completion.Usage.OutputTokenCount);

                    var response = completion.Content[0].Text.ExtrapolateJson() ??
                        throw new EmptyJsonException();

                    response = response.Trim();

                    conversationHistory.Add(new AssistantChatMessage(response));

                    generatedSubtitles = JsonConvert.DeserializeObject<List<Subtitle>>(response);
                }

                if (success)
                {
                    var currentMaxID = generatedSubtitles!.Select(s => s.Index).Max()!;
                    var progress = (double)currentMaxID / subtitlesLength * 100;
                    await subtitlesView.UpdateProgress(progress);

                    subtitlesView.LastTranslatedID = currentMaxID; // Update last translated ID, so that progress can be cached


                    // save translated file based on subtitlesConfig



                    // update cache
                }
                else
                {
                    // update cache 
                }
            }



            // if paused, stop translation clock and set state to paused

            // if error, stop translation clock, set state to error and throw translation error

            // if completed, stop translation clock, set state to completed and save translation to cache
        }
        catch(Exception ex) 
        {
            var ext = ex;
        }

    }

    public async Task PauseTranslation()
    {
        // cancel token set to cancell request
    }
}
