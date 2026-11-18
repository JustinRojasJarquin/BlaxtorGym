using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.Tests.Fakes;

namespace BlaxtorGym.Tests.Services;

[TestClass] public class PagoServiceTests
{
    private FakePagoRepository _pagos = null!; private FakeMembresiaRepository _membresias = null!; private PagoService _service = null!;
    [TestInitialize] public void Inicializar() { _pagos = new(); _membresias = new(); _service = new(_pagos, _membresias, new BlaxtorGym.src.BlaxtorGym.Application.Services.BitacoraService(new FakeBitacoraRepository()), new BlaxtorGym.src.BlaxtorGym.Application.Services.SesionService()); _membresias.Membresias.Add(new Membresia { Id = 7, PrecioAplicado = 300m }); }
    [TestMethod] public async Task RegistrarAsync_PagoValido_CreaPagoConMontoPendiente() { var fecha = new DateTime(2026, 1, 15, 18, 0, 0); _pagos.TotalPagado = 100m; await _service.RegistrarAsync(7, "  Efectivo ", "  Abono  ", fecha); Assert.AreEqual(200m, _pagos.Agregado!.Monto); Assert.AreEqual("Efectivo", _pagos.Agregado.MetodoPago); Assert.AreEqual("Abono", _pagos.Agregado.Observacion); Assert.AreEqual(fecha.Date, _pagos.Agregado.FechaPago); }
    [TestMethod] public async Task RegistrarAsync_MembresiaInexistente_LanzaExcepcion() { await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.RegistrarAsync(99, "Efectivo", null, DateTime.Today)); }
    [TestMethod] public async Task RegistrarAsync_MembresiaPagada_LanzaExcepcion() { _pagos.TotalPagado = 300m; await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.RegistrarAsync(7, "Efectivo", null, DateTime.Today)); }
    [TestMethod] public async Task RegistrarAsync_MetodoVacio_LanzaExcepcion() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.RegistrarAsync(7, " ", null, DateTime.Today)); }
    [TestMethod] public async Task RegistrarAsync_FechaFutura_LanzaExcepcion() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.RegistrarAsync(7, "Efectivo", null, DateTime.Today.AddDays(1))); }
    [TestMethod] public async Task RegistrarAsync_FechaAnteriorAl2000_LanzaExcepcion() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.RegistrarAsync(7, "Efectivo", null, new DateTime(1999, 12, 31))); }
    [TestMethod] public async Task ObtenerPorRangoFechas_RangoInvertido_LanzaExcepcion() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.ObtenerPorRangoFechasAsync(DateTime.Today, DateTime.Today.AddDays(-1))); }
    [TestMethod] public async Task ObtenerPorMembresiaAsync_IdInvalido_RetornaListaVacia() { Assert.IsEmpty(await _service.ObtenerPorMembresiaAsync(0)); }
    [TestMethod] public async Task ObtenerPorClienteAsync_IdInvalido_RetornaListaVacia() { Assert.IsEmpty(await _service.ObtenerPorClienteAsync(0)); }
}
