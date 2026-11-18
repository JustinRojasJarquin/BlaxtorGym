using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.Tests.Fakes;

public sealed class FakeMembresiaRepository : IMembresiaRepository
{
    public List<Membresia> Membresias { get; } = new();
    public Membresia? Agregada { get; private set; }
    public Membresia? Actualizada { get; private set; }
    public Task<List<Membresia>> ObtenerTodasAsync() => Task.FromResult(Membresias.ToList());
    public Task<Membresia?> ObtenerPorIdAsync(int id) => Task.FromResult(Membresias.FirstOrDefault(x => x.Id == id));
    public Task<Membresia?> ObtenerActivaPorClienteAsync(int clienteId) => Task.FromResult(Membresias.FirstOrDefault(x => x.ClienteId == clienteId && x.Activa));
    public Task<List<Membresia>> ObtenerPorClienteAsync(int clienteId) => Task.FromResult(Membresias.Where(x => x.ClienteId == clienteId).ToList());
    public Task AgregarAsync(Membresia membresia) { Agregada = membresia; if (!Membresias.Contains(membresia)) Membresias.Add(membresia); return Task.CompletedTask; }
    public Task ActualizarAsync(Membresia membresia) { Actualizada = membresia; return Task.CompletedTask; }
}
