using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
namespace BlaxtorGym.Tests.Fakes;
public sealed class FakeNotificacionRepository : INotificacionRepository
{
    public List<Notificacion> Notificaciones { get; } = new(); public Notificacion? Actualizada { get; private set; }
    public Task<List<Notificacion>> ObtenerTodasAsync() => Task.FromResult(Notificaciones.ToList()); public Task<List<Notificacion>> ObtenerPendientesAsync() => Task.FromResult(Notificaciones.Where(x => !x.Enviada).ToList()); public Task<Notificacion?> ObtenerPorIdAsync(int id) => Task.FromResult(Notificaciones.FirstOrDefault(x => x.Id == id));
    public Task<bool> ExisteAsync(int membresiaId, string mensaje) => Task.FromResult(Notificaciones.Any(x => x.MembresiaId == membresiaId && x.Mensaje == mensaje));
    public Task AgregarAsync(Notificacion notificacion) { Notificaciones.Add(notificacion); return Task.CompletedTask; } public Task ActualizarAsync(Notificacion notificacion) { Actualizada = notificacion; return Task.CompletedTask; }
}
