using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Repositories;

public class PrecioRepository : IPrecioRepository
{
    private readonly AppDbContext _context;

    public PrecioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Precio>> ObtenerTodosAsync()
    {
        return await _context.Precios
            .Include(precio => precio.TipoCliente)
            .Include(precio => precio.TipoMembresia)
            .AsNoTracking()
            .OrderBy(precio => precio.TipoCliente.Nombre)
            .ThenBy(precio => precio.TipoMembresia.Nombre)
            .ThenByDescending(precio => precio.FechaInicioVigencia)
            .ToListAsync();
    }

    public async Task<Precio?> ObtenerPorIdAsync(int id)
    {
        return await _context.Precios
            .FirstOrDefaultAsync(precio =>
                precio.Id == id);
    }

    public async Task<Precio?> ObtenerActivoPorCombinacionAsync(
        int tipoClienteId,
        int tipoMembresiaId)
    {
        return await _context.Precios
            .Include(precio => precio.TipoCliente)
            .Include(precio => precio.TipoMembresia)
            .AsNoTracking()
            .FirstOrDefaultAsync(precio =>
                precio.TipoClienteId == tipoClienteId &&
                precio.TipoMembresiaId == tipoMembresiaId &&
                precio.Activo);
    }

    public async Task<bool> ExistePrecioActivoAsync(
        int tipoClienteId,
        int tipoMembresiaId,
        int? idExcluir = null)
    {
        return await _context.Precios
            .AnyAsync(precio =>
                precio.TipoClienteId == tipoClienteId &&
                precio.TipoMembresiaId == tipoMembresiaId &&
                precio.Activo &&
                (!idExcluir.HasValue ||
                 precio.Id != idExcluir.Value));
    }

    public async Task AgregarAsync(Precio precio)
    {
        await _context.Precios.AddAsync(precio);

        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Precio precio)
    {
        await _context.SaveChangesAsync();
    }
}