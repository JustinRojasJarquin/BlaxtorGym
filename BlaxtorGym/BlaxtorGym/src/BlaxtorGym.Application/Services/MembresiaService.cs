using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.src.BlaxtorGym.Application.Services;

public class MembresiaService
{
    private readonly IMembresiaRepository _membresiaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly ITipoMembresiaRepository _tipoMembresiaRepository;
    private readonly IPrecioRepository _precioRepository;
    private readonly BitacoraService _bitacoraService;
    private readonly SesionService _sesionService;

    public MembresiaService(
        IMembresiaRepository membresiaRepository,
        IClienteRepository clienteRepository,
        ITipoMembresiaRepository tipoMembresiaRepository,
        IPrecioRepository precioRepository,
        BitacoraService bitacoraService,
        SesionService sesionService)
    {
        _membresiaRepository = membresiaRepository;
        _clienteRepository = clienteRepository;
        _tipoMembresiaRepository = tipoMembresiaRepository;
        _precioRepository = precioRepository;
        _bitacoraService = bitacoraService;
        _sesionService = sesionService;
    }

    public async Task<List<Membresia>> ObtenerTodasAsync()
    {
        return await _membresiaRepository.ObtenerTodasAsync();
    }

    public async Task<List<Membresia>> ObtenerPorClienteAsync(
        int clienteId)
    {
        if (clienteId <= 0)
        {
            return new List<Membresia>();
        }

        return await _membresiaRepository
            .ObtenerPorClienteAsync(clienteId);
    }

    public async Task CrearAsync(
        int clienteId,
        int tipoMembresiaId,
        DateTime fechaInicio)
    {
        Cliente? cliente =
            await _clienteRepository
                .ObtenerPorIdAsync(clienteId);

        if (cliente is null)
        {
            throw new InvalidOperationException(
                "El cliente seleccionado no existe.");
        }

        if (!cliente.Activo)
        {
            throw new InvalidOperationException(
                "El cliente seleccionado se encuentra inactivo.");
        }

        TipoMembresia? tipoMembresia =
            await _tipoMembresiaRepository
                .ObtenerPorIdAsync(tipoMembresiaId);

        if (tipoMembresia is null)
        {
            throw new InvalidOperationException(
                "El tipo de membresía seleccionado no existe.");
        }

        if (!tipoMembresia.Activo)
        {
            throw new InvalidOperationException(
                "El tipo de membresía seleccionado se encuentra inactivo.");
        }

        Membresia? membresiaAnterior =
            await _membresiaRepository
                .ObtenerActivaPorClienteAsync(clienteId);

        if (membresiaAnterior is not null)
        {
            if (membresiaAnterior.FechaVencimiento.Date >=
                fechaInicio.Date)
            {
                throw new InvalidOperationException(
                    "El cliente ya tiene una membresía activa para esa fecha.");
            }

            /*
             * La membresía anterior ya venció antes de la
             * nueva fecha de inicio.
             *
             * La marcamos como inactiva para evitar mantener
             * dos registros con Activa = true.
             */
            Membresia? membresiaAnteriorSeguimiento =
                await _membresiaRepository
                    .ObtenerPorIdAsync(membresiaAnterior.Id);

            if (membresiaAnteriorSeguimiento is not null)
            {
                membresiaAnteriorSeguimiento.Activa = false;

                await _membresiaRepository
                    .ActualizarAsync(
                        membresiaAnteriorSeguimiento);
            }
        }

        Precio? precio =
            await _precioRepository
                .ObtenerActivoPorCombinacionAsync(
                    cliente.TipoClienteId,
                    tipoMembresia.Id);

        if (precio is null)
        {
            throw new InvalidOperationException(
                "No existe una tarifa activa para este tipo de cliente y membresía.");
        }

        DateTime inicio =
            fechaInicio.Date;

        DateTime vencimiento =
            inicio.AddDays(
                tipoMembresia.DuracionDias - 1);

        var membresia = new Membresia
        {
            FechaInicio = inicio,
            FechaVencimiento = vencimiento,
            PrecioAplicado = precio.Monto,
            Activa = true,
            ClienteId = cliente.Id,
            TipoMembresiaId = tipoMembresia.Id,
            PrecioId = precio.Id
        };

        await _membresiaRepository
            .AgregarAsync(membresia);
        await RegistrarBitacoraAsync("CREAR", membresia.Id, $"Se creó una membresía para el cliente {cliente.NombreCompleto}.");
    }

