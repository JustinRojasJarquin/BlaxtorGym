namespace BlaxtorGym.src.BlaxtorGym.Application.DTOs;

public class ReporteFiltroDto
{
    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int? ClienteId { get; set; }

    public int? TipoClienteId { get; set; }

    public int? TipoMembresiaId { get; set; }

    public string? EstadoMembresia { get; set; }

    public string? MetodoPago { get; set; }
}