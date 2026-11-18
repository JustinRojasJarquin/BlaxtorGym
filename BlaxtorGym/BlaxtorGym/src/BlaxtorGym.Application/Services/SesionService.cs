using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
namespace BlaxtorGym.src.BlaxtorGym.Application.Services;
public class SesionService
{
    public Usuario? UsuarioActual { get; private set; }
    public bool HaySesionActiva => UsuarioActual is not null;
    public int? UsuarioId => UsuarioActual?.Id;
    public string NombreUsuario => UsuarioActual?.NombreUsuario ?? string.Empty;
    public void IniciarSesion(Usuario usuario) { ArgumentNullException.ThrowIfNull(usuario); UsuarioActual = usuario; }
    public void CerrarSesion() => UsuarioActual = null;
}
