using captly.Models;
using System.Text;

namespace captly.Interfaces;
public interface ISubtitileService
{
    Task<List<Subtitle>?> ReadJsonSubtitles(string path);
    IEnumerable<Subtitle>? ReadSRTSubtitles(string path, Encoding? encoding = null);
    Task SaveSubtitlesAsJson(IEnumerable<Subtitle> subtitles, string directory, string name, bool apend = false);
    Task SaveSubtitlesAsSRT(IEnumerable<Subtitle> subtitles, string directory, string name, bool apend = false);
}