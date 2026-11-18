using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
namespace BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
public interface IBitacoraRepository { Task AgregarAsync(BitacoraCambio cambio); Task<List<BitacoraCambio>> ObtenerAsync(DateTime? fechaInicio, DateTime? fechaFin, string? accion, string? entidad, int? usuarioId); }
