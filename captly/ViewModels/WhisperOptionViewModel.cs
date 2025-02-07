using captly.Core;

namespace captly.ViewModels;
public class WhisperOptionViewModel: BaseViewModel
{
    private bool isChecked;
    private object? _value = null;

    public string Option { get; set; } = default!;
    public string Original { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Description { get; set; } = default!;
    public object? DefaultValue { get; set; } = null;
    public bool IsChecked
    {
        get => isChecked;
        set
        {
            SetProperty(ref isChecked, value);
        }
    }
    public object? Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
}
