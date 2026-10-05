using BlaxtorGym.src.BlaxtorGym.Domain.Common;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Entities;

public class BitacoraCambio : BaseEntity
{
    public DateTime FechaHora { get; set; } = DateTime.Now;

    public string Accion { get; set; } = string.Empty;

    public string Entidad { get; set; } = string.Empty;

    public int? EntidadId { get; set; }

    public string? Descripcion { get; set; }

    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;
}