using BlaxtorGym.src.BlaxtorGym.Domain.Entities;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

public interface ITipoClienteRepository
{
    Task<List<TipoCliente>> ObtenerTodosAsync();

    Task<TipoCliente?> ObtenerPorIdAsync(int id);

    Task<bool> ExisteNombreAsync(
        string nombre,
        int? idExcluir = null);

    Task AgregarAsync(TipoCliente tipoCliente);

    Task ActualizarAsync(TipoCliente tipoCliente);
}