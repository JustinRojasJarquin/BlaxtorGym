using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Enums;
using BlaxtorGym.Tests.Fakes;

namespace BlaxtorGym.Tests.Services;

[TestClass]
public class AutenticacionServiceTests
{
    private const string ContrasenaCorrecta =
        "ClaveCorrecta123*";

    private FakeUsuarioRepository _usuarioRepository = null!;

    private AutenticacionService _autenticacionService = null!;

    [TestInitialize]
    public void Inicializar()
    {
        _usuarioRepository =
            new FakeUsuarioRepository();

        _autenticacionService =
            new AutenticacionService(
                _usuarioRepository);
    }

    [TestMethod]
    public async Task AutenticarAsync_CredencialesCorrectas_LoginExitoso()
    {
        _usuarioRepository.Usuario =
            CrearUsuario();

        var solicitud = new LoginRequestDto
        {
            NombreUsuario = "admin",
            Contrasena = ContrasenaCorrecta
        };

        ResultadoLogin resultado =
            await _autenticacionService
                .AutenticarAsync(solicitud);

        Assert.IsTrue(resultado.Exitoso);

        Assert.IsNotNull(resultado.Usuario);

        Assert.AreEqual(
            "admin",
            resultado.Usuario.NombreUsuario);

        Assert.AreEqual(
            "Inicio de sesión exitoso.",
            resultado.Mensaje);
    }

    [TestMethod]
    public async Task AutenticarAsync_UsuarioInexistente_LoginFallido()
    {
        _usuarioRepository.Usuario = null;

        var solicitud = new LoginRequestDto
        {
            NombreUsuario = "noexiste",
            Contrasena = "CualquierClave123*"
        };

        ResultadoLogin resultado =
            await _autenticacionService
                .AutenticarAsync(solicitud);

        Assert.IsFalse(resultado.Exitoso);

        Assert.IsNull(resultado.Usuario);

        Assert.AreEqual(
            "El usuario no existe.",
            resultado.Mensaje);
    }

    [TestMethod]
    public async Task AutenticarAsync_ContrasenaIncorrecta_IncrementaIntentos()
    {
        Usuario usuario =
            CrearUsuario();

        _usuarioRepository.Usuario =
            usuario;

        var solicitud = new LoginRequestDto
        {
            NombreUsuario = "admin",
            Contrasena = "Incorrecta123*"
        };

        ResultadoLogin resultado =
            await _autenticacionService
                .AutenticarAsync(solicitud);

        Assert.IsFalse(resultado.Exitoso);

        Assert.AreEqual(
            1,
            usuario.IntentosFallidos);

        Assert.AreEqual(
            1,
            _usuarioRepository.CantidadActualizaciones);

        Assert.AreEqual(
            "Contraseña incorrecta. Intentos restantes: 2.",
            resultado.Mensaje);
    }

    [TestMethod]
    public async Task AutenticarAsync_UsuarioInactivo_LoginFallido()
    {
        Usuario usuario =
            CrearUsuario();

        usuario.Activo = false;

        _usuarioRepository.Usuario =
            usuario;

        var solicitud = new LoginRequestDto
        {
            NombreUsuario = "admin",
            Contrasena = ContrasenaCorrecta
        };

        ResultadoLogin resultado =
            await _autenticacionService
                .AutenticarAsync(solicitud);

        Assert.IsFalse(resultado.Exitoso);

        Assert.IsNull(resultado.Usuario);

        Assert.AreEqual(
            "El usuario se encuentra inactivo.",
            resultado.Mensaje);
    }

    [TestMethod]
    public async Task AutenticarAsync_TercerIntentoFallido_BloqueaUsuario()
    {
        Usuario usuario =
            CrearUsuario();

        usuario.IntentosFallidos = 2;

        _usuarioRepository.Usuario =
            usuario;

        var solicitud = new LoginRequestDto
        {
            NombreUsuario = "admin",
            Contrasena = "Incorrecta123*"
        };

        ResultadoLogin resultado =
            await _autenticacionService
                .AutenticarAsync(solicitud);

        Assert.IsFalse(resultado.Exitoso);

        Assert.AreEqual(
            0,
            usuario.IntentosFallidos);

        Assert.IsNotNull(
            usuario.BloqueadoHasta);

        Assert.IsTrue(
            usuario.BloqueadoHasta > DateTime.Now);

        Assert.AreEqual(
            "Usuario bloqueado temporalmente por demasiados intentos fallidos.",
            resultado.Mensaje);
    }

    [TestMethod]
    public async Task AutenticarAsync_UsuarioBloqueado_NoPermiteLogin()
    {
        Usuario usuario =
            CrearUsuario();

        usuario.BloqueadoHasta =
            DateTime.Now.AddMinutes(3);

        _usuarioRepository.Usuario =
            usuario;

        var solicitud = new LoginRequestDto
        {
            NombreUsuario = "admin",
            Contrasena = ContrasenaCorrecta
        };

        ResultadoLogin resultado =
            await _autenticacionService
                .AutenticarAsync(solicitud);

        Assert.IsFalse(resultado.Exitoso);

        Assert.IsNull(resultado.Usuario);

        Assert.AreEqual(
            "El usuario se encuentra temporalmente bloqueado.",
            resultado.Mensaje);
    }

    [TestMethod]
    public async Task AutenticarAsync_LoginCorrecto_ReiniciaIntentosFallidos()
    {
        Usuario usuario =
            CrearUsuario();

        usuario.IntentosFallidos = 2;

        _usuarioRepository.Usuario =
            usuario;

        var solicitud = new LoginRequestDto
        {
            NombreUsuario = "admin",
            Contrasena = ContrasenaCorrecta
        };

        ResultadoLogin resultado =
            await _autenticacionService
                .AutenticarAsync(solicitud);

        Assert.IsTrue(resultado.Exitoso);

        Assert.AreEqual(
            0,
            usuario.IntentosFallidos);

        Assert.IsNull(
            usuario.BloqueadoHasta);

        Assert.AreEqual(
            1,
            _usuarioRepository.CantidadActualizaciones);
    }

    private static Usuario CrearUsuario()
    {
        return new Usuario
        {
            Id = 1,

            NombreUsuario = "admin",

            ContrasenaHash =
                BCrypt.Net.BCrypt.HashPassword(
                    ContrasenaCorrecta),

            NombreCompleto =
                "Administrador",

            Rol =
                RolUsuario.Administrador,

            Activo = true,

            IntentosFallidos = 0,

            BloqueadoHasta = null
        };
    }
}