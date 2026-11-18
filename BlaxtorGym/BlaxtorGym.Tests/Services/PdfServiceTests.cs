using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Services;
using QuestPDF.Infrastructure;
namespace BlaxtorGym.Tests.Services;
[TestClass] public class PdfServiceTests
{
    private string _directorio = null!;
    [ClassInitialize]
    public static void ConfigurarQuestPdf(TestContext context) => QuestPDF.Settings.License = LicenseType.Community;
    [TestInitialize] public void Inicializar() { _directorio = Path.Combine(Path.GetTempPath(), "BlaxtorGym.Tests", Guid.NewGuid().ToString("N")); Directory.CreateDirectory(_directorio); }
    [TestCleanup] public void Limpiar() { if (Directory.Exists(_directorio)) Directory.Delete(_directorio, true); }
    [TestMethod] public async Task GenerarReporteAsync_ReporteGeneralValido_CreaPdfNoVacio() { var ruta = Path.Combine(_directorio, "general.pdf"); await new PdfService().GenerarAsync(Reporte(), "Reporte general", ruta); Assert.IsTrue(File.Exists(ruta)); Assert.IsGreaterThan(0, new FileInfo(ruta).Length); Assert.IsTrue((await File.ReadAllBytesAsync(ruta)).Take(4).SequenceEqual(new byte[] { 0x25, 0x50, 0x44, 0x46 })); }
    [TestMethod] public async Task GenerarReporteAsync_Clientes_CreaPdfValido() { await GenerarYValidarAsync("clientes", "Reporte de clientes", r => r.Clientes.Add(new ReporteClienteItemDto { NombreCompleto = "Ana" })); }
    [TestMethod] public async Task GenerarReporteAsync_Membresias_CreaPdfValido() { await GenerarYValidarAsync("membresias", "Reporte de membresías", r => r.Membresias.Add(new ReporteMembresiaItemDto { Cliente = "Ana", TipoMembresia = "Mensual", Estado = "Vigente" })); }
    [TestMethod] public async Task GenerarReporteAsync_Pagos_CreaPdfValido() { await GenerarYValidarAsync("pagos", "Reporte de pagos e ingresos", r => r.Pagos.Add(new ReportePagoItemDto { Cliente = "Ana", Monto = 100m })); }
    [TestMethod] public async Task GenerarReporteAsync_Mediciones_CreaPdfValido() { await GenerarYValidarAsync("mediciones", "Reporte de mediciones físicas", r => r.Mediciones.Add(new ReporteMedicionItemDto { Cliente = "Ana", PesoKg = 70m })); }
    [TestMethod] public async Task GenerarReporteAsync_SinRegistros_GeneraPdfValido() { var ruta = Path.Combine(_directorio, "vacio.pdf"); await new PdfService().GenerarAsync(new ReporteGeneralDto(), "Reporte general", ruta); Assert.IsTrue(File.Exists(ruta)); Assert.IsGreaterThan(0, new FileInfo(ruta).Length); }
    [TestMethod] public async Task GenerarAsync_ReporteNulo_LanzaExcepcion() { await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => new PdfService().GenerarAsync(null!, "Reporte", Path.Combine(_directorio, "x.pdf"))); }
    private async Task GenerarYValidarAsync(string nombre, string titulo, Action<ReporteGeneralDto> preparar) { var reporte = Reporte(); preparar(reporte); var ruta = Path.Combine(_directorio, nombre + ".pdf"); await new PdfService().GenerarAsync(reporte, titulo, ruta); Assert.IsTrue(File.Exists(ruta)); Assert.IsGreaterThan(0, new FileInfo(ruta).Length); }
    private static ReporteGeneralDto Reporte() => new() { FechaGeneracion = DateTime.Now };
}
