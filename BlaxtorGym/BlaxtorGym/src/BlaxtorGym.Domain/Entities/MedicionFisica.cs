using BlaxtorGym.src.BlaxtorGym.Domain.Common;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Entities;

public class MedicionFisica : BaseEntity
{
    public DateTime FechaMedicion { get; set; } = DateTime.Now;

    public decimal PesoKg { get; set; }

    public decimal? EstaturaCm { get; set; }

    public decimal? PorcentajeGrasa { get; set; }

    public string? Observacion { get; set; }

    public int ClienteId { get; set; }

    public Cliente Cliente { get; set; } = null!;
}