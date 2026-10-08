using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.WPF.Commands;
using BlaxtorGym.src.BlaxtorGym.WPF.Notifications;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;

public class TarifasViewModel : INotifyPropertyChanged
{
    private readonly TarifaService _tarifaService;
    private readonly TipoClienteService _tipoClienteService;
    private readonly TipoMembresiaService _tipoMembresiaService;
    private readonly INotificationService _notificationService;

    private Precio? _tarifaSeleccionada;
    private TipoCliente? _tipoClienteSeleccionado;
    private TipoMembresia? _tipoMembresiaSeleccionado;

    private string _monto = string.Empty;
    private bool _procesando;

    public TarifasViewModel(
        TarifaService tarifaService,
        TipoClienteService tipoClienteService,
        TipoMembresiaService tipoMembresiaService,
        INotificationService notificationService)
    {
        _tarifaService = tarifaService;
        _tipoClienteService = tipoClienteService;
        _tipoMembresiaService = tipoMembresiaService;
        _notificationService = notificationService;

        CrearCommand = new RelayCommand(
            _ => CrearAsync(),
            _ => !Procesando);

        CambiarTarifaCommand = new RelayCommand(
            _ => CambiarTarifaAsync(),
            _ =>
                !Procesando &&
                TarifaSeleccionada is not null);

        DesactivarCommand = new RelayCommand(
            _ => DesactivarAsync(),
            _ =>
                !Procesando &&
                TarifaSeleccionada is not null &&
                TarifaSeleccionada.Activo);

        LimpiarCommand = new RelayCommand(
            _ =>
            {
                LimpiarFormulario();
                return Task.CompletedTask;
            },
            _ => !Procesando);
    }

    public ObservableCollection<Precio> Tarifas { get; }
        = new();

    public ObservableCollection<TipoCliente> TiposCliente { get; }
        = new();

    public ObservableCollection<TipoMembresia> TiposMembresia { get; }
        = new();

    public Precio? TarifaSeleccionada
    {
        get => _tarifaSeleccionada;
        set
        {
            if (_tarifaSeleccionada == value)
            {
                return;
            }

            _tarifaSeleccionada = value;

            OnPropertyChanged();

            CargarSeleccionadaEnFormulario();

            CommandManager.InvalidateRequerySuggested();
        }
    }

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

    public string Monto
    {
        get => _monto;
        set
        {
            if (_monto == value)
            {
                return;
            }

            _monto = value;
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

    public ICommand CambiarTarifaCommand { get; }

    public ICommand DesactivarCommand { get; }

    public ICommand LimpiarCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task CargarAsync()
    {
        try
        {
            Procesando = true;

            await CargarTiposClienteAsync();
            await CargarTiposMembresiaAsync();
            await CargarTarifasAsync();
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "No fue posible cargar la información de tarifas.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task CargarTiposClienteAsync()
    {
        List<TipoCliente> tipos =
            await _tipoClienteService.ObtenerTodosAsync();

        TiposCliente.Clear();

        foreach (TipoCliente tipo in tipos)
        {
            if (tipo.Activo)
            {
                TiposCliente.Add(tipo);
            }
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

    private async Task CargarTarifasAsync()
    {
        List<Precio> tarifas =
            await _tarifaService.ObtenerTodosAsync();

        Tarifas.Clear();

        foreach (Precio tarifa in tarifas)
        {
            Tarifas.Add(tarifa);
        }
    }

    private async Task CrearAsync()
    {
        try
        {
            Procesando = true;

            decimal monto =
                ObtenerMonto();

            int tipoClienteId =
                TipoClienteSeleccionado?.Id ?? 0;

            int tipoMembresiaId =
                TipoMembresiaSeleccionado?.Id ?? 0;

            await _tarifaService.CrearAsync(
                tipoClienteId,
                tipoMembresiaId,
                monto);

            _notificationService.MostrarExito(
                "Tarifa creada correctamente.");

            LimpiarFormulario();

            await CargarTarifasAsync();
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
                "Ocurrió un error al crear la tarifa.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task CambiarTarifaAsync()
    {
        if (TarifaSeleccionada is null)
        {
            return;
        }

        try
        {
            Procesando = true;

            decimal nuevoMonto =
                ObtenerMonto();

            await _tarifaService.CambiarTarifaAsync(
                TarifaSeleccionada.TipoClienteId,
                TarifaSeleccionada.TipoMembresiaId,
                nuevoMonto);

            _notificationService.MostrarExito(
                "Tarifa actualizada correctamente.");

            LimpiarFormulario();

            await CargarTarifasAsync();
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
                "Ocurrió un error al cambiar la tarifa.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task DesactivarAsync()
    {
        if (TarifaSeleccionada is null)
        {
            return;
        }

        try
        {
            Procesando = true;

            await _tarifaService.DesactivarAsync(
                TarifaSeleccionada.Id);

            _notificationService.MostrarExito(
                "Tarifa desactivada correctamente.");

            LimpiarFormulario();

            await CargarTarifasAsync();
        }
        catch (InvalidOperationException ex)
        {
            _notificationService.MostrarAdvertencia(
                ex.Message);
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "Ocurrió un error al desactivar la tarifa.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private decimal ObtenerMonto()
    {
        string montoNormalizado =
            Monto.Trim()
                .Replace("₡", string.Empty)
                .Replace(" ", string.Empty);

        bool valido =
            decimal.TryParse(
                montoNormalizado,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal monto);

        if (!valido)
        {
            throw new ArgumentException(
                "El monto debe ser un número válido.");
        }

        return monto;
    }

    private void CargarSeleccionadaEnFormulario()
    {
        if (TarifaSeleccionada is null)
        {
            return;
        }

        TipoClienteSeleccionado =
            TiposCliente.FirstOrDefault(tipo =>
                tipo.Id ==
                TarifaSeleccionada.TipoClienteId);

        TipoMembresiaSeleccionado =
            TiposMembresia.FirstOrDefault(tipo =>
                tipo.Id ==
                TarifaSeleccionada.TipoMembresiaId);

        Monto =
            TarifaSeleccionada.Monto.ToString("0.##");
    }

    private void LimpiarFormulario()
    {
        TarifaSeleccionada = null;

        TipoClienteSeleccionado = null;
        TipoMembresiaSeleccionado = null;

        Monto = string.Empty;
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