using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.src.BlaxtorGym.Application.Services;

public class AutenticacionService
{
    private const int MaximoIntentosFallidos = 3;
    private const int MinutosBloqueo = 5;

    private readonly IUsuarioRepository _usuarioRepository;

    public AutenticacionService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ResultadoLogin> AutenticarAsync(
        LoginRequestDto solicitud)
    {
        string nombreUsuario = solicitud.NombreUsuario.Trim();

        if (string.IsNullOrWhiteSpace(nombreUsuario) ||
            string.IsNullOrWhiteSpace(solicitud.Contrasena))
        {
            return CrearResultadoFallido(
                "Debe ingresar el usuario y la contraseña.");
        }

        var usuario = await _usuarioRepository
            .ObtenerPorNombreUsuarioAsync(nombreUsuario);

        if (usuario is null)
        {
            return CrearResultadoFallido(
                "El usuario no existe.");
        }

        if (!usuario.Activo)
        {
            return CrearResultadoFallido(
                "El usuario se encuentra inactivo.");
        }

        if (usuario.BloqueadoHasta.HasValue &&
            usuario.BloqueadoHasta.Value > DateTime.Now)
        {
            return CrearResultadoFallido(
                "El usuario se encuentra temporalmente bloqueado.");
        }

        bool contrasenaCorrecta = BCrypt.Net.BCrypt.Verify(
            solicitud.Contrasena,
            usuario.ContrasenaHash);

        if (!contrasenaCorrecta)
        {
            usuario.IntentosFallidos++;

            if (usuario.IntentosFallidos >= MaximoIntentosFallidos)
            {
                usuario.IntentosFallidos = 0;
                usuario.BloqueadoHasta =
                    DateTime.Now.AddMinutes(MinutosBloqueo);

                await _usuarioRepository.ActualizarAsync(usuario);

                return CrearResultadoFallido(
                    "Usuario bloqueado temporalmente por demasiados intentos fallidos.");
            }

            await _usuarioRepository.ActualizarAsync(usuario);

            int intentosRestantes =
                MaximoIntentosFallidos - usuario.IntentosFallidos;

            return CrearResultadoFallido(
                $"Contraseña incorrecta. Intentos restantes: {intentosRestantes}.");
        }

        usuario.IntentosFallidos = 0;
        usuario.BloqueadoHasta = null;

        await _usuarioRepository.ActualizarAsync(usuario);

        return new ResultadoLogin
        {
            Exitoso = true,
            Mensaje = "Inicio de sesión exitoso.",
            Usuario = usuario
        };
    }

    private static ResultadoLogin CrearResultadoFallido(string mensaje)
    {
        return new ResultadoLogin
        {
            Exitoso = false,
            Mensaje = mensaje,
            Usuario = null
        };
    }
}