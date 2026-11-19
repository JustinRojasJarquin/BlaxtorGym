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

public class MedicionesViewModel : INotifyPropertyChanged
{
    private readonly ClienteService _clienteService;
    private readonly MedicionFisicaService _medicionService;
    private readonly INotificationService _notificationService;

    private Cliente? _clienteSeleccionado;
    private DateTime? _fechaMedicion = DateTime.Today;
    private string _pesoKg = string.Empty;
    private string _estaturaCm = string.Empty;
    private string _porcentajeGrasa = string.Empty;
    private string _hombros = string.Empty;
    private string _pecho = string.Empty;
    private string _bicepsDerecho = string.Empty;
    private string _bicepsIzquierdo = string.Empty;
    private string _cintura = string.Empty;
    private string _musloDerecho = string.Empty;
    private string _musloIzquierdo = string.Empty;
    private string _observacion = string.Empty;

    private bool _procesando;
    private bool _cargandoDatosIniciales;

    public MedicionesViewModel(
        ClienteService clienteService,
        MedicionFisicaService medicionService,
        INotificationService notificationService)
    {
        _clienteService = clienteService;
        _medicionService = medicionService;
        _notificationService = notificationService;

        RegistrarCommand = new RelayCommand(
            _ => RegistrarAsync(),
            _ => !Procesando &&
                 ClienteSeleccionado is not null);

        LimpiarCommand = new RelayCommand(
            _ =>
            {
                LimpiarFormulario();
                return Task.CompletedTask;
            },
            _ => !Procesando);
    }

    public ObservableCollection<Cliente> ClientesActivos { get; }
        = new();

    public ObservableCollection<MedicionFisica> Historial { get; }
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

            Historial.Clear();

            OnPropertyChanged();

            CommandManager.InvalidateRequerySuggested();

