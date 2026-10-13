using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Repositories;

public class MedicionFisicaRepository : IMedicionFisicaRepository
{
    private readonly AppDbContext _context;

    public MedicionFisicaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AgregarAsync(MedicionFisica medicion)
    {
        await _context.MedicionesFisicas.AddAsync(medicion);
        await _context.SaveChangesAsync();
    }

    public async Task<List<MedicionFisica>> ObtenerPorClienteAsync(
        int clienteId)
    {
        return await _context.MedicionesFisicas
            .AsNoTracking()
            .Where(medicion => medicion.ClienteId == clienteId)
            .OrderByDescending(medicion => medicion.FechaMedicion)
            .ThenByDescending(medicion => medicion.Id)
            .ToListAsync();
    }
}
