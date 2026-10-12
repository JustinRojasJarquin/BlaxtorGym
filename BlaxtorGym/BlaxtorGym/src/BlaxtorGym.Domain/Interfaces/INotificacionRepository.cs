using BlaxtorGym.src.BlaxtorGym.Domain.Entities;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

public interface INotificacionRepository
{
    Task<List<Notificacion>> ObtenerTodasAsync();

    Task<List<Notificacion>> ObtenerPendientesAsync();

    Task<Notificacion?> ObtenerPorIdAsync(int id);

    Task<bool> ExisteAsync(
        int membresiaId,
        string mensaje);

    Task AgregarAsync(
        Notificacion notificacion);

    Task ActualizarAsync(
        Notificacion notificacion);
}