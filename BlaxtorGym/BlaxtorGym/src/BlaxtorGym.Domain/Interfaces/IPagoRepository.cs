using BlaxtorGym.src.BlaxtorGym.Domain.Entities;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

public interface IPagoRepository
{
    Task<List<Pago>> ObtenerTodosAsync();

    Task<Pago?> ObtenerPorIdAsync(int id);

    Task<List<Pago>> ObtenerPorMembresiaAsync(
        int membresiaId);

    Task<List<Pago>> ObtenerPorClienteAsync(
        int clienteId);

    Task<List<Pago>> ObtenerPorRangoFechasAsync(
        DateTime fechaInicio,
        DateTime fechaFin);

    Task<decimal> ObtenerTotalPagadoPorMembresiaAsync(
        int membresiaId);

    Task AgregarAsync(Pago pago);
}