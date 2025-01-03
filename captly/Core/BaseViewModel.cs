using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace captly.Core;
public class BaseViewModel : INotifyPropertyChanged
{
    // The PropertyChanged event is raised whenever a property is changed
    public event PropertyChangedEventHandler? PropertyChanged;

    // This method is called to raise the PropertyChanged event
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // This method sets the value of a property and notifies the UI if it changes
    protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!Equals(backingField, value))
        {
            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        return false;
    }

    // Generic method to update a property on any underlying model and notify the UI if it changes
    protected bool SetSubProperty<TModel, TProperty>(TModel model, Func<TModel, TProperty> propertyGetter, Action<TModel, TProperty> propertySetter, TProperty value, [CallerMemberName] string? propertyName = null)
    {
        if (!Equals(propertyGetter(model), value))
        {
            propertySetter(model, value);
            OnPropertyChanged(propertyName);
            return true;
        }
        return false;
    }
}
