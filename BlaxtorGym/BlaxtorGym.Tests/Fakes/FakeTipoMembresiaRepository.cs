using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.Tests.Fakes;

public sealed class FakeTipoMembresiaRepository : ITipoMembresiaRepository
{
    public List<TipoMembresia> Tipos { get; } = new();
    public TipoMembresia? Agregado { get; private set; }
    public TipoMembresia? Actualizado { get; private set; }
    public Task<List<TipoMembresia>> ObtenerTodosAsync() => Task.FromResult(Tipos.ToList());
    public Task<TipoMembresia?> ObtenerPorIdAsync(int id) => Task.FromResult(Tipos.FirstOrDefault(x => x.Id == id));
    public Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null) => Task.FromResult(Tipos.Any(x => x.Nombre == nombre && x.Id != idExcluir));
    public Task AgregarAsync(TipoMembresia tipoMembresia) { Agregado = tipoMembresia; if (!Tipos.Contains(tipoMembresia)) Tipos.Add(tipoMembresia); return Task.CompletedTask; }
    public Task ActualizarAsync(TipoMembresia tipoMembresia) { Actualizado = tipoMembresia; return Task.CompletedTask; }
}
