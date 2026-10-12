using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Repositories;

public class NotificacionRepository : INotificacionRepository
{
    private readonly AppDbContext _context;

    public NotificacionRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notificacion>> ObtenerTodasAsync()
    {
        return await _context.Notificaciones
            .Include(n => n.Cliente)
            .Include(n => n.Membresia)
                .ThenInclude(m => m.TipoMembresia)
            .AsNoTracking()
            .OrderByDescending(n => n.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<Notificacion>> ObtenerPendientesAsync()
    {
        return await _context.Notificaciones
            .Include(n => n.Cliente)
            .Include(n => n.Membresia)
                .ThenInclude(m => m.TipoMembresia)
            .AsNoTracking()
            .Where(n => !n.Enviada)
            .OrderBy(n => n.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Notificacion?> ObtenerPorIdAsync(
        int id)
    {
        return await _context.Notificaciones
            .Include(n => n.Cliente)
            .Include(n => n.Membresia)
                .ThenInclude(m => m.TipoMembresia)
            .FirstOrDefaultAsync(n =>
                n.Id == id);
    }

    public async Task<bool> ExisteAsync(
        int membresiaId,
        string mensaje)
    {
        return await _context.Notificaciones
            .AsNoTracking()
            .AnyAsync(n =>
                n.MembresiaId == membresiaId &&
                n.Mensaje == mensaje);
    }

    public async Task AgregarAsync(
        Notificacion notificacion)
    {
        await _context.Notificaciones
            .AddAsync(notificacion);

        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(
        Notificacion notificacion)
    {
        await _context.SaveChangesAsync();
    }
}