using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.Tests.Fakes;

public sealed class FakeClienteRepository : IClienteRepository
{
    public List<Cliente> Clientes { get; } = new();
    public Cliente? Agregado { get; private set; }
    public Cliente? Actualizado { get; private set; }

    public Task<List<Cliente>> BuscarAsync(string criterio, int limite = 100) =>
        Task.FromResult(Clientes.Where(c => c.NombreCompleto.Contains(criterio, StringComparison.OrdinalIgnoreCase) || c.Cedula.Contains(criterio, StringComparison.OrdinalIgnoreCase)).Take(limite).ToList());
    public Task<List<Cliente>> ObtenerActivosAsync() => Task.FromResult(Clientes.Where(c => c.Activo).ToList());
    public Task<Cliente?> ObtenerPorIdAsync(int id) => Task.FromResult(Clientes.FirstOrDefault(c => c.Id == id));
    public Task<Cliente?> ObtenerPorCedulaAsync(string cedula) => Task.FromResult(Clientes.FirstOrDefault(c => c.Cedula == cedula));
    public Task<bool> ExisteCedulaAsync(string cedula, int? idExcluir = null) => Task.FromResult(Clientes.Any(c => c.Cedula == cedula && c.Id != idExcluir));
    public Task AgregarAsync(Cliente cliente) { Agregado = cliente; if (!Clientes.Contains(cliente)) Clientes.Add(cliente); return Task.CompletedTask; }
    public Task ActualizarAsync(Cliente cliente) { Actualizado = cliente; return Task.CompletedTask; }
}
