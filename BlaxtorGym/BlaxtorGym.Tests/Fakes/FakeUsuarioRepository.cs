using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.Tests.Fakes;

public class FakeUsuarioRepository : IUsuarioRepository
{
    public Usuario? Usuario { get; set; }

    public int CantidadActualizaciones { get; private set; }

    public Task<Usuario?> ObtenerPorNombreUsuarioAsync(
        string nombreUsuario)
    {
        if (Usuario is null ||
            Usuario.NombreUsuario != nombreUsuario)
        {
            return Task.FromResult<Usuario?>(null);
        }

        return Task.FromResult<Usuario?>(Usuario);
    }

    public Task ActualizarAsync(Usuario usuario)
    {
        Usuario = usuario;

        CantidadActualizaciones++;

        return Task.CompletedTask;
    }
}