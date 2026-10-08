using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.WPF.Commands;
using BlaxtorGym.src.BlaxtorGym.WPF.Notifications;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;

public class TipoMembresiaViewModel : INotifyPropertyChanged
{
    private readonly TipoMembresiaService _tipoMembresiaService;
    private readonly INotificationService _notificationService;

    private TipoMembresia? _tipoMembresiaSeleccionado;

    private string _nombre = string.Empty;
    private string _duracionDias = string.Empty;
    private string _descripcion = string.Empty;

    private bool _procesando;

    public TipoMembresiaViewModel(
        TipoMembresiaService tipoMembresiaService,
        INotificationService notificationService)
    {
        _tipoMembresiaService = tipoMembresiaService;
        _notificationService = notificationService;

        CrearCommand = new RelayCommand(
            _ => CrearAsync(),
            _ => !Procesando);

        ModificarCommand = new RelayCommand(
            _ => ModificarAsync(),
            _ =>
                !Procesando &&
                TipoMembresiaSeleccionado is not null);

        ActivarDesactivarCommand = new RelayCommand(
            _ => CambiarEstadoAsync(),
            _ =>
                !Procesando &&
                TipoMembresiaSeleccionado is not null);

        LimpiarCommand = new RelayCommand(
            _ =>
            {
                LimpiarFormulario();
                return Task.CompletedTask;
            },
            _ => !Procesando);
    }

    public ObservableCollection<TipoMembresia> TiposMembresia { get; }
        = new();

    public TipoMembresia? TipoMembresiaSeleccionado
    {
        get => _tipoMembresiaSeleccionado;
        set
        {
            if (_tipoMembresiaSeleccionado == value)
            {
                return;
            }

            _tipoMembresiaSeleccionado = value;

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

    public string DuracionDias
    {
        get => _duracionDias;
        set
        {
            if (_duracionDias == value)
            {
                return;
            }

            _duracionDias = value;
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

            List<TipoMembresia> tipos =
                await _tipoMembresiaService.ObtenerTodosAsync();

            TiposMembresia.Clear();

            foreach (TipoMembresia tipo in tipos)
            {
                TiposMembresia.Add(tipo);
            }
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "No fue posible cargar los tipos de membresía.");
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

            int duracionDias =
                ObtenerDuracionDias();

            await _tipoMembresiaService.CrearAsync(
                Nombre,
                duracionDias,
                Descripcion);

            _notificationService.MostrarExito(
                "Tipo de membresía creado correctamente.");

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
                "Ocurrió un error al crear el tipo de membresía.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task ModificarAsync()
    {
        if (TipoMembresiaSeleccionado is null)
        {
            return;
        }

        try
        {
            Procesando = true;

            int duracionDias =
                ObtenerDuracionDias();

            await _tipoMembresiaService.ModificarAsync(
                TipoMembresiaSeleccionado.Id,
                Nombre,
                duracionDias,
                Descripcion);

            _notificationService.MostrarExito(
                "Tipo de membresía modificado correctamente.");

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
                "Ocurrió un error al modificar el tipo de membresía.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task CambiarEstadoAsync()
    {
        if (TipoMembresiaSeleccionado is null)
        {
            return;
        }

        try
        {
            Procesando = true;

            bool nuevoEstado =
                !TipoMembresiaSeleccionado.Activo;

            await _tipoMembresiaService.CambiarEstadoAsync(
                TipoMembresiaSeleccionado.Id,
                nuevoEstado);

            string mensaje = nuevoEstado
                ? "Tipo de membresía activado correctamente."
                : "Tipo de membresía desactivado correctamente.";

            _notificationService.MostrarExito(
                mensaje);

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
                "Ocurrió un error al cambiar el estado del tipo de membresía.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private int ObtenerDuracionDias()
    {
        if (!int.TryParse(
            DuracionDias,
            out int duracionDias))
        {
            throw new ArgumentException(
                "La duración debe ser un número entero de días.");
        }

        return duracionDias;
    }

    private void CargarSeleccionadoEnFormulario()
    {
        if (TipoMembresiaSeleccionado is null)
        {
            return;
        }

        Nombre =
            TipoMembresiaSeleccionado.Nombre;

        DuracionDias =
            TipoMembresiaSeleccionado.DuracionDias.ToString();

        Descripcion =
            TipoMembresiaSeleccionado.Descripcion;
    }

    private void LimpiarFormulario()
    {
        TipoMembresiaSeleccionado = null;

        Nombre = string.Empty;
        DuracionDias = string.Empty;
        Descripcion = string.Empty;
    }

    private void OnPropertyChanged(
        [CallerMemberName]
        string? nombrePropiedad = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                nombrePropiedad));
    }
}