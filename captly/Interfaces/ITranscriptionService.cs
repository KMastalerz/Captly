using captly.Models;
using captly.ViewModels;

namespace captly.Services;
public interface ITranscriptionService
{
    Task TranscribeFile(TranscriptionViewModel transcriptionViewModel, CancellationToken cancellationToken);
    TranscriptionSetup GetGlobalTranscriptionSetup();
}