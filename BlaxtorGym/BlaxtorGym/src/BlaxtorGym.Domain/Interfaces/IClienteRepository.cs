using BlaxtorGym.src.BlaxtorGym.Domain.Entities;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

public interface IClienteRepository
{
    Task<List<Cliente>> ObtenerTodosAsync();

    Task<List<Cliente>> ObtenerActivosAsync();

    Task<Cliente?> ObtenerPorIdAsync(int id);

    Task<Cliente?> ObtenerPorCedulaAsync(string cedula);

    Task<bool> ExisteCedulaAsync(
        string cedula,
        int? idExcluir = null);

    Task AgregarAsync(Cliente cliente);

    Task ActualizarAsync(Cliente cliente);
}
