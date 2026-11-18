using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.Tests.Fakes;

public sealed class FakePrecioRepository : IPrecioRepository
{
    public List<Precio> Precios { get; } = new();
    public Precio? Agregado { get; private set; }
    public Precio? Actualizado { get; private set; }
    public Task<List<Precio>> ObtenerTodosAsync() => Task.FromResult(Precios.ToList());
    public Task<Precio?> ObtenerPorIdAsync(int id) => Task.FromResult(Precios.FirstOrDefault(x => x.Id == id));
    public Task<Precio?> ObtenerActivoPorCombinacionAsync(int clienteId, int membresiaId) => Task.FromResult(Precios.FirstOrDefault(x => x.Activo && x.TipoClienteId == clienteId && x.TipoMembresiaId == membresiaId));
    public Task<bool> ExistePrecioActivoAsync(int clienteId, int membresiaId, int? idExcluir = null) => Task.FromResult(Precios.Any(x => x.Activo && x.TipoClienteId == clienteId && x.TipoMembresiaId == membresiaId && x.Id != idExcluir));
    public Task AgregarAsync(Precio precio) { Agregado = precio; if (!Precios.Contains(precio)) Precios.Add(precio); return Task.CompletedTask; }
    public Task ActualizarAsync(Precio precio) { Actualizado = precio; return Task.CompletedTask; }
}