            if (_clienteSeleccionado is not null &&
                !_cargandoDatosIniciales)
            {
                _ = CargarHistorialAlCambiarClienteAsync(
                    _clienteSeleccionado.Id);
            }
        }
    }

    public DateTime? FechaMedicion
    {
        get => _fechaMedicion;

        set
        {
            if (_fechaMedicion == value)
            {
                return;
            }

            _fechaMedicion = value;

            OnPropertyChanged();
        }
    }

    public string PesoKg
    {
        get => _pesoKg;

        set
        {
            if (_pesoKg == value)
            {
                return;
            }

            _pesoKg = value;

            OnPropertyChanged();

            OnPropertyChanged(
                nameof(ImcActual));
        }
    }

    public string EstaturaCm
    {
        get => _estaturaCm;

        set
        {
            if (_estaturaCm == value)
            {
                return;
            }

            _estaturaCm = value;

            OnPropertyChanged();

            OnPropertyChanged(
                nameof(ImcActual));
        }
    }

    public string PorcentajeGrasa
    {
        get => _porcentajeGrasa;

        set
        {
            if (_porcentajeGrasa == value)
            {
                return;
            }

            _porcentajeGrasa = value;

            OnPropertyChanged();
        }
    }

    public string Hombros { get => _hombros; set { if (_hombros != value) { _hombros = value; OnPropertyChanged(); } } }
    public string Pecho { get => _pecho; set { if (_pecho != value) { _pecho = value; OnPropertyChanged(); } } }
    public string BicepsDerecho { get => _bicepsDerecho; set { if (_bicepsDerecho != value) { _bicepsDerecho = value; OnPropertyChanged(); } } }
    public string BicepsIzquierdo { get => _bicepsIzquierdo; set { if (_bicepsIzquierdo != value) { _bicepsIzquierdo = value; OnPropertyChanged(); } } }
    public string Cintura { get => _cintura; set { if (_cintura != value) { _cintura = value; OnPropertyChanged(); } } }
    public string MusloDerecho { get => _musloDerecho; set { if (_musloDerecho != value) { _musloDerecho = value; OnPropertyChanged(); } } }
    public string MusloIzquierdo { get => _musloIzquierdo; set { if (_musloIzquierdo != value) { _musloIzquierdo = value; OnPropertyChanged(); } } }

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

    public string ImcActual
    {
        get
        {
            if (!IntentarLeerNumero(
                    PesoKg,
                    out decimal peso) ||
                !IntentarLeerNumero(
                    EstaturaCm,
                    out decimal estaturaCm) ||
                peso <= 0 ||
                estaturaCm <= 0)
            {
                return "—";
            }

            decimal estaturaMetros =
                estaturaCm / 100m;

            decimal imc =
                peso /
                (estaturaMetros * estaturaMetros);

            return decimal
                .Round(imc, 2)
                .ToString(
                    "N2",
                    CultureInfo.CurrentCulture);
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

            CommandManager
                .InvalidateRequerySuggested();
        }
    }

    public ICommand RegistrarCommand { get; }

    public ICommand LimpiarCommand { get; }

    public event PropertyChangedEventHandler?
        PropertyChanged;

    public async Task CargarAsync()
    {
        try
        {
            Procesando = true;
            _cargandoDatosIniciales = true;

            int? clienteIdSeleccionado =
                ClienteSeleccionado?.Id;

            List<Cliente> clientes =
                await _clienteService
                    .ObtenerActivosAsync();

            ClientesActivos.Clear();

            foreach (Cliente cliente in clientes)
            {
                ClientesActivos.Add(cliente);
            }

            ClienteSeleccionado =
                clienteIdSeleccionado.HasValue
                    ? ClientesActivos
                        .FirstOrDefault(
                            cliente =>
                                cliente.Id ==
                                clienteIdSeleccionado.Value)
                    : ClientesActivos
                        .FirstOrDefault();

            if (ClienteSeleccionado is not null)
            {
                await CargarHistorialInternoAsync(
                    ClienteSeleccionado.Id);
            }
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "No fue posible cargar los clientes y sus mediciones.");
        }
        finally
        {
            _cargandoDatosIniciales = false;
            Procesando = false;
        }
    }

    private async Task
        CargarHistorialAlCambiarClienteAsync(
            int clienteId)
    {
        try
        {
            List<MedicionFisica> mediciones =
                await _medicionService
                    .ObtenerHistorialPorClienteAsync(
                        clienteId);

            // El usuario pudo cambiar nuevamente de cliente
            // mientras se hacía la consulta.
            if (ClienteSeleccionado?.Id != clienteId)
            {
                return;
            }

            ActualizarHistorial(
                mediciones);
        }
        catch (Exception)
        {
            if (ClienteSeleccionado?.Id ==
                clienteId)
            {
                _notificationService.MostrarError(
                    "No fue posible cargar el historial de mediciones.");
            }
        }
    }

    private async Task RegistrarAsync()
    {
        try
        {
            Procesando = true;

            if (ClienteSeleccionado is null)
            {
                throw new ArgumentException(
                    "Debe seleccionar un cliente activo.");
            }

            if (!FechaMedicion.HasValue)
            {
                throw new ArgumentException(
                    "Debe seleccionar la fecha de medición.");
            }

            int clienteId =
                ClienteSeleccionado.Id;

            decimal pesoKg =
                LeerNumeroRequerido(
                    PesoKg,
                    "Debe ingresar un peso válido.");

            decimal? estaturaCm =
                LeerNumeroOpcional(
                    EstaturaCm);

            decimal? porcentajeGrasa =
                LeerNumeroOpcional(
                    PorcentajeGrasa);

            decimal? hombros = LeerNumeroOpcional(Hombros);
            decimal? pecho = LeerNumeroOpcional(Pecho);
            decimal? bicepsDerecho = LeerNumeroOpcional(BicepsDerecho);
            decimal? bicepsIzquierdo = LeerNumeroOpcional(BicepsIzquierdo);
            decimal? cintura = LeerNumeroOpcional(Cintura);
            decimal? musloDerecho = LeerNumeroOpcional(MusloDerecho);
            decimal? musloIzquierdo = LeerNumeroOpcional(MusloIzquierdo);

            await _medicionService
                .RegistrarAsync(
                    clienteId,
                    FechaMedicion.Value,
                    pesoKg,
                    estaturaCm,
                    porcentajeGrasa,
                    Observacion,
                    hombros,
                    pecho,
                    bicepsDerecho,
                    bicepsIzquierdo,
                    cintura,
                    musloDerecho,
                    musloIzquierdo);

            _notificationService.MostrarExito(
                "Medición registrada correctamente.");

            LimpiarFormulario();

            try
            {
                await CargarHistorialInternoAsync(
                    clienteId);
            }
            catch (Exception)
            {
                _notificationService.MostrarError(
                    "La medición se guardó, pero no fue posible actualizar el historial.");
            }
        }
        catch (ArgumentException ex)
        {
            _notificationService
                .MostrarAdvertencia(
                    ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _notificationService
                .MostrarAdvertencia(
                    ex.Message);
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "Ocurrió un error al registrar la medición.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task
        CargarHistorialInternoAsync(
            int clienteId)
    {
        List<MedicionFisica> mediciones =
            await _medicionService
                .ObtenerHistorialPorClienteAsync(
                    clienteId);

        if (ClienteSeleccionado?.Id != clienteId)
        {
            return;
        }

        ActualizarHistorial(
            mediciones);
    }

    private void ActualizarHistorial(
        IEnumerable<MedicionFisica> mediciones)
    {
        Historial.Clear();

        foreach (MedicionFisica medicion
                 in mediciones)
        {
            Historial.Add(
                medicion);
        }
    }

    private static decimal
        LeerNumeroRequerido(
            string texto,
            string mensajeError)
    {
        if (!IntentarLeerNumero(
                texto,
                out decimal valor))
        {
            throw new ArgumentException(
                mensajeError);
        }

        return valor;
    }

    private static decimal?
        LeerNumeroOpcional(
            string texto)
    {
        if (string.IsNullOrWhiteSpace(
                texto))
        {
            return null;
        }

        if (!IntentarLeerNumero(
                texto,
                out decimal valor))
        {
            throw new ArgumentException(
                "Las mediciones opcionales deben contener un número válido.");
        }

        return valor;
    }

    private static bool IntentarLeerNumero(
        string texto,
        out decimal valor)
    {
        const NumberStyles estilos =
            NumberStyles.AllowLeadingSign |
            NumberStyles.AllowDecimalPoint;

        return decimal.TryParse(
                   texto,
                   estilos,
                   CultureInfo.CurrentCulture,
                   out valor) ||
               decimal.TryParse(
                   texto,
                   estilos,
                   CultureInfo.InvariantCulture,
                   out valor);
    }

    private void LimpiarFormulario()
    {
        FechaMedicion =
            DateTime.Today;

        PesoKg =
            string.Empty;

        EstaturaCm =
            string.Empty;

        PorcentajeGrasa =
            string.Empty;

        Hombros = string.Empty;
        Pecho = string.Empty;
        BicepsDerecho = string.Empty;
        BicepsIzquierdo = string.Empty;
        Cintura = string.Empty;
        MusloDerecho = string.Empty;
        MusloIzquierdo = string.Empty;

        Observacion =
            string.Empty;
    }

    private void OnPropertyChanged(
        [CallerMemberName]
        string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                propertyName));
    }
}
