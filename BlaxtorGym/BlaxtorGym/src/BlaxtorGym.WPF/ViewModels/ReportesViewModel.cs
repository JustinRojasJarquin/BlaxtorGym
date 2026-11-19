using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Services;
using BlaxtorGym.src.BlaxtorGym.WPF.Commands;
using BlaxtorGym.src.BlaxtorGym.WPF.Notifications;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;

public class ReportesViewModel : INotifyPropertyChanged
{
    private readonly ReporteService _reporteService;
    private readonly PdfService _pdfService;
    private readonly ClienteService _clienteService;
    private readonly TipoClienteService _tipoClienteService;
    private readonly TipoMembresiaService _tipoMembresiaService;
    private readonly INotificationService _notificationService;
    private string _tipoReporteSeleccionado = "Reporte general";
    private Cliente? _clienteSeleccionado;
    private TipoCliente? _tipoClienteSeleccionado;
    private TipoMembresia? _tipoMembresiaSeleccionado;
    private string? _estadoMembresia;
    private string? _metodoPago;

    public ReportesViewModel(ReporteService reporteService, PdfService pdfService, ClienteService clienteService, TipoClienteService tipoClienteService, TipoMembresiaService tipoMembresiaService, INotificationService notificationService)
    {
        _reporteService = reporteService; _pdfService = pdfService; _clienteService = clienteService; _tipoClienteService = tipoClienteService; _tipoMembresiaService = tipoMembresiaService; _notificationService = notificationService;
        GenerarCommand = new RelayCommand(_ => GenerarAsync(), _ => !Procesando);
        ExportarPdfCommand = new RelayCommand(_ => ExportarPdfAsync(), _ => Resultado is not null && !Procesando);
    }

    public ObservableCollection<string> TiposReporte { get; } = new(new[] { "Reporte general", "Clientes", "Membresías", "Pagos e ingresos", "Mediciones físicas" });
    public ObservableCollection<Cliente> Clientes { get; } = new();
    public ObservableCollection<TipoCliente> TiposCliente { get; } = new();
    public ObservableCollection<TipoMembresia> TiposMembresia { get; } = new();
    public ObservableCollection<string> EstadosMembresia { get; } = new(new[] { "Todos", "Vigente", "Vencida", "Pendiente", "Inactiva" });
    public ObservableCollection<string> MetodosPago { get; } = new(new[] { "Todos", "Efectivo", "Tarjeta", "Transferencia" });
    public string TipoReporteSeleccionado { get => _tipoReporteSeleccionado; set { if (_tipoReporteSeleccionado == value) return; _tipoReporteSeleccionado = value; LimpiarFiltrosSiEsGeneral(); OnPropertyChanged(); OnPropertyChanged(nameof(MostrarFiltroCliente)); OnPropertyChanged(nameof(MostrarFiltroTipoCliente)); OnPropertyChanged(nameof(MostrarFiltroTipoMembresia)); OnPropertyChanged(nameof(MostrarFiltroEstado)); OnPropertyChanged(nameof(MostrarFiltroMetodoPago)); } }
    public Cliente? ClienteSeleccionado { get => _clienteSeleccionado; set { _clienteSeleccionado = value; if (value is not null && value.TipoClienteId > 0) TipoClienteSeleccionado = TiposCliente.FirstOrDefault(x => x.Id == value.TipoClienteId); OnPropertyChanged(); } }
    public TipoCliente? TipoClienteSeleccionado { get => _tipoClienteSeleccionado; set { _tipoClienteSeleccionado = value; OnPropertyChanged(); } }
    public TipoMembresia? TipoMembresiaSeleccionado { get => _tipoMembresiaSeleccionado; set { _tipoMembresiaSeleccionado = value; OnPropertyChanged(); } }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? EstadoMembresia { get => _estadoMembresia; set { _estadoMembresia = value; OnPropertyChanged(); } }
    public string? MetodoPago { get => _metodoPago; set { _metodoPago = value; OnPropertyChanged(); } }
    public ReporteGeneralDto? Resultado { get; private set; }
    public bool Procesando { get; private set; }
    public bool MostrarFiltroCliente => TipoReporteSeleccionado != "Reporte general";
    public bool MostrarFiltroTipoCliente => TipoReporteSeleccionado is "Clientes" or "Membresías" or "Pagos e ingresos" or "Mediciones físicas";
    public bool MostrarFiltroTipoMembresia => TipoReporteSeleccionado is "Membresías" or "Pagos e ingresos";
    public bool MostrarFiltroEstado => TipoReporteSeleccionado == "Membresías";
    public bool MostrarFiltroMetodoPago => TipoReporteSeleccionado == "Pagos e ingresos";
    public ICommand GenerarCommand { get; }
    public ICommand ExportarPdfCommand { get; }

