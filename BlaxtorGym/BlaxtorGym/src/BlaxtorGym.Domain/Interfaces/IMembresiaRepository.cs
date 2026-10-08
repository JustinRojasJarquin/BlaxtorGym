using BlaxtorGym.src.BlaxtorGym.Domain.Entities;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

public interface IMembresiaRepository
{
    Task<List<Membresia>> ObtenerTodasAsync();

    Task<Membresia?> ObtenerPorIdAsync(int id);

    Task<Membresia?> ObtenerActivaPorClienteAsync(
        int clienteId);

    Task<List<Membresia>> ObtenerPorClienteAsync(
        int clienteId);

    Task AgregarAsync(Membresia membresia);

    Task ActualizarAsync(Membresia membresia);
}