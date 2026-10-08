using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.WPF.Commands;
using BlaxtorGym.src.BlaxtorGym.WPF.Notifications;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;

public class ClientesViewModel : INotifyPropertyChanged
{
    private readonly ClienteService _clienteService;
    private readonly INotificationService _notificationService;

    private Cliente? _clienteSeleccionado;
    private string _cedula = string.Empty;
    private string _nombreCompleto = string.Empty;
    private string _telefono = string.Empty;
    private string _correoElectronico = string.Empty;
    private DateTime _fechaNacimiento = DateTime.Today;
    private int _tipoClienteId;
    private bool _procesando;

    public ClientesViewModel(
        ClienteService clienteService,
        INotificationService notificationService)
    {
        _clienteService = clienteService;
        _notificationService = notificationService;

        CrearCommand = new RelayCommand(
            _ => CrearAsync(),
            _ => !Procesando);

        ModificarCommand = new RelayCommand(
            _ => ModificarAsync(),
            _ => !Procesando && ClienteSeleccionado is not null);

        ActivarDesactivarCommand = new RelayCommand(
            _ => CambiarEstadoAsync(),
            _ => !Procesando && ClienteSeleccionado is not null);

        LimpiarCommand = new RelayCommand(
            _ =>
            {
                LimpiarFormulario();
                return Task.CompletedTask;
            },
            _ => !Procesando);
    }

    public ObservableCollection<Cliente> Clientes { get; }
        = new();

    public Cliente? ClienteSeleccionado
    {
        get => _clienteSeleccionado;
        set
        {
            if (_clienteSeleccionado == value)
            {
                return;
            }

            _clienteSeleccionado = value;
            OnPropertyChanged();
            CargarSeleccionadoEnFormulario();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public string Cedula
    {
        get => _cedula;
        set
        {
            if (_cedula == value)
            {
                return;
            }

            _cedula = value;
            OnPropertyChanged();
        }
    }

    public string NombreCompleto
    {
        get => _nombreCompleto;
        set
        {
            if (_nombreCompleto == value)
            {
                return;
            }

            _nombreCompleto = value;
            OnPropertyChanged();
        }
    }

    public string Telefono
    {
        get => _telefono;
        set
        {
            if (_telefono == value)
            {
                return;
            }

            _telefono = value;
            OnPropertyChanged();
        }
    }

    public string CorreoElectronico
    {
        get => _correoElectronico;
        set
        {
            if (_correoElectronico == value)
            {
                return;
            }

            _correoElectronico = value;
            OnPropertyChanged();
        }
    }

    public DateTime FechaNacimiento
    {
        get => _fechaNacimiento;
        set
        {
            if (_fechaNacimiento == value)
            {
                return;
            }

            _fechaNacimiento = value;
            OnPropertyChanged();
        }
    }

    public int TipoClienteId
    {
        get => _tipoClienteId;
        set
        {
            if (_tipoClienteId == value)
            {
                return;
            }

            _tipoClienteId = value;
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

            List<Cliente> clientes = await _clienteService.ObtenerTodosAsync();

            Clientes.Clear();

            foreach (Cliente cliente in clientes)
            {
                Clientes.Add(cliente);
            }
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "No fue posible cargar los clientes.");
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

            await _clienteService.CrearAsync(
                Cedula,
                NombreCompleto,
                Telefono,
                CorreoElectronico,
                FechaNacimiento,
                TipoClienteId);

            _notificationService.MostrarExito(
                "Cliente creado correctamente.");

            LimpiarFormulario();
            await CargarAsync();
        }
        catch (ArgumentException ex)
        {
            _notificationService.MostrarAdvertencia(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _notificationService.MostrarAdvertencia(ex.Message);
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "Ocurrió un error al crear el cliente.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task ModificarAsync()
    {
        if (ClienteSeleccionado is null)
        {
            return;
        }

        try
        {
            Procesando = true;

            await _clienteService.ModificarAsync(
                ClienteSeleccionado.Id,
                Cedula,
                NombreCompleto,
                Telefono,
                CorreoElectronico,
                FechaNacimiento,
                TipoClienteId);

            _notificationService.MostrarExito(
                "Cliente modificado correctamente.");

            LimpiarFormulario();
            await CargarAsync();
        }
        catch (ArgumentException ex)
        {
            _notificationService.MostrarAdvertencia(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _notificationService.MostrarAdvertencia(ex.Message);
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "Ocurrió un error al modificar el cliente.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task CambiarEstadoAsync()
    {
        if (ClienteSeleccionado is null)
        {
            return;
        }

        try
        {
            Procesando = true;

            bool nuevoEstado = !ClienteSeleccionado.Activo;

            await _clienteService.CambiarEstadoAsync(
                ClienteSeleccionado.Id,
                nuevoEstado);

            string mensaje = nuevoEstado
                ? "Cliente activado correctamente."
                : "Cliente desactivado correctamente.";

            _notificationService.MostrarExito(mensaje);

            LimpiarFormulario();
            await CargarAsync();
        }
        catch (InvalidOperationException ex)
        {
            _notificationService.MostrarAdvertencia(ex.Message);
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "Ocurrió un error al cambiar el estado del cliente.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private void CargarSeleccionadoEnFormulario()
    {
        if (ClienteSeleccionado is null)
        {
            return;
        }

        Cedula = ClienteSeleccionado.Cedula;
        NombreCompleto = ClienteSeleccionado.NombreCompleto;
        Telefono = ClienteSeleccionado.Telefono;
        CorreoElectronico = ClienteSeleccionado.CorreoElectronico;
        FechaNacimiento = ClienteSeleccionado.FechaNacimiento;
        TipoClienteId = ClienteSeleccionado.TipoClienteId;
    }

    private void LimpiarFormulario()
    {
        ClienteSeleccionado = null;
        Cedula = string.Empty;
        NombreCompleto = string.Empty;
        Telefono = string.Empty;
        CorreoElectronico = string.Empty;
        FechaNacimiento = DateTime.Today;
        TipoClienteId = 0;
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? nombrePropiedad = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nombrePropiedad));
    }
}
