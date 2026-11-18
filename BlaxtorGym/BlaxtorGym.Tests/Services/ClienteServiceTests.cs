using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.Tests.Fakes;

namespace BlaxtorGym.Tests.Services;

[TestClass]
public class ClienteServiceTests
{
    private FakeClienteRepository _clientes = null!;
    private FakeTipoClienteRepository _tipos = null!;
    private ClienteService _service = null!;

    [TestInitialize] public void Inicializar() { _clientes = new(); _tipos = new(); _service = new(_clientes, _tipos, new BlaxtorGym.src.BlaxtorGym.Application.Services.BitacoraService(new FakeBitacoraRepository()), new BlaxtorGym.src.BlaxtorGym.Application.Services.SesionService()); }

    [TestMethod]
    public async Task CrearAsync_DatosValidos_CreaClienteNormalizado()
    {
        _tipos.Tipos.Add(new TipoCliente { Id = 3, Nombre = "Regular", Activo = true });
        await _service.CrearAsync("  123  ", "  Ana Pérez  ", " 555  ", " ana@correo.com ", new DateTime(1990, 1, 1), 3);
        Assert.IsNotNull(_clientes.Agregado); Assert.AreEqual("123", _clientes.Agregado.Cedula); Assert.AreEqual("Ana Pérez", _clientes.Agregado.NombreCompleto); Assert.AreEqual(3, _clientes.Agregado.TipoClienteId);
    }

    [TestMethod] public async Task CrearAsync_CedulaDuplicada_LanzaExcepcion() { _tipos.Tipos.Add(new TipoCliente { Id = 1, Activo = true }); _clientes.Clientes.Add(new Cliente { Id = 4, Cedula = "123" }); await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.CrearAsync("123", "Ana", "555", "a@b.com", new(1990, 1, 1), 1)); }
    [TestMethod] public async Task CrearAsync_FechaFutura_RechazaDatos() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.CrearAsync("123", "Ana", "555", "a@b.com", DateTime.Today.AddDays(1), 1)); Assert.IsNull(_clientes.Agregado); }
    [TestMethod] public async Task CrearAsync_TipoInactivo_RechazaDatos() { _tipos.Tipos.Add(new TipoCliente { Id = 1, Activo = false }); await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.CrearAsync("123", "Ana", "555", "a@b.com", new(1990, 1, 1), 1)); }
    [TestMethod] public async Task ObtenerPorIdAsync_IdInvalido_RetornaNulo() { Assert.IsNull(await _service.ObtenerPorIdAsync(0)); }
    [TestMethod] public async Task BuscarAsync_CriterioNulo_UsaCriterioVacio() { _clientes.Clientes.Add(new Cliente { Id = 1, Cedula = "123", NombreCompleto = "Ana" }); var resultado = await _service.BuscarAsync(null!); Assert.HasCount(1, resultado); }
    [TestMethod] public async Task CambiarEstadoAsync_ClienteExistente_ActualizaEstado() { _clientes.Clientes.Add(new Cliente { Id = 2, Activo = true }); await _service.CambiarEstadoAsync(2, false); Assert.IsFalse(_clientes.Actualizado!.Activo); }
    [TestMethod] public async Task ModificarAsync_ClienteInexistente_LanzaExcepcion() { await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.ModificarAsync(9, "123", "Ana", "555", "a@b.com", new(1990, 1, 1), 1)); }
}
