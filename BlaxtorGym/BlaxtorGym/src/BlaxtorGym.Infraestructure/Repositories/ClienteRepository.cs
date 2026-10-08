using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cliente>> ObtenerTodosAsync()
    {
        return await _context.Clientes
            .Include(cliente => cliente.TipoCliente)
            .AsNoTracking()
            .OrderBy(cliente => cliente.NombreCompleto)
            .ToListAsync();
    }

    public async Task<List<Cliente>> ObtenerActivosAsync()
    {
        return await _context.Clientes
            .Include(cliente => cliente.TipoCliente)
            .AsNoTracking()
            .Where(cliente => cliente.Activo)
            .OrderBy(cliente => cliente.NombreCompleto)
            .ToListAsync();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        return await _context.Clientes
            .Include(cliente => cliente.TipoCliente)
            .FirstOrDefaultAsync(cliente => cliente.Id == id);
    }

    public async Task<Cliente?> ObtenerPorCedulaAsync(string cedula)
    {
        string cedulaNormalizada = cedula.Trim();

        return await _context.Clientes
            .Include(cliente => cliente.TipoCliente)
            .FirstOrDefaultAsync(cliente =>
                cliente.Cedula == cedulaNormalizada);
    }

    public async Task<bool> ExisteCedulaAsync(
        string cedula,
        int? idExcluir = null)
    {
        string cedulaNormalizada = cedula.Trim();

        return await _context.Clientes
            .AnyAsync(cliente =>
                cliente.Cedula == cedulaNormalizada &&
                (!idExcluir.HasValue || cliente.Id != idExcluir.Value));
    }

    public async Task AgregarAsync(Cliente cliente)
    {
        await _context.Clientes.AddAsync(cliente);

        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);

        await _context.SaveChangesAsync();
    }
}