    public async Task RenovarAsync(
        int membresiaId,
        int tipoMembresiaId)
    {
        Membresia? membresiaAnterior =
            await _membresiaRepository
                .ObtenerPorIdAsync(membresiaId);

        if (membresiaAnterior is null)
        {
            throw new InvalidOperationException(
                "La membresía seleccionada no existe.");
        }

        Cliente? cliente =
            await _clienteRepository
                .ObtenerPorIdAsync(
                    membresiaAnterior.ClienteId);

        if (cliente is null)
        {
            throw new InvalidOperationException(
                "El cliente de la membresía no existe.");
        }

        if (!cliente.Activo)
        {
            throw new InvalidOperationException(
                "El cliente se encuentra inactivo.");
        }

        TipoMembresia? tipoMembresia =
            await _tipoMembresiaRepository
                .ObtenerPorIdAsync(tipoMembresiaId);

        if (tipoMembresia is null)
        {
            throw new InvalidOperationException(
                "El tipo de membresía seleccionado no existe.");
        }

        if (!tipoMembresia.Activo)
        {
            throw new InvalidOperationException(
                "El tipo de membresía seleccionado se encuentra inactivo.");
        }

        Membresia? otraMembresiaActiva =
            await _membresiaRepository
                .ObtenerActivaPorClienteAsync(
                    membresiaAnterior.ClienteId);

        if (otraMembresiaActiva is not null &&
            otraMembresiaActiva.Id != membresiaAnterior.Id &&
            otraMembresiaActiva.FechaVencimiento.Date >=
            DateTime.Today)
        {
            throw new InvalidOperationException(
                "El cliente ya posee otra membresía vigente.");
        }

        Precio? precio =
            await _precioRepository
                .ObtenerActivoPorCombinacionAsync(
                    cliente.TipoClienteId,
                    tipoMembresia.Id);

        if (precio is null)
        {
            throw new InvalidOperationException(
                "No existe una tarifa activa para este tipo de cliente y membresía.");
        }

        DateTime hoy =
            DateTime.Today;

        DateTime nuevaFechaInicio;

        if (membresiaAnterior.FechaVencimiento.Date >= hoy)
        {
            /*
             * Si todavía está vigente, la renovación comienza
             * al día siguiente de su vencimiento.
             */
            nuevaFechaInicio =
                membresiaAnterior
                    .FechaVencimiento
                    .Date
                    .AddDays(1);
        }
        else
        {
            /*
             * Si ya venció, la renovación comienza hoy.
             * Así evitamos crear una renovación que nazca vencida.
             */
            nuevaFechaInicio = hoy;
        }

        DateTime nuevaFechaVencimiento =
            nuevaFechaInicio.AddDays(
                tipoMembresia.DuracionDias - 1);

        membresiaAnterior.Activa = false;

        await _membresiaRepository
            .ActualizarAsync(membresiaAnterior);

        var nuevaMembresia = new Membresia
        {
            FechaInicio = nuevaFechaInicio,
            FechaVencimiento = nuevaFechaVencimiento,
            PrecioAplicado = precio.Monto,
            Activa = true,
            ClienteId = cliente.Id,
            TipoMembresiaId = tipoMembresia.Id,
            PrecioId = precio.Id
        };

        await _membresiaRepository
            .AgregarAsync(nuevaMembresia);
        await RegistrarBitacoraAsync("RENOVAR", nuevaMembresia.Id, $"Se renovó la membresía del cliente {cliente.NombreCompleto}.");
    }

    public async Task DesactivarAsync(
        int membresiaId)
    {
        Membresia? membresia =
            await _membresiaRepository
                .ObtenerPorIdAsync(membresiaId);

        if (membresia is null)
        {
            throw new InvalidOperationException(
                "La membresía seleccionada no existe.");
        }

        if (!membresia.Activa)
        {
            throw new InvalidOperationException(
                "La membresía ya se encuentra inactiva.");
        }

        membresia.Activa = false;

        await _membresiaRepository
            .ActualizarAsync(membresia);
        await RegistrarBitacoraAsync("DESACTIVAR", membresia.Id, $"Se desactivó la membresía del cliente {membresia.Cliente?.NombreCompleto ?? membresia.ClienteId.ToString()}.");
    }

    private async Task RegistrarBitacoraAsync(string accion, int entidadId, string descripcion)
    {
        if (_sesionService.UsuarioId is int usuarioId)
            await _bitacoraService.RegistrarAsync(usuarioId, accion, "Membresia", entidadId, descripcion);
    }
}
