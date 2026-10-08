using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.src.BlaxtorGym.Application.Services;

public class TipoMembresiaService
{
    private readonly ITipoMembresiaRepository _tipoMembresiaRepository;

    public TipoMembresiaService(
        ITipoMembresiaRepository tipoMembresiaRepository)
    {
        _tipoMembresiaRepository = tipoMembresiaRepository;
    }

    public async Task<List<TipoMembresia>> ObtenerTodosAsync()
    {
        return await _tipoMembresiaRepository.ObtenerTodosAsync();
    }

    public async Task<TipoMembresia?> ObtenerPorIdAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        return await _tipoMembresiaRepository.ObtenerPorIdAsync(id);
    }

    public async Task CrearAsync(
        string nombre,
        int duracionDias,
        string descripcion)
    {
        nombre = nombre.Trim();
        descripcion = descripcion.Trim();

        ValidarDatos(
            nombre,
            duracionDias);

        bool existe =
            await _tipoMembresiaRepository
                .ExisteNombreAsync(nombre);

        if (existe)
        {
            throw new InvalidOperationException(
                "Ya existe un tipo de membresía con ese nombre.");
        }

        var tipoMembresia = new TipoMembresia
        {
            Nombre = nombre,
            DuracionDias = duracionDias,
            Descripcion = descripcion,
            Activo = true
        };

        await _tipoMembresiaRepository
            .AgregarAsync(tipoMembresia);
    }

    public async Task ModificarAsync(
        int id,
        string nombre,
        int duracionDias,
        string descripcion)
    {
        nombre = nombre.Trim();
        descripcion = descripcion.Trim();

        ValidarDatos(
            nombre,
            duracionDias);

        TipoMembresia? tipoMembresia =
            await _tipoMembresiaRepository
                .ObtenerPorIdAsync(id);

        if (tipoMembresia is null)
        {
            throw new InvalidOperationException(
                "El tipo de membresía no existe.");
        }

        bool existe =
            await _tipoMembresiaRepository
                .ExisteNombreAsync(
                    nombre,
                    id);

        if (existe)
        {
            throw new InvalidOperationException(
                "Ya existe otro tipo de membresía con ese nombre.");
        }

        tipoMembresia.Nombre = nombre;
        tipoMembresia.DuracionDias = duracionDias;
        tipoMembresia.Descripcion = descripcion;

        await _tipoMembresiaRepository
            .ActualizarAsync(tipoMembresia);
    }

    public async Task CambiarEstadoAsync(
        int id,
        bool activo)
    {
        TipoMembresia? tipoMembresia =
            await _tipoMembresiaRepository
                .ObtenerPorIdAsync(id);

        if (tipoMembresia is null)
        {
            throw new InvalidOperationException(
                "El tipo de membresía no existe.");
        }

        tipoMembresia.Activo = activo;

        await _tipoMembresiaRepository
            .ActualizarAsync(tipoMembresia);
    }

    private static void ValidarDatos(
        string nombre,
        int duracionDias)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException(
                "El nombre del tipo de membresía es obligatorio.");
        }

        if (nombre.Length > 50)
        {
            throw new ArgumentException(
                "El nombre del tipo de membresía no puede superar los 50 caracteres.");
        }

        if (duracionDias <= 0)
        {
            throw new ArgumentException(
                "La duración debe ser mayor a 0 días.");
        }

        if (duracionDias > 365)
        {
            throw new ArgumentException(
                "La duración no puede superar los 365 días.");
        }
    }
}