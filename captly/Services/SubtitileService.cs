using captly.Models;
using System.IO;

namespace captly.Services;

public class SubtitileService
{
    public async Task<IEnumerable<Subtitle>?> ReadJsonSubtitles(string path)
    {
        if (!Directory.Exists(path))
        {
            return null;
        }

        var jsonSubtitles = await File.ReadAllTextAsync(path);
        return null; 
    }

    public async Task<IEnumerable<Subtitle>?> ReadSRTSubtitles(string path)
    {
        if (!Directory.Exists(path))
        {
            return null;
        }

        var srtSubtitles = await File.ReadAllTextAsync(path);
        return null;
    }

    public async Task SaveSubtitlesAsJson(IEnumerable<Subtitle> subtitles)
    {
    }

    public async Task SaveSubtitlesAsSRT(IEnumerable<Subtitle> subtitles)
    {
    }
}
