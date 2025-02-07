using captly.Constants;
using captly.Exceptions;
using captly.Extensions;
using captly.Interfaces;
using captly.Models;
using captly.ViewModels;
using Newtonsoft.Json;
using OllamaSharp;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
namespace captly.Services;

public class TranslationService(IAppConfigurationService appConfigurationService, ISubtitileService subtitileService) : ITranslationService
{
    // Public method to translate a file based on the provided TranslationViewModel.
    public async Task TranslateFile(TranslationViewModel translationViewModel, CancellationToken cancellationToken)
    {
        try
        {
            // Check if the translation setup is configured to use Ollama.
            if (translationViewModel.TranslationSetup.RunOllama)
            {
                // Execute the translation process using Ollama.
                await TranslateWithOllama(translationViewModel, cancellationToken);
            }
            else
            {
                // Placeholder for alternative translation methods (if any).
                await TranslateWithOpenAiApi(translationViewModel, cancellationToken);
            }
        }
        // If the operation was cancelled, update the status to Paused.
        catch (CancelledException)
        {
            translationViewModel.Status = TranslationStatus.Paused;
        }
        // For any other exception, update the status to Error.
        catch
        {
            translationViewModel.Status = TranslationStatus.Error;
        }
    }

    // Returns the global translation setup from the application configuration.
    public TranslationSetup GetGlobalTranslationSetup()
    {
        return appConfigurationService.ApplicationConfiguration.TranslationSetup;
    }

