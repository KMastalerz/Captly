using captly.Extensions;
using captly.Interfaces;
using captly.Models;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace captly.Services;

public class SubtitileService : ISubtitileService
{
    public async Task<List<Subtitle>?> ReadJsonSubtitles(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        var jsonSubtitles = await File.ReadAllTextAsync(path);
        var json = JsonConvert.DeserializeObject<List<Subtitle>>(jsonSubtitles);
        return json;
    }

    public IEnumerable<Subtitle>? ReadSRTSubtitles(string path, Encoding? encoding = null)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        if (encoding == null)
        {
            encoding = Encoding.UTF8;
        }

        var streamReader = new StreamReader(path, encoding);
        var subtitles = streamReader.SrtToList();

        return subtitles;
    }

    public async Task SaveSubtitlesAsJson(IEnumerable<Subtitle> subtitles, string directory, string name, bool apend = false)
    {
        if (!name.EndsWith(".json"))
        {
            name = $"{name}.json";
        }

        var tempPath = Path.Combine(directory, name);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (apend)
        {
            var currentList = await ReadJsonSubtitles(tempPath) ?? new();
            currentList.AddRange(subtitles);
            var text = JsonConvert.SerializeObject(currentList);
            await File.WriteAllTextAsync(tempPath, text);
        }
        else
        {
            var text = JsonConvert.SerializeObject(subtitles);
            await File.WriteAllTextAsync(tempPath, text);
        }
    }

    public async Task SaveSubtitlesAsSRT(IEnumerable<Subtitle> subtitles, string directory, string name, bool apend = false)
    {
        if (!name.EndsWith(".srt"))
        {
            name = $"{name}.srt";
        }

        var tempPath = Path.Combine(directory, name);

        var text = subtitles.ParseSubtitlesToSrt();

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (apend)
        {
            await File.AppendAllTextAsync(tempPath, text);
        }
        else
        {
            await File.WriteAllTextAsync(tempPath, text);
        }
    }
}
