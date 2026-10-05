using BlaxtorGym.src.BlaxtorGym.Domain.Common;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Entities;

public class TipoMembresia : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;

    public int DuracionDias { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}