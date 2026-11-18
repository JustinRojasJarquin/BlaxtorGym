using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.src.BlaxtorGym.Application.Services;

public class TarifaService
{
    private readonly IPrecioRepository _precioRepository;
    private readonly ITipoClienteRepository _tipoClienteRepository;
    private readonly ITipoMembresiaRepository _tipoMembresiaRepository;
    private readonly BitacoraService _bitacoraService;
    private readonly SesionService _sesionService;

    public TarifaService(
        IPrecioRepository precioRepository,
        ITipoClienteRepository tipoClienteRepository,
        ITipoMembresiaRepository tipoMembresiaRepository,
        BitacoraService bitacoraService,
        SesionService sesionService)
    {
        _precioRepository = precioRepository;
        _tipoClienteRepository = tipoClienteRepository;
        _tipoMembresiaRepository = tipoMembresiaRepository;
        _bitacoraService = bitacoraService;
        _sesionService = sesionService;
    }

    public async Task<List<Precio>> ObtenerTodosAsync()
    {
        return await _precioRepository.ObtenerTodosAsync();
    }

    public async Task<Precio?> ObtenerPorIdAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        return await _precioRepository.ObtenerPorIdAsync(id);
    }

    public async Task<Precio?> ObtenerPrecioActivoAsync(
        int tipoClienteId,
        int tipoMembresiaId)
    {
        if (tipoClienteId <= 0 || tipoMembresiaId <= 0)
        {
            return null;
        }

        return await _precioRepository
            .ObtenerActivoPorCombinacionAsync(
                tipoClienteId,
                tipoMembresiaId);
    }

    public async Task CrearAsync(
        int tipoClienteId,
        int tipoMembresiaId,
        decimal monto)
    {
        ValidarMonto(monto);

        TipoCliente tipoCliente =
            await ObtenerTipoClienteValidoAsync(
                tipoClienteId);

        TipoMembresia tipoMembresia =
            await ObtenerTipoMembresiaValidoAsync(
                tipoMembresiaId);

        bool existePrecioActivo =
            await _precioRepository
                .ExistePrecioActivoAsync(
                    tipoCliente.Id,
                    tipoMembresia.Id);

        if (existePrecioActivo)
        {
            throw new InvalidOperationException(
                "Ya existe una tarifa activa para esta combinación.");
        }

        var precio = new Precio
        {
            Monto = monto,
            FechaInicioVigencia = DateTime.Now,
            FechaFinVigencia = null,
            Activo = true,
            TipoClienteId = tipoCliente.Id,
            TipoMembresiaId = tipoMembresia.Id
        };

        await _precioRepository.AgregarAsync(precio);
        await RegistrarBitacoraAsync("CREAR", precio.Id, $"Se creó la tarifa {tipoCliente.Nombre} - {tipoMembresia.Nombre} por ₡{precio.Monto:N2}.");
    }

    public async Task CambiarTarifaAsync(
        int tipoClienteId,
        int tipoMembresiaId,
        decimal nuevoMonto)
    {
        ValidarMonto(nuevoMonto);

        TipoCliente tipoCliente =
            await ObtenerTipoClienteValidoAsync(
                tipoClienteId);

        TipoMembresia tipoMembresia =
            await ObtenerTipoMembresiaValidoAsync(
                tipoMembresiaId);

        Precio? precioActual =
            await _precioRepository
                .ObtenerActivoPorCombinacionAsync(
                    tipoCliente.Id,
                    tipoMembresia.Id);

        if (precioActual is null)
        {
            throw new InvalidOperationException(
                "No existe una tarifa activa para esta combinación.");
        }

        Precio? precioRastreado =
            await _precioRepository
                .ObtenerPorIdAsync(precioActual.Id);

        if (precioRastreado is null)
        {
            throw new InvalidOperationException(
                "La tarifa actual no pudo ser localizada.");
        }

        if (precioRastreado.Monto == nuevoMonto)
        {
            throw new InvalidOperationException(
                "El nuevo monto debe ser diferente al monto actual.");
        }

        precioRastreado.Activo = false;
        precioRastreado.FechaFinVigencia = DateTime.Now;

        await _precioRepository
            .ActualizarAsync(precioRastreado);

        var nuevoPrecio = new Precio
        {
            Monto = nuevoMonto,
            FechaInicioVigencia = DateTime.Now,
            FechaFinVigencia = null,
            Activo = true,
            TipoClienteId = tipoCliente.Id,
            TipoMembresiaId = tipoMembresia.Id
        };

        await _precioRepository
            .AgregarAsync(nuevoPrecio);
        await RegistrarBitacoraAsync("EDITAR", nuevoPrecio.Id, $"Se modificó la tarifa {tipoCliente.Nombre} - {tipoMembresia.Nombre} a ₡{nuevoPrecio.Monto:N2}.");
    }

    public async Task DesactivarAsync(int id)
    {
        Precio? precio =
            await _precioRepository.ObtenerPorIdAsync(id);

        if (precio is null)
        {
            throw new InvalidOperationException(
                "La tarifa no existe.");
        }

        if (!precio.Activo)
        {
            throw new InvalidOperationException(
                "La tarifa ya se encuentra inactiva.");
        }

        precio.Activo = false;
        precio.FechaFinVigencia = DateTime.Now;

        await _precioRepository.ActualizarAsync(precio);
        await RegistrarBitacoraAsync("DESACTIVAR", precio.Id, $"Se desactivó la tarifa por ₡{precio.Monto:N2}.");
    }

    private async Task RegistrarBitacoraAsync(string accion, int entidadId, string descripcion)
    {
        if (_sesionService.UsuarioId is int usuarioId)
            await _bitacoraService.RegistrarAsync(usuarioId, accion, "Precio", entidadId, descripcion);
    }

    private async Task<TipoCliente>
        ObtenerTipoClienteValidoAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                "Debe seleccionar un tipo de cliente.");
        }

        TipoCliente? tipoCliente =
            await _tipoClienteRepository.ObtenerPorIdAsync(id);

        if (tipoCliente is null)
        {
            throw new InvalidOperationException(
                "El tipo de cliente seleccionado no existe.");
        }

        if (!tipoCliente.Activo)
        {
            throw new InvalidOperationException(
                "El tipo de cliente seleccionado está inactivo.");
        }

        return tipoCliente;
    }

    private async Task<TipoMembresia>
        ObtenerTipoMembresiaValidoAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                "Debe seleccionar un tipo de membresía.");
        }

        TipoMembresia? tipoMembresia =
            await _tipoMembresiaRepository.ObtenerPorIdAsync(id);

        if (tipoMembresia is null)
        {
            throw new InvalidOperationException(
                "El tipo de membresía seleccionado no existe.");
        }

        if (!tipoMembresia.Activo)
        {
            throw new InvalidOperationException(
                "El tipo de membresía seleccionado está inactivo.");
        }

        return tipoMembresia;
    }

    private static void ValidarMonto(decimal monto)
    {
        if (monto <= 0)
        {
            throw new ArgumentException(
                "El monto de la tarifa debe ser mayor a cero.");
        }

        if (monto > 10_000_000)
        {
            throw new ArgumentException(
                "El monto de la tarifa no es válido.");
        }
    }
}
