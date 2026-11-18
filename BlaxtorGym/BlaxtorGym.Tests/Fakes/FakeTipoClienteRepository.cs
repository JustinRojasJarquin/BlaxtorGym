using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.Tests.Fakes;

public sealed class FakeTipoClienteRepository : ITipoClienteRepository
{
    public List<TipoCliente> Tipos { get; } = new();
    public TipoCliente? Agregado { get; private set; }
    public TipoCliente? Actualizado { get; private set; }
    public Task<List<TipoCliente>> ObtenerTodosAsync() => Task.FromResult(Tipos.ToList());
    public Task<TipoCliente?> ObtenerPorIdAsync(int id) => Task.FromResult(Tipos.FirstOrDefault(x => x.Id == id));
    public Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null) => Task.FromResult(Tipos.Any(x => x.Nombre == nombre && x.Id != idExcluir));
    public Task AgregarAsync(TipoCliente tipoCliente) { Agregado = tipoCliente; if (!Tipos.Contains(tipoCliente)) Tipos.Add(tipoCliente); return Task.CompletedTask; }
    public Task ActualizarAsync(TipoCliente tipoCliente) { Actualizado = tipoCliente; return Task.CompletedTask; }
}
