using BlaxtorGym.src.BlaxtorGym.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

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

    [NotMapped]
    public decimal? Imc
    {
        get
        {
            if (!EstaturaCm.HasValue ||
                EstaturaCm.Value <= 0)
            {
                return null;
            }

            decimal estaturaMetros =
                EstaturaCm.Value / 100m;

            decimal imc =
                PesoKg /
                (estaturaMetros * estaturaMetros);

            return decimal.Round(imc, 2);
        }
    }
}