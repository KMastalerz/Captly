namespace captly.Model;
public class Subtitle
{
    public int Index { get; set; }
    public string Text { get; set; } = default!;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
