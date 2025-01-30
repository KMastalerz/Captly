using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace captly.Core.AsyncCommand;

public abstract class AsyncCommand : IAsyncCommand
{
    private readonly ObservableCollection<Task> runningTasks;
    protected AsyncCommand()
    {
        runningTasks = new ObservableCollection<Task>();
        runningTasks.CollectionChanged += OnRunningTasksChanged;
    }

    private void OnRunningTasksChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CommandManager.InvalidateRequerySuggested();
    }

    public IEnumerable<Task> RunningTasks
    {
        get => runningTasks;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    bool ICommand.CanExecute(object? parameter) => CanExecute(parameter);
    async void ICommand.Execute(object? parameter)
    {
        Task runninTask = ExecuteAsync(parameter);

        runningTasks.Add(runninTask);

        try
        {
            await runninTask;
        }
        finally
        {
            runningTasks.Remove(runninTask);
        }
    }
    public abstract bool CanExecute(object? parameter = null);
    public abstract Task ExecuteAsync(object? parameter = null);
}
