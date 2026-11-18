using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.Tests.Fakes;

public sealed class FakePagoRepository : IPagoRepository
{
    public List<Pago> Pagos { get; } = new();
    public Pago? Agregado { get; private set; }
    public decimal TotalPagado { get; set; }
    public Task<List<Pago>> ObtenerTodosAsync() => Task.FromResult(Pagos.ToList());
    public Task<Pago?> ObtenerPorIdAsync(int id) => Task.FromResult(Pagos.FirstOrDefault(x => x.Id == id));
    public Task<List<Pago>> ObtenerPorMembresiaAsync(int membresiaId) => Task.FromResult(Pagos.Where(x => x.MembresiaId == membresiaId).ToList());
    public Task<List<Pago>> ObtenerPorClienteAsync(int clienteId) => Task.FromResult(Pagos.Where(x => x.Membresia?.ClienteId == clienteId).ToList());
    public Task<List<Pago>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin) => Task.FromResult(Pagos.Where(x => x.FechaPago.Date >= fechaInicio.Date && x.FechaPago.Date <= fechaFin.Date).ToList());
    public Task<decimal> ObtenerTotalPagadoPorMembresiaAsync(int membresiaId) => Task.FromResult(TotalPagado);
    public Task AgregarAsync(Pago pago) { Agregado = pago; if (!Pagos.Contains(pago)) Pagos.Add(pago); return Task.CompletedTask; }
}
