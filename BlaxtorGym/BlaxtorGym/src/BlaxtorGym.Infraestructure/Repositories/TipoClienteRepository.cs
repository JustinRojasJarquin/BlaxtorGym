using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Repositories;

public class TipoClienteRepository : ITipoClienteRepository
{
    private readonly AppDbContext _context;

    public TipoClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TipoCliente>> ObtenerTodosAsync()
    {
        return await _context.TiposCliente
            .AsNoTracking()
            .OrderBy(tipo => tipo.Nombre)
            .ToListAsync();
    }

    public async Task<TipoCliente?> ObtenerPorIdAsync(int id)
    {
        return await _context.TiposCliente
            .FirstOrDefaultAsync(tipo => tipo.Id == id);
    }

    public async Task<bool> ExisteNombreAsync(
        string nombre,
        int? idExcluir = null)
    {
        string nombreNormalizado = nombre.Trim();

        return await _context.TiposCliente
            .AnyAsync(tipo =>
                tipo.Nombre == nombreNormalizado &&
                (!idExcluir.HasValue ||
                 tipo.Id != idExcluir.Value));
    }

    public async Task AgregarAsync(TipoCliente tipoCliente)
    {
        await _context.TiposCliente.AddAsync(tipoCliente);

        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(TipoCliente tipoCliente)
    {
        _context.TiposCliente.Update(tipoCliente);

        await _context.SaveChangesAsync();
    }
}