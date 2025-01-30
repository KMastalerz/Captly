using System.Windows.Input;

namespace captly.Core.AsyncCommand;

public interface IAsyncCommand : ICommand
{
    IEnumerable<Task> RunningTasks { get; }

    event EventHandler? CanExecuteChanged;

    new bool CanExecute(object? parameter = null);
    Task ExecuteAsync(object? parameter = null);
}
