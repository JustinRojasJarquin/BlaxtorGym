using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Services;
using BlaxtorGym.src.BlaxtorGym.WPF.Commands;
using BlaxtorGym.src.BlaxtorGym.WPF.Notifications;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
namespace BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
public class ReportesViewModel : INotifyPropertyChanged
{
    private readonly ReporteService _service; private readonly INotificationService _notifications;
    public ReportesViewModel(ReporteService service, PdfService pdfService, ClienteService clientes, TipoClienteService tiposCliente, TipoMembresiaService tiposMembresia, INotificationService notifications) { _service = service; _notifications = notifications; GenerarCommand = new RelayCommand(_ => GenerarAsync()); }
    public DateTime? FechaInicio { get; set; } public DateTime? FechaFin { get; set; } public int? ClienteId { get; set; } public int? TipoClienteId { get; set; } public int? TipoMembresiaId { get; set; } public string? EstadoMembresia { get; set; } public string? MetodoPago { get; set; } public ReporteGeneralDto? Resultado { get; private set; } public ICommand GenerarCommand { get; }
    public async Task GenerarAsync() { try { Resultado = await _service.GenerarAsync(new ReporteFiltroDto { FechaInicio = FechaInicio, FechaFin = FechaFin, ClienteId = ClienteId, TipoClienteId = TipoClienteId, TipoMembresiaId = TipoMembresiaId, EstadoMembresia = EstadoMembresia, MetodoPago = MetodoPago }); OnPropertyChanged(nameof(Resultado)); } catch (Exception ex) { _notifications.MostrarError(ex.Message); } }
    public event PropertyChangedEventHandler? PropertyChanged; private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
