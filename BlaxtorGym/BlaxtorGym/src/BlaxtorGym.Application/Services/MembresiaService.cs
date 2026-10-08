using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.src.BlaxtorGym.Application.Services;

public class MembresiaService
{
    private readonly IMembresiaRepository _membresiaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly ITipoMembresiaRepository _tipoMembresiaRepository;
    private readonly IPrecioRepository _precioRepository;

    public MembresiaService(
        IMembresiaRepository membresiaRepository,
        IClienteRepository clienteRepository,
        ITipoMembresiaRepository tipoMembresiaRepository,
        IPrecioRepository precioRepository)
    {
        _membresiaRepository = membresiaRepository;
        _clienteRepository = clienteRepository;
        _tipoMembresiaRepository = tipoMembresiaRepository;
        _precioRepository = precioRepository;
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
        Cliente cliente =
            await ObtenerClienteValidoAsync(clienteId);

        TipoMembresia tipoMembresia =
            await ObtenerTipoMembresiaValidoAsync(
                tipoMembresiaId);

        Membresia? membresiaActiva =
            await _membresiaRepository
                .ObtenerActivaPorClienteAsync(cliente.Id);

        if (membresiaActiva is not null &&
            membresiaActiva.FechaVencimiento.Date >=
            fechaInicio.Date)
        {
            throw new InvalidOperationException(
                "El cliente ya tiene una membresía activa.");
        }

        Precio? precio =
            await _precioRepository
                .ObtenerActivoPorCombinacionAsync(
                    cliente.TipoClienteId,
                    tipoMembresia.Id);

        if (precio is null)
        {
            throw new InvalidOperationException(
                "No existe una tarifa activa para el tipo de cliente y la membresía seleccionados.");
        }

        DateTime fechaVencimiento =
            CalcularFechaVencimiento(
                fechaInicio,
                tipoMembresia.DuracionDias);

        var membresia = new Membresia
        {
            FechaInicio = fechaInicio.Date,
            FechaVencimiento = fechaVencimiento,
            PrecioAplicado = precio.Monto,
            Activa = true,
            ClienteId = cliente.Id,
            TipoMembresiaId = tipoMembresia.Id,
            PrecioId = precio.Id
        };

        await _membresiaRepository
            .AgregarAsync(membresia);
    }

    public async Task RenovarAsync(
        int membresiaId,
        int tipoMembresiaId)
    {
        Membresia? membresiaActual =
            await _membresiaRepository
                .ObtenerPorIdAsync(membresiaId);

        if (membresiaActual is null)
        {
            throw new InvalidOperationException(
                "La membresía no existe.");
        }

        Cliente cliente =
            await ObtenerClienteValidoAsync(
                membresiaActual.ClienteId);

        TipoMembresia tipoMembresia =
            await ObtenerTipoMembresiaValidoAsync(
                tipoMembresiaId);

        Precio? precio =
            await _precioRepository
                .ObtenerActivoPorCombinacionAsync(
                    cliente.TipoClienteId,
                    tipoMembresia.Id);

        if (precio is null)
        {
            throw new InvalidOperationException(
                "No existe una tarifa activa para la renovación seleccionada.");
        }

        membresiaActual.Activa = false;

        await _membresiaRepository
            .ActualizarAsync(membresiaActual);

        DateTime nuevaFechaInicio =
            membresiaActual.FechaVencimiento.Date.AddDays(1);

        DateTime nuevaFechaVencimiento =
            CalcularFechaVencimiento(
                nuevaFechaInicio,
                tipoMembresia.DuracionDias);

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
    }

    public async Task DesactivarAsync(int id)
    {
        Membresia? membresia =
            await _membresiaRepository.ObtenerPorIdAsync(id);

        if (membresia is null)
        {
            throw new InvalidOperationException(
                "La membresía no existe.");
        }

        if (!membresia.Activa)
        {
            throw new InvalidOperationException(
                "La membresía ya se encuentra inactiva.");
        }

        membresia.Activa = false;

        await _membresiaRepository
            .ActualizarAsync(membresia);
    }

    private async Task<Cliente>
        ObtenerClienteValidoAsync(int clienteId)
    {
        if (clienteId <= 0)
        {
            throw new ArgumentException(
                "Debe seleccionar un cliente.");
        }

        Cliente? cliente =
            await _clienteRepository.ObtenerPorIdAsync(
                clienteId);

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

        return cliente;
    }

    private async Task<TipoMembresia>
        ObtenerTipoMembresiaValidoAsync(
            int tipoMembresiaId)
    {
        if (tipoMembresiaId <= 0)
        {
            throw new ArgumentException(
                "Debe seleccionar un tipo de membresía.");
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

        return tipoMembresia;
    }

    private static DateTime CalcularFechaVencimiento(
        DateTime fechaInicio,
        int duracionDias)
    {
        return fechaInicio.Date
            .AddDays(duracionDias - 1);
    }
}