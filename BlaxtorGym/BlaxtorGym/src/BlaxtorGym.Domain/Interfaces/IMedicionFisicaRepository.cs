using BlaxtorGym.src.BlaxtorGym.Domain.Entities;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

public interface IMedicionFisicaRepository
{
    Task AgregarAsync(MedicionFisica medicion);

    Task<List<MedicionFisica>> ObtenerPorClienteAsync(
        int clienteId);
}
