using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using BlaxtorGym.src.BlaxtorGym.Application.Interfaces;
namespace BlaxtorGym.src.BlaxtorGym.Application.Services;
public class ReporteService
{
    private readonly IReporteRepository _repository;
    public ReporteService(IReporteRepository repository) => _repository = repository;
    public async Task<ReporteGeneralDto> GenerarAsync(ReporteFiltroDto filtro)
    {
        ValidarFiltro(filtro); var clientes = await _repository.ObtenerClientesAsync(filtro); var membresias = await _repository.ObtenerMembresiasAsync(filtro); var pagos = await _repository.ObtenerPagosAsync(filtro); var mediciones = await _repository.ObtenerMedicionesAsync(filtro);
        return new ReporteGeneralDto { Clientes = clientes, Membresias = membresias, Pagos = pagos, Mediciones = mediciones, TotalClientes = clientes.Count, ClientesActivos = clientes.Count(x => x.Activo), ClientesInactivos = clientes.Count(x => !x.Activo), MembresiasVigentes = membresias.Count(x => x.Estado == "Vigente"), MembresiasVencidas = membresias.Count(x => x.Estado == "Vencida"), MembresiasPendientes = membresias.Count(x => x.Estado == "Pendiente"), MembresiasInactivas = membresias.Count(x => x.Estado == "Inactiva"), TotalPagos = pagos.Count, TotalIngresos = pagos.Sum(x => x.Monto) };
    }
    private static void ValidarFiltro(ReporteFiltroDto filtro) { ArgumentNullException.ThrowIfNull(filtro); if (filtro.FechaInicio.HasValue && filtro.FechaFin.HasValue && filtro.FechaInicio.Value.Date > filtro.FechaFin.Value.Date) throw new ArgumentException("La fecha inicial no puede ser posterior a la fecha final."); if (filtro.ClienteId is <= 0 || filtro.TipoClienteId is <= 0 || filtro.TipoMembresiaId is <= 0) throw new ArgumentException("Los identificadores deben ser mayores que cero."); }
}
