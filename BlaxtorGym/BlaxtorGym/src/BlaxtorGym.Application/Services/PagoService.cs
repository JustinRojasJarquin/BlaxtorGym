using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.src.BlaxtorGym.Application.Services;

public class PagoService
{
    private readonly IPagoRepository _pagoRepository;
    private readonly IMembresiaRepository _membresiaRepository;

    public PagoService(
        IPagoRepository pagoRepository,
        IMembresiaRepository membresiaRepository)
    {
        _pagoRepository = pagoRepository;
        _membresiaRepository = membresiaRepository;
    }

    public async Task<List<Pago>> ObtenerTodosAsync()
    {
        return await _pagoRepository.ObtenerTodosAsync();
    }

    public async Task<List<Pago>> ObtenerPorMembresiaAsync(
        int membresiaId)
    {
        if (membresiaId <= 0)
        {
            return new List<Pago>();
        }

        return await _pagoRepository
            .ObtenerPorMembresiaAsync(membresiaId);
    }

    public async Task<List<Pago>> ObtenerPorClienteAsync(
        int clienteId)
    {
        if (clienteId <= 0)
        {
            return new List<Pago>();
        }

        return await _pagoRepository
            .ObtenerPorClienteAsync(clienteId);
    }

    public async Task<List<Pago>> ObtenerPorRangoFechasAsync(
        DateTime fechaInicio,
        DateTime fechaFin)
    {
        if (fechaInicio.Date > fechaFin.Date)
        {
            throw new ArgumentException(
                "La fecha inicial no puede ser posterior a la fecha final.");
        }

        return await _pagoRepository
            .ObtenerPorRangoFechasAsync(
                fechaInicio,
                fechaFin);
    }

    public async Task RegistrarAsync(
        int membresiaId,
        string metodoPago,
        string? observacion,
        DateTime fechaPago)
    {
        ValidarMetodoPago(metodoPago);
        ValidarFechaPago(fechaPago);

        Membresia? membresia =
            await _membresiaRepository
                .ObtenerPorIdAsync(membresiaId);

        if (membresia is null)
        {
            throw new InvalidOperationException(
                "La membresía seleccionada no existe.");
        }

        decimal totalPagado =
            await _pagoRepository
                .ObtenerTotalPagadoPorMembresiaAsync(
                    membresia.Id);

        if (totalPagado >= membresia.PrecioAplicado)
        {
            throw new InvalidOperationException(
                "La membresía ya se encuentra pagada.");
        }

        decimal montoPendiente =
            membresia.PrecioAplicado - totalPagado;

        var pago = new Pago
        {
            Monto = montoPendiente,
            FechaPago = fechaPago.Date,
            MetodoPago = metodoPago.Trim(),
            Observacion =
                string.IsNullOrWhiteSpace(observacion)
                    ? null
                    : observacion.Trim(),
            MembresiaId = membresia.Id
        };

        await _pagoRepository.AgregarAsync(pago);
    }

    private static void ValidarMetodoPago(
        string metodoPago)
    {
        if (string.IsNullOrWhiteSpace(metodoPago))
        {
            throw new ArgumentException(
                "Debe seleccionar o indicar un método de pago.");
        }

        if (metodoPago.Trim().Length > 50)
        {
            throw new ArgumentException(
                "El método de pago no puede superar los 50 caracteres.");
        }
    }

    private static void ValidarFechaPago(
        DateTime fechaPago)
    {
        if (fechaPago.Date > DateTime.Today)
        {
            throw new ArgumentException(
                "La fecha del pago no puede ser futura.");
        }

        if (fechaPago.Year < 2000)
        {
            throw new ArgumentException(
                "La fecha del pago no es válida.");
        }
    }
}