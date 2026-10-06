using System.Windows.Input;

namespace BlaxtorGym.src.BlaxtorGym.WPF.Commands;

public class RelayCommand : ICommand
{
    private readonly Func<object?, Task> _ejecutar;
    private readonly Predicate<object?>? _puedeEjecutar;
    private bool _ejecutando;

    public RelayCommand(
        Func<object?, Task> ejecutar,
        Predicate<object?>? puedeEjecutar = null)
    {
        _ejecutar = ejecutar;
        _puedeEjecutar = puedeEjecutar;
    }

    public bool CanExecute(object? parameter)
    {
        return !_ejecutando &&
               (_puedeEjecutar?.Invoke(parameter) ?? true);
    }

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
        {
            return;
        }

        try
        {
            _ejecutando = true;
            CommandManager.InvalidateRequerySuggested();

            await _ejecutar(parameter);
        }
        finally
        {
            _ejecutando = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}