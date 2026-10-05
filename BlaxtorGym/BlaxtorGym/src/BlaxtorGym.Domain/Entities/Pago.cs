using BlaxtorGym.src.BlaxtorGym.Domain.Common;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Entities;

public class Pago : BaseEntity
{
    public decimal Monto { get; set; }

    public DateTime FechaPago { get; set; } = DateTime.Now;

    public string MetodoPago { get; set; } = string.Empty;

    public string? Observacion { get; set; }

    public int MembresiaId { get; set; }

    public Membresia Membresia { get; set; } = null!;
}