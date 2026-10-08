using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.src.BlaxtorGym.Application.Services;

public class ClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<List<Cliente>> ObtenerTodosAsync()
    {
        return await _clienteRepository.ObtenerTodosAsync();
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

    public async Task<Cliente?> ObtenerPorCedulaAsync(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula))
        {
            return null;
        }

        return await _clienteRepository.ObtenerPorCedulaAsync(cedula);
    }

    public async Task CrearAsync(
        string cedula,
        string nombreCompleto,
        string telefono,
        string correoElectronico,
        DateTime fechaNacimiento,
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

        if (tipoClienteId <= 0)
        {
            throw new ArgumentException(
                "Debe seleccionar un tipo de cliente.");
        }

        if (await _clienteRepository.ExisteCedulaAsync(cedula))
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
            FechaNacimiento = fechaNacimiento,
            FechaRegistro = DateTime.Now,
            Activo = true,
            TipoClienteId = tipoClienteId
        };

        await _clienteRepository.AgregarAsync(cliente);
    }

    public async Task ModificarAsync(
        int id,
        string cedula,
        string nombreCompleto,
        string telefono,
        string correoElectronico,
        DateTime fechaNacimiento,
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

        if (tipoClienteId <= 0)
        {
            throw new ArgumentException(
                "Debe seleccionar un tipo de cliente.");
        }

        Cliente? cliente = await _clienteRepository.ObtenerPorIdAsync(id);

        if (cliente is null)
        {
            throw new InvalidOperationException(
                "El cliente no existe.");
        }

        if (await _clienteRepository.ExisteCedulaAsync(cedula, id))
        {
            throw new InvalidOperationException(
                "Ya existe otro cliente con esa cédula.");
        }

        cliente.Cedula = cedula;
        cliente.NombreCompleto = nombreCompleto;
        cliente.Telefono = telefono;
        cliente.CorreoElectronico = correoElectronico;
        cliente.FechaNacimiento = fechaNacimiento;
        cliente.TipoClienteId = tipoClienteId;

        await _clienteRepository.ActualizarAsync(cliente);
    }

    public async Task CambiarEstadoAsync(int id, bool activo)
    {
        Cliente? cliente = await _clienteRepository.ObtenerPorIdAsync(id);

        if (cliente is null)
        {
            throw new InvalidOperationException(
                "El cliente no existe.");
        }

        cliente.Activo = activo;

        await _clienteRepository.ActualizarAsync(cliente);
    }

    private static string NormalizarTexto(string valor)
    {
        return valor.Trim();
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

    private static void ValidarNombreCompleto(string nombre)
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

    private static void ValidarTelefono(string telefono)
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

    private static void ValidarCorreo(string correo)
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

        if (!correo.Contains('@'))
        {
            throw new ArgumentException(
                "El correo electrónico no tiene un formato válido.");
        }
    }

    private static void ValidarFechaNacimiento(DateTime fechaNacimiento)
    {
        if (fechaNacimiento == default)
        {
            throw new ArgumentException(
                "La fecha de nacimiento es obligatoria.");
        }

        if (fechaNacimiento > DateTime.Today)
        {
            throw new ArgumentException(
                "La fecha de nacimiento no puede ser futura.");
        }
    }
}
