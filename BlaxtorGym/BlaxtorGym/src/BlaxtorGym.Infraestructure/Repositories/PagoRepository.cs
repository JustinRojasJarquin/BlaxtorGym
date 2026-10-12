using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Repositories;

public class PagoRepository : IPagoRepository
{
    private readonly AppDbContext _context;

    public PagoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Pago>> ObtenerTodosAsync()
    {
        return await _context.Pagos
            .Include(p => p.Membresia)
                .ThenInclude(m => m.Cliente)
            .Include(p => p.Membresia)
                .ThenInclude(m => m.TipoMembresia)
            .AsNoTracking()
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync();
    }

    public async Task<Pago?> ObtenerPorIdAsync(int id)
    {
        return await _context.Pagos
            .Include(p => p.Membresia)
                .ThenInclude(m => m.Cliente)
            .Include(p => p.Membresia)
                .ThenInclude(m => m.TipoMembresia)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Pago>> ObtenerPorMembresiaAsync(
        int membresiaId)
    {
        return await _context.Pagos
            .Include(p => p.Membresia)
                .ThenInclude(m => m.Cliente)
            .Include(p => p.Membresia)
                .ThenInclude(m => m.TipoMembresia)
            .AsNoTracking()
            .Where(p => p.MembresiaId == membresiaId)
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync();
    }

    public async Task<List<Pago>> ObtenerPorClienteAsync(
        int clienteId)
    {
        return await _context.Pagos
            .Include(p => p.Membresia)
                .ThenInclude(m => m.Cliente)
            .Include(p => p.Membresia)
                .ThenInclude(m => m.TipoMembresia)
            .AsNoTracking()
            .Where(p => p.Membresia.ClienteId == clienteId)
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync();
    }

    public async Task<List<Pago>> ObtenerPorRangoFechasAsync(
        DateTime fechaInicio,
        DateTime fechaFin)
    {
        DateTime inicio = fechaInicio.Date;
        DateTime finExclusivo = fechaFin.Date.AddDays(1);

        return await _context.Pagos
            .Include(p => p.Membresia)
                .ThenInclude(m => m.Cliente)
            .Include(p => p.Membresia)
                .ThenInclude(m => m.TipoMembresia)
            .AsNoTracking()
            .Where(p =>
                p.FechaPago >= inicio &&
                p.FechaPago < finExclusivo)
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync();
    }

    public async Task<decimal> ObtenerTotalPagadoPorMembresiaAsync(
        int membresiaId)
    {
        return await _context.Pagos
            .AsNoTracking()
            .Where(p => p.MembresiaId == membresiaId)
            .SumAsync(p => (decimal?)p.Monto)
            ?? 0m;
    }

    public async Task AgregarAsync(Pago pago)
    {
        await _context.Pagos.AddAsync(pago);

        await _context.SaveChangesAsync();
    }
}