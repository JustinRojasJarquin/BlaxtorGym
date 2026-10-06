using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.WPF.Commands;
using BlaxtorGym.src.BlaxtorGym.WPF.Notifications;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;

public class TipoClienteViewModel : INotifyPropertyChanged
{
    private readonly TipoClienteService _tipoClienteService;
    private readonly INotificationService _notificationService;

    private TipoCliente? _tipoClienteSeleccionado;
    private string _nombre = string.Empty;
    private string _descripcion = string.Empty;
    private bool _procesando;

    public TipoClienteViewModel(
        TipoClienteService tipoClienteService,
        INotificationService notificationService)
    {
        _tipoClienteService = tipoClienteService;
        _notificationService = notificationService;

        CrearCommand = new RelayCommand(
            _ => CrearAsync(),
            _ => !Procesando);

        ModificarCommand = new RelayCommand(
            _ => ModificarAsync(),
            _ => !Procesando && TipoClienteSeleccionado is not null);

        ActivarDesactivarCommand = new RelayCommand(
            _ => CambiarEstadoAsync(),
            _ => !Procesando && TipoClienteSeleccionado is not null);

        LimpiarCommand = new RelayCommand(
            _ =>
            {
                LimpiarFormulario();
                return Task.CompletedTask;
            },
            _ => !Procesando);
    }

    public ObservableCollection<TipoCliente> TiposCliente { get; }
        = new();

    public TipoCliente? TipoClienteSeleccionado
    {
        get => _tipoClienteSeleccionado;
        set
        {
            if (_tipoClienteSeleccionado == value)
            {
                return;
            }

            _tipoClienteSeleccionado = value;
            OnPropertyChanged();

            CargarSeleccionadoEnFormulario();

            CommandManager.InvalidateRequerySuggested();
        }
    }

    public string Nombre
    {
        get => _nombre;
        set
        {
            if (_nombre == value)
            {
                return;
            }

            _nombre = value;
            OnPropertyChanged();
        }
    }

    public string Descripcion
    {
        get => _descripcion;
        set
        {
            if (_descripcion == value)
            {
                return;
            }

            _descripcion = value;
            OnPropertyChanged();
        }
    }

    public bool Procesando
    {
        get => _procesando;
        private set
        {
            if (_procesando == value)
            {
                return;
            }

            _procesando = value;
            OnPropertyChanged();

            CommandManager.InvalidateRequerySuggested();
        }
    }

    public ICommand CrearCommand { get; }

    public ICommand ModificarCommand { get; }

    public ICommand ActivarDesactivarCommand { get; }

    public ICommand LimpiarCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task CargarAsync()
    {
        try
        {
            Procesando = true;

            List<TipoCliente> tipos =
                await _tipoClienteService.ObtenerTodosAsync();

            TiposCliente.Clear();

            foreach (TipoCliente tipo in tipos)
            {
                TiposCliente.Add(tipo);
            }
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "No fue posible cargar los tipos de cliente.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task CrearAsync()
    {
        try
        {
            Procesando = true;

            await _tipoClienteService.CrearAsync(
                Nombre,
                Descripcion);

            _notificationService.MostrarExito(
                "Tipo de cliente creado correctamente.");

            LimpiarFormulario();

            await CargarAsync();
        }
        catch (ArgumentException ex)
        {
            _notificationService.MostrarAdvertencia(
                ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _notificationService.MostrarAdvertencia(
                ex.Message);
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "Ocurrió un error al crear el tipo de cliente.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task ModificarAsync()
    {
        if (TipoClienteSeleccionado is null)
        {
            return;
        }

        try
        {
            Procesando = true;

            await _tipoClienteService.ModificarAsync(
                TipoClienteSeleccionado.Id,
                Nombre,
                Descripcion);

            _notificationService.MostrarExito(
                "Tipo de cliente modificado correctamente.");

            LimpiarFormulario();

            await CargarAsync();
        }
        catch (ArgumentException ex)
        {
            _notificationService.MostrarAdvertencia(
                ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _notificationService.MostrarAdvertencia(
                ex.Message);
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "Ocurrió un error al modificar el tipo de cliente.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task CambiarEstadoAsync()
    {
        if (TipoClienteSeleccionado is null)
        {
            return;
        }

        try
        {
            Procesando = true;

            bool nuevoEstado =
                !TipoClienteSeleccionado.Activo;

            await _tipoClienteService.CambiarEstadoAsync(
                TipoClienteSeleccionado.Id,
                nuevoEstado);

            string mensaje = nuevoEstado
                ? "Tipo de cliente activado correctamente."
                : "Tipo de cliente desactivado correctamente.";

            _notificationService.MostrarExito(mensaje);

            LimpiarFormulario();

            await CargarAsync();
        }
        catch (InvalidOperationException ex)
        {
            _notificationService.MostrarAdvertencia(
                ex.Message);
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "Ocurrió un error al cambiar el estado del tipo de cliente.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private void CargarSeleccionadoEnFormulario()
    {
        if (TipoClienteSeleccionado is null)
        {
            return;
        }

        Nombre = TipoClienteSeleccionado.Nombre;
        Descripcion = TipoClienteSeleccionado.Descripcion;
    }

    private void LimpiarFormulario()
    {
        TipoClienteSeleccionado = null;
        Nombre = string.Empty;
        Descripcion = string.Empty;
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? nombrePropiedad = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nombrePropiedad));
    }
}