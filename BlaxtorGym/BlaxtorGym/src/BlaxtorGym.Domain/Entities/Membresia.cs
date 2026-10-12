using BlaxtorGym.src.BlaxtorGym.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Entities;

public class Membresia : BaseEntity
{
    public DateTime FechaInicio { get; set; }

    public DateTime FechaVencimiento { get; set; }

    public decimal PrecioAplicado { get; set; }

    public bool Activa { get; set; } = true;

    public int ClienteId { get; set; }

    public Cliente Cliente { get; set; } = null!;

    public int TipoMembresiaId { get; set; }

    public TipoMembresia TipoMembresia { get; set; } = null!;

    public int PrecioId { get; set; }

    public Precio Precio { get; set; } = null!;

    [NotMapped]
    public bool Vigente
    {
        get
        {
            DateTime hoy = DateTime.Today;

            return Activa &&
                   FechaInicio.Date <= hoy &&
                   FechaVencimiento.Date >= hoy;
        }
    }

    [NotMapped]
    public string Estado
    {
        get
        {
            if (!Activa)
            {
                return "Inactiva";
            }

            if (DateTime.Today < FechaInicio.Date)
            {
                return "Pendiente";
            }

            if (DateTime.Today > FechaVencimiento.Date)
            {
                return "Vencida";
            }

            return "Vigente";
        }
    }
}