    public async Task CargarCatalogosAsync()
    {
        if (Clientes.Count > 0 || TiposCliente.Count > 0 || TiposMembresia.Count > 0) return;
        foreach (var item in await _clienteService.ObtenerActivosAsync()) Clientes.Add(item);
        foreach (var item in await _tipoClienteService.ObtenerTodosAsync()) TiposCliente.Add(item);
        foreach (var item in await _tipoMembresiaService.ObtenerTodosAsync()) TiposMembresia.Add(item);
    }

    public async Task GenerarAsync()
    {
        try { Procesando = true; OnPropertyChanged(nameof(Procesando)); var filtro = new ReporteFiltroDto { FechaInicio = FechaInicio, FechaFin = FechaFin, ClienteId = ClienteSeleccionado?.Id, TipoClienteId = TipoClienteSeleccionado?.Id, TipoMembresiaId = TipoMembresiaSeleccionado?.Id, EstadoMembresia = NormalizarFiltro(EstadoMembresia), MetodoPago = NormalizarFiltro(MetodoPago) }; Resultado = await _reporteService.GenerarAsync(filtro); OnPropertyChanged(nameof(Resultado)); CommandManager.InvalidateRequerySuggested(); }
        catch (Exception ex) { _notificationService.MostrarError(ex.Message); }
        finally { Procesando = false; OnPropertyChanged(nameof(Procesando)); CommandManager.InvalidateRequerySuggested(); }
    }

    private async Task ExportarPdfAsync()
    {
        if (Resultado is null) return;
        var dialog = new SaveFileDialog { DefaultExt = ".pdf", Filter = "Archivo PDF (*.pdf)|*.pdf", FileName = $"BlaxtorGym_Reporte_{Sanitizar(TipoReporteSeleccionado)}_{DateTime.Now:yyyyMMdd_HHmm}.pdf", AddExtension = true, OverwritePrompt = true };
        if (dialog.ShowDialog() != true) return;
        try { Procesando = true; OnPropertyChanged(nameof(Procesando)); await _pdfService.GenerarAsync(Resultado, $"Reporte de {TipoReporteSeleccionado}", dialog.FileName); _notificationService.MostrarExito($"Reporte exportado correctamente:\n{dialog.FileName}"); }
        catch (Exception ex) { _notificationService.MostrarError($"No fue posible exportar el reporte.\n{ex.Message}"); }
        finally { Procesando = false; OnPropertyChanged(nameof(Procesando)); CommandManager.InvalidateRequerySuggested(); }
    }

    private void LimpiarFiltrosSiEsGeneral() { if (TipoReporteSeleccionado != "Reporte general") return; FechaInicio = null; FechaFin = null; ClienteSeleccionado = null; TipoClienteSeleccionado = null; TipoMembresiaSeleccionado = null; EstadoMembresia = null; MetodoPago = null; }
    private static string? NormalizarFiltro(string? value) => string.IsNullOrWhiteSpace(value) || value == "Todos" ? null : value;
    private static string Sanitizar(string value) => string.Concat(value.Where(c => char.IsLetterOrDigit(c) || c == '_'));
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
