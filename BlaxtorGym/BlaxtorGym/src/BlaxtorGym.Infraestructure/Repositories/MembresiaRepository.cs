using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Repositories;

public class MembresiaRepository : IMembresiaRepository
{
    private readonly AppDbContext _context;

    public MembresiaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Membresia>> ObtenerTodasAsync()
    {
        return await _context.Membresias
            .Include(membresia => membresia.Cliente)
            .Include(membresia => membresia.TipoMembresia)
            .Include(membresia => membresia.Precio)
            .AsNoTracking()
            .OrderByDescending(membresia => membresia.FechaInicio)
            .ToListAsync();
    }

    public async Task<Membresia?> ObtenerPorIdAsync(int id)
    {
        return await _context.Membresias
            .FirstOrDefaultAsync(membresia =>
                membresia.Id == id);
    }

    public async Task<Membresia?> ObtenerActivaPorClienteAsync(
        int clienteId)
    {
        return await _context.Membresias
            .Include(membresia => membresia.TipoMembresia)
            .Include(membresia => membresia.Precio)
            .AsNoTracking()
            .Where(membresia =>
                membresia.ClienteId == clienteId &&
                membresia.Activa)
            .OrderByDescending(membresia =>
                membresia.FechaVencimiento)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Membresia>> ObtenerPorClienteAsync(
        int clienteId)
    {
        return await _context.Membresias
            .Include(membresia => membresia.TipoMembresia)
            .Include(membresia => membresia.Precio)
            .AsNoTracking()
            .Where(membresia =>
                membresia.ClienteId == clienteId)
            .OrderByDescending(membresia =>
                membresia.FechaInicio)
            .ToListAsync();
    }

    public async Task AgregarAsync(
        Membresia membresia)
    {
        await _context.Membresias.AddAsync(membresia);

        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(
        Membresia membresia)
    {
        await _context.SaveChangesAsync();
    }
}