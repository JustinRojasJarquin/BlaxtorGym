using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.Tests.Fakes;

namespace BlaxtorGym.Tests.Services;

[TestClass] public class TarifaServiceTests
{
    private FakePrecioRepository _precios = null!; private FakeTipoClienteRepository _clientes = null!; private FakeTipoMembresiaRepository _membresias = null!; private TarifaService _service = null!;
    [TestInitialize] public void Inicializar() { _precios = new(); _clientes = new(); _membresias = new(); _service = new(_precios, _clientes, _membresias, new BlaxtorGym.src.BlaxtorGym.Application.Services.BitacoraService(new FakeBitacoraRepository()), new BlaxtorGym.src.BlaxtorGym.Application.Services.SesionService()); _clientes.Tipos.Add(new TipoCliente { Id = 1, Activo = true }); _membresias.Tipos.Add(new TipoMembresia { Id = 2, Activo = true }); }
    [TestMethod] public async Task CrearAsync_DatosValidos_CreaTarifaActiva() { await _service.CrearAsync(1, 2, 150m); Assert.AreEqual(150m, _precios.Agregado!.Monto); Assert.IsTrue(_precios.Agregado.Activo); }
    [TestMethod] public async Task CrearAsync_MontoInvalido_LanzaExcepcion() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.CrearAsync(1, 2, 0m)); Assert.IsNull(_precios.Agregado); }
    [TestMethod] public async Task CrearAsync_CombinacionDuplicada_LanzaExcepcion() { _precios.Precios.Add(new Precio { Id = 5, TipoClienteId = 1, TipoMembresiaId = 2, Activo = true }); await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.CrearAsync(1, 2, 200m)); }
    [TestMethod] public async Task CrearAsync_TipoClienteInactivo_LanzaExcepcion() { _clientes.Tipos[0].Activo = false; await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.CrearAsync(1, 2, 100m)); }
    [TestMethod] public async Task CambiarTarifaAsync_TarifaActiva_CierraAnteriorYCreaNueva() { var anterior = new Precio { Id = 7, TipoClienteId = 1, TipoMembresiaId = 2, Monto = 100m, Activo = true }; _precios.Precios.Add(anterior); await _service.CambiarTarifaAsync(1, 2, 120m); Assert.IsFalse(_precios.Actualizado!.Activo); Assert.AreEqual(120m, _precios.Agregado!.Monto); }
    [TestMethod] public async Task CambiarTarifaAsync_MismoMonto_LanzaExcepcion() { _precios.Precios.Add(new Precio { Id = 7, TipoClienteId = 1, TipoMembresiaId = 2, Monto = 100m, Activo = true }); await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.CambiarTarifaAsync(1, 2, 100m)); }
    [TestMethod] public async Task DesactivarAsync_TarifaActiva_ActualizaEstado() { _precios.Precios.Add(new Precio { Id = 3, Activo = true }); await _service.DesactivarAsync(3); Assert.IsFalse(_precios.Actualizado!.Activo); Assert.IsNotNull(_precios.Actualizado.FechaFinVigencia); }
    [TestMethod] public async Task ObtenerPrecioActivoAsync_IdsInvalidos_RetornaNulo() { Assert.IsNull(await _service.ObtenerPrecioActivoAsync(0, 2)); }
}
