using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Repositories;

public class TipoMembresiaRepository : ITipoMembresiaRepository
{
    private readonly AppDbContext _context;

    public TipoMembresiaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TipoMembresia>> ObtenerTodosAsync()
    {
        return await _context.TiposMembresia
            .AsNoTracking()
            .OrderBy(tipo => tipo.DuracionDias)
            .ThenBy(tipo => tipo.Nombre)
            .ToListAsync();
    }

    public async Task<TipoMembresia?> ObtenerPorIdAsync(int id)
    {
        return await _context.TiposMembresia
            .FirstOrDefaultAsync(tipo =>
                tipo.Id == id);
    }

    public async Task<bool> ExisteNombreAsync(
        string nombre,
        int? idExcluir = null)
    {
        string nombreNormalizado = nombre.Trim();

        return await _context.TiposMembresia
            .AnyAsync(tipo =>
                tipo.Nombre == nombreNormalizado &&
                (!idExcluir.HasValue ||
                 tipo.Id != idExcluir.Value));
    }

    public async Task AgregarAsync(
        TipoMembresia tipoMembresia)
    {
        await _context.TiposMembresia
            .AddAsync(tipoMembresia);

        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(
        TipoMembresia tipoMembresia)
    {
        await _context.SaveChangesAsync();
    }
}