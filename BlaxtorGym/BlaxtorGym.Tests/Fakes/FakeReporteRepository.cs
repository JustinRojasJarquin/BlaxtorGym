using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using BlaxtorGym.src.BlaxtorGym.Application.Interfaces;
namespace BlaxtorGym.Tests.Fakes;
public sealed class FakeReporteRepository : IReporteRepository
{
    public ReporteFiltroDto? FiltroClientes { get; private set; } public ReporteFiltroDto? FiltroMembresias { get; private set; } public ReporteFiltroDto? FiltroPagos { get; private set; } public ReporteFiltroDto? FiltroMediciones { get; private set; }
    public List<ReporteClienteItemDto> Clientes { get; set; } = new(); public List<ReporteMembresiaItemDto> Membresias { get; set; } = new(); public List<ReportePagoItemDto> Pagos { get; set; } = new(); public List<ReporteMedicionItemDto> Mediciones { get; set; } = new();
    public Task<List<ReporteClienteItemDto>> ObtenerClientesAsync(ReporteFiltroDto filtro) { FiltroClientes = filtro; return Task.FromResult(Clientes); }
    public Task<List<ReporteMembresiaItemDto>> ObtenerMembresiasAsync(ReporteFiltroDto filtro) { FiltroMembresias = filtro; return Task.FromResult(Membresias); }
    public Task<List<ReportePagoItemDto>> ObtenerPagosAsync(ReporteFiltroDto filtro) { FiltroPagos = filtro; return Task.FromResult(Pagos); }
    public Task<List<ReporteMedicionItemDto>> ObtenerMedicionesAsync(ReporteFiltroDto filtro) { FiltroMediciones = filtro; return Task.FromResult(Mediciones); }
}
