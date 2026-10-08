using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.WPF.Commands;
using BlaxtorGym.src.BlaxtorGym.WPF.Notifications;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;

public class MembresiasViewModel : INotifyPropertyChanged
{
    private readonly MembresiaService _membresiaService;
    private readonly ClienteService _clienteService;
    private readonly TipoMembresiaService _tipoMembresiaService;
    private readonly INotificationService _notificationService;

    private Membresia? _membresiaSeleccionada;
    private Cliente? _clienteSeleccionado;
    private TipoMembresia? _tipoMembresiaSeleccionado;

    private DateTime? _fechaInicio = DateTime.Today;
    private bool _procesando;

    public MembresiasViewModel(
        MembresiaService membresiaService,
        ClienteService clienteService,
        TipoMembresiaService tipoMembresiaService,
        INotificationService notificationService)
    {
        _membresiaService = membresiaService;
        _clienteService = clienteService;
        _tipoMembresiaService = tipoMembresiaService;
        _notificationService = notificationService;

        CrearCommand = new RelayCommand(
            _ => CrearAsync(),
            _ => !Procesando);

        RenovarCommand = new RelayCommand(
            _ => RenovarAsync(),
            _ =>
                !Procesando &&
                MembresiaSeleccionada is not null);

        DesactivarCommand = new RelayCommand(
            _ => DesactivarAsync(),
            _ =>
                !Procesando &&
                MembresiaSeleccionada is not null &&
                MembresiaSeleccionada.Activa);

        LimpiarCommand = new RelayCommand(
            _ =>
            {
                LimpiarFormulario();
                return Task.CompletedTask;
            },
            _ => !Procesando);
    }

    public ObservableCollection<Membresia> Membresias { get; }
        = new();

    public ObservableCollection<Cliente> Clientes { get; }
        = new();

    public ObservableCollection<TipoMembresia> TiposMembresia { get; }
        = new();

    public Membresia? MembresiaSeleccionada
    {
        get => _membresiaSeleccionada;
        set
        {
            if (_membresiaSeleccionada == value)
            {
                return;
            }

            _membresiaSeleccionada = value;

            OnPropertyChanged();

            CargarSeleccionadaEnFormulario();

            CommandManager.InvalidateRequerySuggested();
        }
    }

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
        }
    }

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
        }
    }

    public DateTime? FechaInicio
    {
        get => _fechaInicio;
        set
        {
            if (_fechaInicio == value)
            {
                return;
            }

            _fechaInicio = value;
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

    public ICommand RenovarCommand { get; }

    public ICommand DesactivarCommand { get; }

    public ICommand LimpiarCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task CargarAsync()
    {
        try
        {
            Procesando = true;

            await CargarClientesAsync();
            await CargarTiposMembresiaAsync();
            await CargarMembresiasAsync();
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "No fue posible cargar la información de membresías.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task CargarClientesAsync()
    {
        List<Cliente> clientes =
            await _clienteService.ObtenerActivosAsync();

        Clientes.Clear();

        foreach (Cliente cliente in clientes)
        {
            Clientes.Add(cliente);
        }
    }

    private async Task CargarTiposMembresiaAsync()
    {
        List<TipoMembresia> tipos =
            await _tipoMembresiaService.ObtenerTodosAsync();

        TiposMembresia.Clear();

        foreach (TipoMembresia tipo in tipos)
        {
            if (tipo.Activo)
            {
                TiposMembresia.Add(tipo);
            }
        }
    }

    private async Task CargarMembresiasAsync()
    {
        List<Membresia> membresias =
            await _membresiaService.ObtenerTodasAsync();

        Membresias.Clear();

        foreach (Membresia membresia in membresias)
        {
            Membresias.Add(membresia);
        }
    }

    private async Task CrearAsync()
    {
        try
        {
            Procesando = true;

            if (!FechaInicio.HasValue)
            {
                throw new ArgumentException(
                    "Debe seleccionar una fecha de inicio.");
            }

            int clienteId =
                ClienteSeleccionado?.Id ?? 0;

            int tipoMembresiaId =
                TipoMembresiaSeleccionado?.Id ?? 0;

            await _membresiaService.CrearAsync(
                clienteId,
                tipoMembresiaId,
                FechaInicio.Value);

            _notificationService.MostrarExito(
                "Membresía creada correctamente.");

            LimpiarFormulario();

            await CargarMembresiasAsync();
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
                "Ocurrió un error al crear la membresía.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task RenovarAsync()
    {
        if (MembresiaSeleccionada is null)
        {
            return;
        }

        try
        {
            Procesando = true;

            int tipoMembresiaId =
                TipoMembresiaSeleccionado?.Id ?? 0;

            if (tipoMembresiaId <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar un tipo de membresía.");
            }

            await _membresiaService.RenovarAsync(
                MembresiaSeleccionada.Id,
                tipoMembresiaId);

            _notificationService.MostrarExito(
                "Membresía renovada correctamente.");

            LimpiarFormulario();

            await CargarMembresiasAsync();
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
                "Ocurrió un error al renovar la membresía.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task DesactivarAsync()
    {
        if (MembresiaSeleccionada is null)
        {
            return;
        }

        try
        {
            Procesando = true;

            await _membresiaService.DesactivarAsync(
                MembresiaSeleccionada.Id);

            _notificationService.MostrarExito(
                "Membresía desactivada correctamente.");

            LimpiarFormulario();

            await CargarMembresiasAsync();
        }
        catch (InvalidOperationException ex)
        {
            _notificationService.MostrarAdvertencia(
                ex.Message);
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "Ocurrió un error al desactivar la membresía.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private void CargarSeleccionadaEnFormulario()
    {
        if (MembresiaSeleccionada is null)
        {
            return;
        }

        ClienteSeleccionado =
            Clientes.FirstOrDefault(cliente =>
                cliente.Id ==
                MembresiaSeleccionada.ClienteId);

        TipoMembresiaSeleccionado =
            TiposMembresia.FirstOrDefault(tipo =>
                tipo.Id ==
                MembresiaSeleccionada.TipoMembresiaId);

        FechaInicio =
            MembresiaSeleccionada.FechaInicio;
    }

    private void LimpiarFormulario()
    {
        MembresiaSeleccionada = null;
        ClienteSeleccionado = null;
        TipoMembresiaSeleccionado = null;

        FechaInicio = DateTime.Today;
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