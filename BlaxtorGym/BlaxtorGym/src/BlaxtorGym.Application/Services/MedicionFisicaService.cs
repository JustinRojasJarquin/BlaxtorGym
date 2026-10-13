using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.src.BlaxtorGym.Application.Services;

public class MedicionFisicaService
{
    private const decimal PesoMinimoKg = 1m;
    private const decimal PesoMaximoKg = 500m;
    private const decimal EstaturaMinimaCm = 50m;
    private const decimal EstaturaMaximaCm = 300m;
    private const decimal PorcentajeGrasaMinimo = 0m;
    private const decimal PorcentajeGrasaMaximo = 100m;
    private const int ObservacionMaxima = 500;

    private readonly IMedicionFisicaRepository _medicionRepository;
    private readonly IClienteRepository _clienteRepository;

    public MedicionFisicaService(
        IMedicionFisicaRepository medicionRepository,
        IClienteRepository clienteRepository)
    {
        _medicionRepository = medicionRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<List<MedicionFisica>> ObtenerHistorialPorClienteAsync(
        int clienteId)
    {
        if (clienteId <= 0)
        {
            throw new ArgumentException(
                "Debe seleccionar un cliente.",
                nameof(clienteId));
        }

        return await _medicionRepository
            .ObtenerPorClienteAsync(clienteId);
    }

    public async Task<MedicionFisica> RegistrarAsync(
        int clienteId,
        DateTime fechaMedicion,
        decimal pesoKg,
        decimal? estaturaCm,
        decimal? porcentajeGrasa,
        string? observacion)
    {
        Cliente cliente =
            await ObtenerClienteActivoAsync(clienteId);

        ValidarFecha(fechaMedicion);
        ValidarPeso(pesoKg);

        if (estaturaCm.HasValue)
        {
            ValidarEstatura(estaturaCm.Value);
        }

        if (porcentajeGrasa.HasValue)
        {
            ValidarPorcentajeGrasa(porcentajeGrasa.Value);
        }

        if ((observacion?.Trim().Length ?? 0) > ObservacionMaxima)
        {
            throw new ArgumentException(
                $"La observación no puede superar los {ObservacionMaxima} caracteres.",
                nameof(observacion));
        }

        var medicion = new MedicionFisica
        {
            ClienteId = cliente.Id,
            FechaMedicion = fechaMedicion.Date,
            PesoKg = pesoKg,
            EstaturaCm = estaturaCm,
            PorcentajeGrasa = porcentajeGrasa,
            Observacion = string.IsNullOrWhiteSpace(observacion)
                ? null
                : observacion.Trim()
        };

        await _medicionRepository.AgregarAsync(medicion);

        return medicion;
    }

    private async Task<Cliente> ObtenerClienteActivoAsync(int clienteId)
    {
        if (clienteId <= 0)
        {
            throw new ArgumentException(
                "Debe seleccionar un cliente.",
                nameof(clienteId));
        }

        Cliente? cliente =
            await _clienteRepository.ObtenerPorIdAsync(clienteId);

        if (cliente is null)
        {
            throw new InvalidOperationException(
                "El cliente seleccionado no existe.");
        }

        if (!cliente.Activo)
        {
            throw new InvalidOperationException(
                "No se pueden registrar mediciones para un cliente inactivo.");
        }

        return cliente;
    }

    private static void ValidarFecha(DateTime fechaMedicion)
    {
        if (fechaMedicion == default ||
            fechaMedicion.Year < 1900)
        {
            throw new ArgumentException(
                "La fecha de medición no es válida.",
                nameof(fechaMedicion));
        }

        if (fechaMedicion.Date > DateTime.Today)
        {
            throw new ArgumentException(
                "La fecha de medición no puede ser futura.",
                nameof(fechaMedicion));
        }
    }

    private static void ValidarPeso(decimal pesoKg)
    {
        ValidarDosDecimales(pesoKg, nameof(pesoKg));

        if (pesoKg < PesoMinimoKg ||
            pesoKg > PesoMaximoKg)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pesoKg),
                $"El peso debe estar entre {PesoMinimoKg} y {PesoMaximoKg} kg.");
        }
    }

    private static void ValidarEstatura(decimal estaturaCm)
    {
        ValidarDosDecimales(estaturaCm, nameof(estaturaCm));

        if (estaturaCm < EstaturaMinimaCm ||
            estaturaCm > EstaturaMaximaCm)
        {
            throw new ArgumentOutOfRangeException(
                nameof(estaturaCm),
                $"La estatura debe estar entre {EstaturaMinimaCm} y {EstaturaMaximaCm} cm.");
        }
    }

    private static void ValidarPorcentajeGrasa(decimal porcentajeGrasa)
    {
        ValidarDosDecimales(porcentajeGrasa, nameof(porcentajeGrasa));

        if (porcentajeGrasa < PorcentajeGrasaMinimo ||
            porcentajeGrasa > PorcentajeGrasaMaximo)
        {
            throw new ArgumentOutOfRangeException(
                nameof(porcentajeGrasa),
                "El porcentaje de grasa debe estar entre 0 y 100.");
        }
    }

    private static void ValidarDosDecimales(
        decimal valor,
        string nombreParametro)
    {
        if (decimal.Round(valor, 2) != valor)
        {
            throw new ArgumentException(
                "Las mediciones admiten un máximo de dos decimales.",
                nombreParametro);
        }
    }
}
