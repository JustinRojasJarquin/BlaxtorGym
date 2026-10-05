using BlaxtorGym.src.BlaxtorGym.Domain.Common;
using BlaxtorGym.src.BlaxtorGym.Domain.Enums;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Entities;

public class Usuario : BaseEntity
{
    public string NombreUsuario { get; set; } = string.Empty;

    public string ContrasenaHash { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public RolUsuario Rol { get; set; }

    public bool Activo { get; set; } = true;

    public int IntentosFallidos { get; set; }

    public DateTime? BloqueadoHasta { get; set; }
}