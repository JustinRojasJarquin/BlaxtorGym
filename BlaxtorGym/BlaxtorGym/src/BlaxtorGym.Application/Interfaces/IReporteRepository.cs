using BlaxtorGym.src.BlaxtorGym.Application.DTOs;

namespace BlaxtorGym.src.BlaxtorGym.Application.Interfaces;

public interface IReporteRepository
{
    Task<List<ReporteClienteItemDto>>
        ObtenerClientesAsync(
            ReporteFiltroDto filtro);

    Task<List<ReporteMembresiaItemDto>>
        ObtenerMembresiasAsync(
            ReporteFiltroDto filtro);

    Task<List<ReportePagoItemDto>>
        ObtenerPagosAsync(
            ReporteFiltroDto filtro);

    Task<List<ReporteMedicionItemDto>>
        ObtenerMedicionesAsync(
            ReporteFiltroDto filtro);
}