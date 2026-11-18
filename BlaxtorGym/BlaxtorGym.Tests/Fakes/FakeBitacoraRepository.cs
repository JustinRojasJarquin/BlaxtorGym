using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
namespace BlaxtorGym.Tests.Fakes;
public sealed class FakeBitacoraRepository : IBitacoraRepository
{
    public List<BitacoraCambio> Registros { get; } = new();
    public DateTime? FechaInicio { get; private set; } public DateTime? FechaFin { get; private set; } public string? Accion { get; private set; } public string? Entidad { get; private set; } public int? UsuarioId { get; private set; }
    public Task AgregarAsync(BitacoraCambio cambio) { Registros.Add(cambio); return Task.CompletedTask; }
    public Task<List<BitacoraCambio>> ObtenerAsync(DateTime? fechaInicio, DateTime? fechaFin, string? accion, string? entidad, int? usuarioId) { FechaInicio = fechaInicio; FechaFin = fechaFin; Accion = accion; Entidad = entidad; UsuarioId = usuarioId; return Task.FromResult(Registros.ToList()); }
}
