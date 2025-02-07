namespace captly.Models;
public class WhisperOption
{
    public string Option { get; set; } = default!;
    public string Original { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Description { get; set; } = default!;
    public object? Value { get; set; } = null;
    public object? DefaultValue { get; set; } = null;
    public bool IsChecked { get; set; } = false;
}

public class OptionDependency
{
    public string Option { get; set; } = default!;
    public object? DefaultValue { get; set; } = null;
}

public class OptionActivation
{
    public string Option { get; set; } = default!;
    public object? DefaultValue { get; set; } = null;
}