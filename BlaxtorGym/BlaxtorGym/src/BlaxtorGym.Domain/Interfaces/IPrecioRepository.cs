using BlaxtorGym.src.BlaxtorGym.Domain.Entities;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

public interface IPrecioRepository
{
    Task<List<Precio>> ObtenerTodosAsync();

    Task<Precio?> ObtenerPorIdAsync(int id);

    Task<Precio?> ObtenerActivoPorCombinacionAsync(
        int tipoClienteId,
        int tipoMembresiaId);

    Task<bool> ExistePrecioActivoAsync(
        int tipoClienteId,
        int tipoMembresiaId,
        int? idExcluir = null);

    Task AgregarAsync(Precio precio);

    Task ActualizarAsync(Precio precio);
}