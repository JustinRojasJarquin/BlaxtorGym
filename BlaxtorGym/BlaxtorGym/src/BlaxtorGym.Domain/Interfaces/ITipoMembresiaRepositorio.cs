using BlaxtorGym.src.BlaxtorGym.Domain.Entities;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

public interface ITipoMembresiaRepository
{
    Task<List<TipoMembresia>> ObtenerTodosAsync();

    Task<TipoMembresia?> ObtenerPorIdAsync(int id);

    Task<bool> ExisteNombreAsync(
        string nombre,
        int? idExcluir = null);

    Task AgregarAsync(TipoMembresia tipoMembresia);

    Task ActualizarAsync(TipoMembresia tipoMembresia);
}