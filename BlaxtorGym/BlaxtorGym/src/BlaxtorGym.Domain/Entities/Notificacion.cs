using BlaxtorGym.src.BlaxtorGym.Domain.Common;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Entities;

public class Notificacion : BaseEntity
{
    public string Mensaje { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public DateTime? FechaEnvio { get; set; }

    public bool Enviada { get; set; }

    public int ClienteId { get; set; }

    public Cliente Cliente { get; set; } = null!;

    public int MembresiaId { get; set; }

    public Membresia Membresia { get; set; } = null!;
}