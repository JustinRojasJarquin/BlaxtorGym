using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.Tests.Fakes;

namespace BlaxtorGym.Tests.Services;

[TestClass] public class MembresiaServiceTests
{
    private FakeMembresiaRepository _membresias = null!; private FakeClienteRepository _clientes = null!; private FakeTipoMembresiaRepository _tipos = null!; private FakePrecioRepository _precios = null!; private MembresiaService _service = null!;
    [TestInitialize] public void Inicializar() { _membresias = new(); _clientes = new(); _tipos = new(); _precios = new(); _service = new(_membresias, _clientes, _tipos, _precios, new BlaxtorGym.src.BlaxtorGym.Application.Services.BitacoraService(new FakeBitacoraRepository()), new BlaxtorGym.src.BlaxtorGym.Application.Services.SesionService()); _clientes.Clientes.Add(new Cliente { Id = 1, Activo = true, TipoClienteId = 2 }); _tipos.Tipos.Add(new TipoMembresia { Id = 3, Activo = true, DuracionDias = 30 }); _precios.Precios.Add(new Precio { Id = 4, Activo = true, Monto = 250m, TipoClienteId = 2, TipoMembresiaId = 3 }); }
    [TestMethod] public async Task CrearAsync_DatosValidos_CreaMembresia() { var inicio = new DateTime(2026, 1, 10, 15, 0, 0); await _service.CrearAsync(1, 3, inicio); Assert.AreEqual(new DateTime(2026, 1, 10), _membresias.Agregada!.FechaInicio); Assert.AreEqual(new DateTime(2026, 2, 8), _membresias.Agregada.FechaVencimiento); Assert.AreEqual(250m, _membresias.Agregada.PrecioAplicado); Assert.IsTrue(_membresias.Agregada.Activa); }
    [TestMethod] public async Task CrearAsync_ClienteInexistente_LanzaExcepcion() { await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.CrearAsync(99, 3, DateTime.Today)); }
    [TestMethod] public async Task CrearAsync_ClienteInactivo_LanzaExcepcion() { _clientes.Clientes[0].Activo = false; await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.CrearAsync(1, 3, DateTime.Today)); }
    [TestMethod] public async Task CrearAsync_TipoInactivo_LanzaExcepcion() { _tipos.Tipos[0].Activo = false; await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.CrearAsync(1, 3, DateTime.Today)); }
    [TestMethod] public async Task CrearAsync_TarifaInexistente_LanzaExcepcion() { _precios.Precios.Clear(); await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.CrearAsync(1, 3, DateTime.Today)); }
    [TestMethod] public async Task CrearAsync_MembresiaActivaVigente_LanzaExcepcion() { _membresias.Membresias.Add(new Membresia { Id = 5, ClienteId = 1, FechaVencimiento = DateTime.Today.AddDays(2), Activa = true }); await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.CrearAsync(1, 3, DateTime.Today)); }
    [TestMethod] public async Task CrearAsync_MembresiaAnteriorVencida_DesactivaAnteriorYCreaNueva() { var anterior = new Membresia { Id = 5, ClienteId = 1, FechaVencimiento = DateTime.Today.AddDays(-1), Activa = true }; _membresias.Membresias.Add(anterior); await _service.CrearAsync(1, 3, DateTime.Today); Assert.IsFalse(_membresias.Actualizada!.Activa); Assert.IsTrue(_membresias.Agregada!.Activa); }
    [TestMethod] public async Task RenovarAsync_MembresiaVencida_CreaNuevaDesdeHoy() { _membresias.Membresias.Add(new Membresia { Id = 5, ClienteId = 1, FechaVencimiento = DateTime.Today.AddDays(-1), Activa = true }); await _service.RenovarAsync(5, 3); Assert.IsFalse(_membresias.Actualizada!.Activa); Assert.AreEqual(DateTime.Today, _membresias.Agregada!.FechaInicio); }
    [TestMethod] public async Task DesactivarAsync_MembresiaActiva_ActualizaEstado() { _membresias.Membresias.Add(new Membresia { Id = 5, Activa = true }); await _service.DesactivarAsync(5); Assert.IsFalse(_membresias.Actualizada!.Activa); }
    [TestMethod] public async Task ObtenerPorClienteAsync_IdInvalido_RetornaListaVacia() { var resultado = await _service.ObtenerPorClienteAsync(0); Assert.IsEmpty(resultado); }
    [TestMethod] public void Estado_MembresiaPendiente_RetornaPendiente() { var membresia = new Membresia { Activa = true, FechaInicio = DateTime.Today.AddDays(1), FechaVencimiento = DateTime.Today.AddDays(30) }; Assert.AreEqual("Pendiente", membresia.Estado); }
    [TestMethod] public void Estado_MembresiaVencida_RetornaVencida() { var membresia = new Membresia { Activa = true, FechaInicio = DateTime.Today.AddDays(-30), FechaVencimiento = DateTime.Today.AddDays(-1) }; Assert.AreEqual("Vencida", membresia.Estado); }
}
