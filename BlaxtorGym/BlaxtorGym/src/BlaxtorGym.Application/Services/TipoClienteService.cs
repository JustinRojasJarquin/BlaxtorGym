using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.src.BlaxtorGym.Application.Services;

public class TipoClienteService
{
    private readonly ITipoClienteRepository _tipoClienteRepository;

    public TipoClienteService(
        ITipoClienteRepository tipoClienteRepository)
    {
        _tipoClienteRepository = tipoClienteRepository;
    }

    public async Task<List<TipoCliente>> ObtenerTodosAsync()
    {
        return await _tipoClienteRepository.ObtenerTodosAsync();
    }

    public async Task<TipoCliente?> ObtenerPorIdAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        return await _tipoClienteRepository.ObtenerPorIdAsync(id);
    }

    public async Task CrearAsync(
        string nombre,
        string descripcion)
    {
        nombre = nombre.Trim();
        descripcion = descripcion.Trim();

        ValidarNombre(nombre);

        bool existe = await _tipoClienteRepository
            .ExisteNombreAsync(nombre);

        if (existe)
        {
            throw new InvalidOperationException(
                "Ya existe un tipo de cliente con ese nombre.");
        }

        var tipoCliente = new TipoCliente
        {
            Nombre = nombre,
            Descripcion = descripcion,
            Activo = true
        };

        await _tipoClienteRepository.AgregarAsync(tipoCliente);
    }

    public async Task ModificarAsync(
        int id,
        string nombre,
        string descripcion)
    {
        nombre = nombre.Trim();
        descripcion = descripcion.Trim();

        ValidarNombre(nombre);

        TipoCliente? tipoCliente =
            await _tipoClienteRepository.ObtenerPorIdAsync(id);

        if (tipoCliente is null)
        {
            throw new InvalidOperationException(
                "El tipo de cliente no existe.");
        }

        bool existe = await _tipoClienteRepository
            .ExisteNombreAsync(nombre, id);

        if (existe)
        {
            throw new InvalidOperationException(
                "Ya existe otro tipo de cliente con ese nombre.");
        }

        tipoCliente.Nombre = nombre;
        tipoCliente.Descripcion = descripcion;

        await _tipoClienteRepository.ActualizarAsync(tipoCliente);
    }

    public async Task CambiarEstadoAsync(
        int id,
        bool activo)
    {
        TipoCliente? tipoCliente =
            await _tipoClienteRepository.ObtenerPorIdAsync(id);

        if (tipoCliente is null)
        {
            throw new InvalidOperationException(
                "El tipo de cliente no existe.");
        }

        tipoCliente.Activo = activo;

        await _tipoClienteRepository.ActualizarAsync(tipoCliente);
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException(
                "El nombre del tipo de cliente es obligatorio.");
        }

        if (nombre.Length > 50)
        {
            throw new ArgumentException(
                "El nombre del tipo de cliente no puede superar los 50 caracteres.");
        }
    }
}