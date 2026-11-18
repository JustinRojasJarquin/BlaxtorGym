using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.Tests.Fakes;
namespace BlaxtorGym.Tests.Services;
[TestClass] public class ReporteServiceTests
{
    private FakeReporteRepository _repository = null!; private ReporteService _service = null!;
    [TestInitialize] public void Inicializar() { _repository = new(); _service = new(_repository); }
    [TestMethod] public async Task GenerarAsync_DatosCompletos_CalculaTotales() { _repository.Clientes.AddRange(new List<ReporteClienteItemDto> { new() { Activo = true }, new() { Activo = false } }); _repository.Membresias.AddRange(new List<ReporteMembresiaItemDto> { new() { Estado = "Vigente" }, new() { Estado = "Vencida" }, new() { Estado = "Pendiente" }, new() { Estado = "Inactiva" } }); _repository.Pagos.AddRange(new List<ReportePagoItemDto> { new() { Monto = 100m }, new() { Monto = 250.50m } }); var resultado = await _service.GenerarAsync(new()); Assert.AreEqual(2, resultado.TotalClientes); Assert.AreEqual(1, resultado.ClientesActivos); Assert.AreEqual(1, resultado.ClientesInactivos); Assert.AreEqual(1, resultado.MembresiasVigentes); Assert.AreEqual(1, resultado.MembresiasVencidas); Assert.AreEqual(1, resultado.MembresiasPendientes); Assert.AreEqual(1, resultado.MembresiasInactivas); Assert.AreEqual(2, resultado.TotalPagos); Assert.AreEqual(350.50m, resultado.TotalIngresos); }
    [TestMethod] public async Task GenerarAsync_FiltroValido_PropagaElMismoFiltro() { var filtro = new ReporteFiltroDto { ClienteId = 4, TipoClienteId = 2, TipoMembresiaId = 3, MetodoPago = "Efectivo" }; await _service.GenerarAsync(filtro); Assert.AreSame(filtro, _repository.FiltroClientes); Assert.AreSame(filtro, _repository.FiltroMembresias); Assert.AreSame(filtro, _repository.FiltroPagos); Assert.AreSame(filtro, _repository.FiltroMediciones); }
    [TestMethod] public async Task GenerarAsync_FechaInicialPosterior_RechazaFiltro() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.GenerarAsync(new() { FechaInicio = DateTime.Today, FechaFin = DateTime.Today.AddDays(-1) })); }
    [TestMethod] public async Task GenerarAsync_ClienteIdInvalido_RechazaFiltro() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.GenerarAsync(new() { ClienteId = 0 })); }
    [TestMethod] public async Task GenerarAsync_TipoClienteIdInvalido_RechazaFiltro() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.GenerarAsync(new() { TipoClienteId = -1 })); }
    [TestMethod] public async Task GenerarAsync_TipoMembresiaIdInvalido_RechazaFiltro() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.GenerarAsync(new() { TipoMembresiaId = 0 })); }
    [TestMethod] public async Task GenerarAsync_ListasVacias_RetornaTotalesCero() { var resultado = await _service.GenerarAsync(new()); Assert.AreEqual(0, resultado.TotalClientes); Assert.AreEqual(0, resultado.TotalPagos); Assert.AreEqual(0m, resultado.TotalIngresos); }
}
