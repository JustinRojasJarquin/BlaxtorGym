using BlaxtorGym.src.BlaxtorGym.Domain.Common;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Entities;

public class Precio : BaseEntity
{
    public decimal Monto { get; set; }

    public DateTime FechaInicioVigencia { get; set; }

    public DateTime? FechaFinVigencia { get; set; }

    public bool Activo { get; set; } = true;

    public int TipoClienteId { get; set; }

    public TipoCliente TipoCliente { get; set; } = null!;

    public int TipoMembresiaId { get; set; }

    public TipoMembresia TipoMembresia { get; set; } = null!;
}