    #region [Ollama Translation]
    // Private method that handles translation using the Ollama API.
    private async Task TranslateWithOllama(TranslationViewModel translationViewModel, CancellationToken cancellationToken)
    {
        // Retrieve the base directory of the application.
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        // Build the temporary directory path (first using "Temp", then switching to "Translations").
        string tempDirectory = Path.Combine(appDirectory, "Temp");
        tempDirectory = Path.Combine(appDirectory, "Translations");
        // Define the subtitle file name (with .srt extension) based on the view model's name.
        var subtitleName = $"{translationViewModel.Name}.srt";
        // Build the full temporary path for the subtitle file.
        var tempPath = Path.Combine(tempDirectory, subtitleName);

        // Set up the URI for the local Ollama API endpoint.
        var uri = new Uri("http://localhost:11434");
        // Create a new Ollama API client using the specified URI.
        var ollama = new OllamaApiClient(uri);
        // Retrieve a list of local models available in Ollama.
        var models = await ollama.ListLocalModelsAsync();

        // Check if the desired model (specified in TranslationSetup) exists in the local models.
        if (models.Any(m => m.Name == translationViewModel.TranslationSetup.ModelName))
        {
            // If available, set the selected model accordingly.
            ollama.SelectedModel = translationViewModel.TranslationSetup.ModelName;
        }
        else
        {
            // Otherwise, pull the model asynchronously.
            await foreach (var status in ollama.PullModelAsync(translationViewModel.TranslationSetup.ModelName)) ;
            // After pulling, set the selected model.
            ollama.SelectedModel = translationViewModel.TranslationSetup.ModelName;
        }

        // Build the path for the original subtitle file.
        var subtitleDirectory = translationViewModel.Path;
        var subtitlePath = Path.Combine(subtitleDirectory, translationViewModel.Name);
        // Append the file extension to complete the file path.
        subtitlePath += translationViewModel.Extension;

        // Read the SRT subtitles from the original file; if null, create a new empty list.
        var subtitles = subtitileService.ReadSRTSubtitles(subtitlePath)?.ToList() ?? new();

        // Initialize the start index for translation.
        var startIndex = 0;
        // If translation is not starting fresh, determine how many subtitles have already been processed.
        if (translationViewModel.Status != TranslationStatus.New)
        {
            startIndex = subtitileService.ReadSRTSubtitles(tempPath)?.Count() ?? 0;
        }

        // Update the UI to reflect that translation is in progress.
        App.Current.Dispatcher.Invoke(() =>
        {
            translationViewModel.Status = TranslationStatus.Translating;
        });

        // Get the total count of subtitles.
        var subtitlesLength = subtitles.Count;

        // Retrieve the base translation prompt from the configuration.
        var basePrompt = await appConfigurationService.ReadTranslationPrompt() ?? string.Empty;
        basePrompt = basePrompt.Replace("@Language", $"{translationViewModel.TranslationSetup.DefaultLanguage}");

        // Determine the batch size for each translation request; default to 15 if not set.
        int batchSize = translationViewModel.TranslationSetup.SubtitleRequestCount > 0
                       ? translationViewModel.TranslationSetup.SubtitleRequestCount
                       : 15;
        // Initialize a retry counter.
        var retry = 0;

        // Loop over the subtitles in batches.
        for (int i = startIndex; i < subtitlesLength; i = i + batchSize)
        {
            // Throw an exception if cancellation is requested.
            cancellationToken.ThrowIfCancellationRequested();

            // Select the current batch of subtitles to translate.
            var currentBatch = subtitles.Skip(i).Take(15).ToList();

            // Serialize the current batch of subtitles to JSON.
            string batchJson = JsonConvert.SerializeObject(currentBatch);

            // Combine the base prompt and the serialized batch into one prompt.
            string prompt = $"{basePrompt}\n{batchJson}";

            // Send the prompt to Ollama and await the response.
            string response = await SendOllamaMessageAsync(prompt, ollama, cancellationToken);

            // Extract and trim the JSON portion from the response.
            response = response.ExtrapolateJson().Trim();

            try
            {
                // Attempt to deserialize the response into a list of Subtitle objects.
                var tempSubtitle = JsonConvert.DeserializeObject<List<Subtitle>>(response) ??
                    throw new Exception();

                // Increment the count of successful requests.
                translationViewModel.RequestCount++;

                // Save the translated subtitles to the temporary SRT file, appending to existing content.
                await subtitileService.SaveSubtitlesAsSRT(tempSubtitle, tempDirectory, translationViewModel.Name, true);
            }
            catch (Exception ex)
            {
                // Increment the retry counter if an exception occurs.
                retry++;

                if (retry > 3)
                {
                    // After exceeding the retry limit, increment the failed request count,
                    // adjust the index to retry the current batch, and continue the loop.
                    translationViewModel.FailedRequestCount++;
                    i = i - batchSize;
                    continue;
                }
                // If within retry limits, rethrow the exception.
                throw new Exception(ex.Message, ex);
            }

            // Update the progress percentage in the view model.
            translationViewModel.Progress = (i + batchSize) / subtitlesLength * 100;
        }

        // TODO: Add moving from temp to correct folder logic
        // Build the final output path for the subtitle file.
        var outputPath = Path.Combine(subtitleDirectory, subtitleName);

        // 1. Archive the old file if the translation setup requires archiving.
        if (translationViewModel.TranslationSetup.ArchiveOldFile)
        {
            if (!string.IsNullOrEmpty(translationViewModel.TranslationSetup.OldVersionPath))
            {
                // If an OldVersionPath is specified, determine its directory.
                var destinationDirectory = Path.GetDirectoryName(translationViewModel.TranslationSetup.OldVersionPath)!;
                var destinationPath = Path.Combine(destinationDirectory, subtitleName);

                if (File.Exists(outputPath))
                {
                    // Create the destination directory if it doesn't exist.
                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }
                    // Move the existing file to the archive location, overwriting if necessary.
                    File.Move(outputPath, destinationPath, overwrite: true);
                }
            }
            else
            {
                // If no specific archive path is provided, use an "Archive" subfolder in the subtitle directory.
                var destinationDirectory = Path.Combine(subtitleDirectory, "Archive");
                var destinationPath = Path.Combine(destinationDirectory, subtitleName);

                if (File.Exists(outputPath))
                {
                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }
                    File.Move(outputPath, destinationPath, overwrite: true);
                }
            }
        }

        // 2. Move the translated file from the temporary directory to the final output location.
        if (translationViewModel.TranslationSetup.OverriteFile)
        {
            // Overwrite any existing file at the output location.
            File.Move(tempPath, outputPath, overwrite: true);
        }
        else if (!string.IsNullOrEmpty(translationViewModel.TranslationSetup.NewVersionPath))
        {
            // If OverriteFile is false and a NewVersionPath is provided, move the file to that location.
            var destinationDirectory = Path.GetDirectoryName(translationViewModel.TranslationSetup.NewVersionPath)!;
            var destinationPath = Path.Combine(destinationDirectory, subtitleName);

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            File.Move(tempPath, destinationPath, overwrite: true);
        }
        // If neither OverriteFile nor NewVersionPath is specified, the temporary file remains in the temp directory.

        // Update the status to Finished once the process is complete.
        translationViewModel.Status = TranslationStatus.Finished;
    }

    // Sends a prompt to Ollama and accumulates the response.
    private async Task<string> SendOllamaMessageAsync(string prompt, OllamaApiClient ollama, CancellationToken cancellationToken)
    {
        var response = "";

        // TODO: throw on cancellation token.
        try
        {
            // Generate the response asynchronously from Ollama.
            await foreach (var stream in ollama.GenerateAsync(prompt, cancellationToken: cancellationToken))
            {
                // If cancellation is requested, throw a CancelledException.
                if (cancellationToken.IsCancellationRequested)
                    throw new CancelledException();

                // Append the current stream's response to the overall response.
                response += stream!.Response;
            }
        }
        catch
        {
            // On any exception, throw a CancelledException.
            throw new CancelledException();
        }

        // Clean the response (e.g., remove any unwanted tags) before returning.
        return CleanOllamaResponse(response);
    }

    // Cleans the Ollama response by removing content within <think>...</think> tags.
    private string CleanOllamaResponse(string response)
    {
        string cleaned = Regex.Replace(response, @"<think>.*?</think>", "", RegexOptions.Singleline);
        return cleaned.Trim();
    }
    #endregion

    #region [OpenAI API Translation]
    // Placeholder method for translation using the OpenAI API.
    private async Task TranslateWithOpenAiApi(TranslationViewModel translationViewModel, CancellationToken cancellationToken)
    {
        // Determine application directories.
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string tempDirectory = Path.Combine(appDirectory, "Temp");
        tempDirectory = Path.Combine(appDirectory, "Translations");
        var subtitleName = $"{translationViewModel.Name}.srt";
        var tempPath = Path.Combine(tempDirectory, subtitleName);

        // Build the path to the original subtitle file.
        var subtitleDirectory = translationViewModel.Path;
        var subtitlePath = Path.Combine(subtitleDirectory, translationViewModel.Name) + translationViewModel.Extension;

        // Read the original SRT subtitles.
        var subtitles = subtitileService.ReadSRTSubtitles(subtitlePath)?.ToList() ?? new();

        // Determine the start index (for resuming translation if needed).
        int startIndex = 0;
        if (translationViewModel.Status != TranslationStatus.New)
        {
            startIndex = subtitileService.ReadSRTSubtitles(tempPath)?.Count() ?? 0;
        }

        // Set UI status to "translating".
        App.Current.Dispatcher.Invoke(() =>
        {
            translationViewModel.Status = TranslationStatus.Translating;
        });

        int subtitlesLength = subtitles.Count;
        // Read the base translation prompt from configuration.
        var basePrompt = await appConfigurationService.ReadTranslationPrompt();
        // Determine the batch size (defaulting to 15 if not set).
        int batchSize = translationViewModel.TranslationSetup.SubtitleRequestCount > 0
                       ? translationViewModel.TranslationSetup.SubtitleRequestCount
                       : 15;
        int retry = 0;

        // Loop over the subtitles in batches.
        for (int i = startIndex; i < subtitlesLength; i += batchSize)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Select a batch of subtitles.
            var currentBatch = subtitles.Skip(i).Take(batchSize).ToList();

            // Serialize the current batch to JSON.
            string batchJson = JsonConvert.SerializeObject(currentBatch);

            // Build the prompt combining the base prompt with the serialized subtitles.
            string prompt = $"{basePrompt}\n{batchJson}";

            // Send the prompt to OpenAI's API and await the response.
            string response = await SendOpenAiMessageAsync(prompt, translationViewModel, cancellationToken);
            // Extract the JSON part from the response.
            response = response.ExtrapolateJson().Trim();

            try
            {
                // Attempt to deserialize the response into a list of Subtitle objects.
                var tempSubtitle = JsonConvert.DeserializeObject<List<Subtitle>>(response)
                                   ?? throw new Exception("Failed to deserialize OpenAI response.");

                translationViewModel.RequestCount++;

                // Append the translated subtitles to the temporary SRT file.
                await subtitileService.SaveSubtitlesAsSRT(tempSubtitle, tempDirectory, translationViewModel.Name, true);
            }
            catch (Exception ex)
            {
                retry++;

                if (retry > translationViewModel.TranslationSetup.RetryCount)
                {
                    translationViewModel.FailedRequestCount++;
                    i = i - batchSize;
                    continue;
                }
                throw new Exception(ex.Message, ex);
            }

            translationViewModel.Progress = (i + batchSize) / subtitlesLength * 100;
        }

        // After all batches are processed, move the temporary file to the final destination.
        var outputPath = Path.Combine(subtitleDirectory, subtitleName);

        // Archive the old file if required.
        if (translationViewModel.TranslationSetup.ArchiveOldFile)
        {
            if (!string.IsNullOrEmpty(translationViewModel.TranslationSetup.OldVersionPath))
            {
                var destinationDirectory = Path.GetDirectoryName(translationViewModel.TranslationSetup.OldVersionPath)!;
                var destinationPath = Path.Combine(destinationDirectory, subtitleName);

                if (File.Exists(outputPath))
                {
                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }
                    File.Move(outputPath, destinationPath, overwrite: true);
                }
            }
            else
            {
                var destinationDirectory = Path.Combine(subtitleDirectory, "Archive");
                var destinationPath = Path.Combine(destinationDirectory, subtitleName);

                if (File.Exists(outputPath))
                {
                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }
                    File.Move(outputPath, destinationPath, overwrite: true);
                }
            }
        }

        // Move the temporary file to the final location based on the overwrite settings.
        if (translationViewModel.TranslationSetup.OverriteFile)
        {
            File.Move(tempPath, outputPath, overwrite: true);
        }
        else if (!string.IsNullOrEmpty(translationViewModel.TranslationSetup.NewVersionPath))
        {
            var destinationDirectory = Path.GetDirectoryName(translationViewModel.TranslationSetup.NewVersionPath)!;
            var destinationPath = Path.Combine(destinationDirectory, subtitleName);

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            File.Move(tempPath, destinationPath, overwrite: true);
        }

        translationViewModel.Status = TranslationStatus.Finished;
    }

    private async Task<string> SendOpenAiMessageAsync(string prompt, TranslationViewModel translationViewModel, CancellationToken cancellationToken)
    {
        // Retrieve the API key and model name from the translation setup.
        // (Ensure you have added an ApiKey property to your TranslationSetup if it doesn't exist yet.)
        string apiKey = translationViewModel.TranslationSetup.OpenAiApiKey; // <-- Make sure this property exists.
        string model = translationViewModel.TranslationSetup.ModelName;

        using (HttpClient httpClient = new HttpClient())
        {
            // Add the authorization header with the API key.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            // Create the request payload for the OpenAI completions endpoint.
            var requestData = new
            {
                model = model,
                prompt = prompt,
                temperature = 0.7,
                max_tokens = 2048
            };

            string jsonRequest = JsonConvert.SerializeObject(requestData);
            using (var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json"))
            {
                // Send the POST request.
                HttpResponseMessage response = await httpClient.PostAsync("https://api.openai.com/v1/completions", content, cancellationToken);
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
        }
    }
    #endregion
}
