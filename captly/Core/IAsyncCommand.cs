using System.Windows.Input;

namespace captly.Core;
internal interface IAsyncCommand : ICommand
{
    IEnumerable<Task> RunningTasks { get; }
    new bool CanExecute(object? parameter = null);
    Task ExecuteAsync(object? parameter = null);
}
