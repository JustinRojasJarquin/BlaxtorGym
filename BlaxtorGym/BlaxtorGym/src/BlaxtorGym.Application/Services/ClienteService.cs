using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace BlaxtorGym.src.BlaxtorGym.Application.Services;

public class ClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ITipoClienteRepository _tipoClienteRepository;

    public ClienteService(
        IClienteRepository clienteRepository,
        ITipoClienteRepository tipoClienteRepository)
    {
        _clienteRepository = clienteRepository;
        _tipoClienteRepository = tipoClienteRepository;
    }

    public async Task<List<Cliente>> BuscarAsync(
        string criterio)
    {
        return await _clienteRepository.BuscarAsync(
            criterio ?? string.Empty);
    }

    public async Task<List<Cliente>> ObtenerActivosAsync()
    {
        return await _clienteRepository.ObtenerActivosAsync();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        return await _clienteRepository.ObtenerPorIdAsync(id);
    }

    public async Task<Cliente?> ObtenerPorCedulaAsync(
        string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula))
        {
            return null;
        }

        return await _clienteRepository
            .ObtenerPorCedulaAsync(cedula.Trim());
    }

    public async Task CrearAsync(
        string cedula,
        string nombreCompleto,
        string telefono,
        string correoElectronico,
        DateTime? fechaNacimiento,
        int tipoClienteId)
    {
        cedula = NormalizarTexto(cedula);
        nombreCompleto = NormalizarTexto(nombreCompleto);
        telefono = NormalizarTexto(telefono);
        correoElectronico = NormalizarTexto(correoElectronico);

        ValidarCedula(cedula);
        ValidarNombreCompleto(nombreCompleto);
        ValidarTelefono(telefono);
        ValidarCorreo(correoElectronico);
        ValidarFechaNacimiento(fechaNacimiento);

        TipoCliente tipoCliente =
            await ObtenerTipoClienteValidoAsync(
                tipoClienteId,
                exigirActivo: true);

        if (await _clienteRepository
            .ExisteCedulaAsync(cedula))
        {
            throw new InvalidOperationException(
                "Ya existe un cliente con esa cédula.");
        }

        var cliente = new Cliente
        {
            Cedula = cedula,
            NombreCompleto = nombreCompleto,
            Telefono = telefono,
            CorreoElectronico = correoElectronico,
            FechaNacimiento = fechaNacimiento!.Value,
            FechaRegistro = DateTime.Now,
            Activo = true,
            TipoClienteId = tipoCliente.Id
        };

        await _clienteRepository.AgregarAsync(cliente);
    }

    public async Task ModificarAsync(
        int id,
        string cedula,
        string nombreCompleto,
        string telefono,
        string correoElectronico,
        DateTime? fechaNacimiento,
        int tipoClienteId)
    {
        cedula = NormalizarTexto(cedula);
        nombreCompleto = NormalizarTexto(nombreCompleto);
        telefono = NormalizarTexto(telefono);
        correoElectronico = NormalizarTexto(correoElectronico);

        ValidarCedula(cedula);
        ValidarNombreCompleto(nombreCompleto);
        ValidarTelefono(telefono);
        ValidarCorreo(correoElectronico);
        ValidarFechaNacimiento(fechaNacimiento);

        Cliente? cliente =
            await _clienteRepository.ObtenerPorIdAsync(id);

        if (cliente is null)
        {
            throw new InvalidOperationException(
                "El cliente no existe.");
        }

        TipoCliente tipoCliente =
            await ObtenerTipoClienteValidoAsync(
                tipoClienteId,
                exigirActivo:
                    tipoClienteId != cliente.TipoClienteId);

        if (await _clienteRepository
            .ExisteCedulaAsync(cedula, id))
        {
            throw new InvalidOperationException(
                "Ya existe otro cliente con esa cédula.");
        }

        cliente.Cedula = cedula;
        cliente.NombreCompleto = nombreCompleto;
        cliente.Telefono = telefono;
        cliente.CorreoElectronico = correoElectronico;
        cliente.FechaNacimiento = fechaNacimiento!.Value;
        cliente.TipoClienteId = tipoCliente.Id;

        await _clienteRepository.ActualizarAsync(cliente);
    }

    public async Task CambiarEstadoAsync(
        int id,
        bool activo)
    {
        Cliente? cliente =
            await _clienteRepository.ObtenerPorIdAsync(id);

        if (cliente is null)
        {
            throw new InvalidOperationException(
                "El cliente no existe.");
        }

        cliente.Activo = activo;

        await _clienteRepository.ActualizarAsync(cliente);
    }

    private async Task<TipoCliente>
        ObtenerTipoClienteValidoAsync(
            int tipoClienteId,
            bool exigirActivo)
    {
        if (tipoClienteId <= 0)
        {
            throw new ArgumentException(
                "Debe seleccionar un tipo de cliente.");
        }

        TipoCliente? tipoCliente =
            await _tipoClienteRepository
                .ObtenerPorIdAsync(tipoClienteId);

        if (tipoCliente is null)
        {
            throw new InvalidOperationException(
                "El tipo de cliente seleccionado no existe.");
        }

        if (exigirActivo && !tipoCliente.Activo)
        {
            throw new InvalidOperationException(
                "El tipo de cliente seleccionado está inactivo.");
        }

        return tipoCliente;
    }

    private static string NormalizarTexto(string valor)
    {
        return valor?.Trim() ?? string.Empty;
    }

    private static void ValidarCedula(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula))
        {
            throw new ArgumentException(
                "La cédula es obligatoria.");
        }

        if (cedula.Length > 20)
        {
            throw new ArgumentException(
                "La cédula no puede superar los 20 caracteres.");
        }
    }

    private static void ValidarNombreCompleto(
        string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException(
                "El nombre completo es obligatorio.");
        }

        if (nombre.Length > 120)
        {
            throw new ArgumentException(
                "El nombre completo no puede superar los 120 caracteres.");
        }
    }

    private static void ValidarTelefono(
        string telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
        {
            throw new ArgumentException(
                "El teléfono es obligatorio.");
        }

        if (telefono.Length > 20)
        {
            throw new ArgumentException(
                "El teléfono no puede superar los 20 caracteres.");
        }
    }

    private static void ValidarCorreo(
        string correo)
    {
        if (string.IsNullOrWhiteSpace(correo))
        {
            throw new ArgumentException(
                "El correo electrónico es obligatorio.");
        }

        if (correo.Length > 150)
        {
            throw new ArgumentException(
                "El correo electrónico no puede superar los 150 caracteres.");
        }

        const string patron =
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        if (!Regex.IsMatch(correo, patron))
        {
            throw new ArgumentException(
                "El correo electrónico no tiene un formato válido.");
        }
    }

    private static void ValidarFechaNacimiento(
        DateTime? fechaNacimiento)
    {
        if (!fechaNacimiento.HasValue)
        {
            throw new ArgumentException(
                "La fecha de nacimiento es obligatoria.");
        }

        if (fechaNacimiento.Value.Date >
            DateTime.Today)
        {
            throw new ArgumentException(
                "La fecha de nacimiento no puede ser futura.");
        }

        if (fechaNacimiento.Value.Year < 1900)
        {
            throw new ArgumentException(
                "La fecha de nacimiento no es válida.");
        }
    }
}