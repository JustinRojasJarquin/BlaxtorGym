using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.WPF.Commands;
using BlaxtorGym.src.BlaxtorGym.WPF.Notifications;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;

public class PagosViewModel : INotifyPropertyChanged
{
    private readonly PagoService _pagoService;
    private readonly MembresiaService _membresiaService;
    private readonly INotificationService _notificationService;

    private Membresia? _membresiaSeleccionada;
    private Pago? _pagoSeleccionado;

    private string? _metodoPagoSeleccionado;
    private string _observacion = string.Empty;
    private DateTime? _fechaPago = DateTime.Today;

    private bool _procesando;

    public PagosViewModel(
        PagoService pagoService,
        MembresiaService membresiaService,
        INotificationService notificationService)
    {
        _pagoService = pagoService;
        _membresiaService = membresiaService;
        _notificationService = notificationService;

        RegistrarCommand = new RelayCommand(
            _ => RegistrarAsync(),
            _ => !Procesando);

        LimpiarCommand = new RelayCommand(
            _ =>
            {
                LimpiarFormulario();
                return Task.CompletedTask;
            },
            _ => !Procesando);

        MostrarTodosCommand = new RelayCommand(
            _ => CargarPagosAsync(),
            _ => !Procesando);

        FiltrarMembresiaCommand = new RelayCommand(
            _ => FiltrarPorMembresiaAsync(),
            _ =>
                !Procesando &&
                MembresiaSeleccionada is not null);
    }

    public ObservableCollection<Pago> Pagos { get; } = new();

    public ObservableCollection<Membresia> Membresias { get; } = new();

    public ObservableCollection<string> MetodosPago { get; } =
        new()
        {
            "Efectivo",
            "SINPE Móvil",
            "Transferencia",
            "Tarjeta"
        };

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
            OnPropertyChanged(nameof(MontoMostrar));

            CommandManager.InvalidateRequerySuggested();
        }
    }

    public string MontoMostrar
    {
        get
        {
            if (MembresiaSeleccionada is null)
            {
                return "₡0";
            }

            return $"₡{MembresiaSeleccionada.PrecioAplicado:N0}";
        }
    }

    public Pago? PagoSeleccionado
    {
        get => _pagoSeleccionado;
        set
        {
            if (_pagoSeleccionado == value)
            {
                return;
            }

            _pagoSeleccionado = value;

            OnPropertyChanged();
        }
    }

    public string? MetodoPagoSeleccionado
    {
        get => _metodoPagoSeleccionado;
        set
        {
            if (_metodoPagoSeleccionado == value)
            {
                return;
            }

            _metodoPagoSeleccionado = value;

            OnPropertyChanged();

            CommandManager.InvalidateRequerySuggested();
        }
    }

    public string Observacion
    {
        get => _observacion;
        set
        {
            if (_observacion == value)
            {
                return;
            }

            _observacion = value;

            OnPropertyChanged();
        }
    }

    public DateTime? FechaPago
    {
        get => _fechaPago;
        set
        {
            if (_fechaPago == value)
            {
                return;
            }

            _fechaPago = value;

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

    public ICommand RegistrarCommand { get; }

    public ICommand LimpiarCommand { get; }

    public ICommand MostrarTodosCommand { get; }

    public ICommand FiltrarMembresiaCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task CargarAsync()
    {
        try
        {
            Procesando = true;

            await CargarMembresiasAsync();

            await CargarPagosAsync();
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "No fue posible cargar la información de pagos.");
        }
        finally
        {
            Procesando = false;
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

    private async Task CargarPagosAsync()
    {
        try
        {
            List<Pago> pagos =
                await _pagoService.ObtenerTodosAsync();

            ActualizarListaPagos(pagos);
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "No fue posible cargar los pagos.");
        }
    }

    private async Task RegistrarAsync()
    {
        try
        {
            Procesando = true;

            if (MembresiaSeleccionada is null)
            {
                throw new ArgumentException(
                    "Debe seleccionar una membresía.");
            }

            if (!FechaPago.HasValue)
            {
                throw new ArgumentException(
                    "Debe seleccionar una fecha de pago.");
            }

            await _pagoService.RegistrarAsync(
                MembresiaSeleccionada.Id,
                MetodoPagoSeleccionado ?? string.Empty,
                Observacion,
                FechaPago.Value);

            _notificationService.MostrarExito(
                "Pago registrado correctamente.");

            LimpiarFormulario();

            await CargarPagosAsync();
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
                "Ocurrió un error al registrar el pago.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task FiltrarPorMembresiaAsync()
    {
        if (MembresiaSeleccionada is null)
        {
            return;
        }

        try
        {
            Procesando = true;

            List<Pago> pagos =
                await _pagoService
                    .ObtenerPorMembresiaAsync(
                        MembresiaSeleccionada.Id);

            ActualizarListaPagos(pagos);
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "No fue posible filtrar los pagos.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private void ActualizarListaPagos(
        IEnumerable<Pago> pagos)
    {
        Pagos.Clear();

        foreach (Pago pago in pagos)
        {
            Pagos.Add(pago);
        }
    }

    private void LimpiarFormulario()
    {
        MembresiaSeleccionada = null;
        PagoSeleccionado = null;

        MetodoPagoSeleccionado = null;
        Observacion = string.Empty;
        FechaPago = DateTime.Today;

        OnPropertyChanged(nameof(MontoMostrar));
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}