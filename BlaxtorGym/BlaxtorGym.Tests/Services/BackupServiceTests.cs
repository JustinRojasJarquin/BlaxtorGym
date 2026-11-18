using BlaxtorGym.src.BlaxtorGym.Infraestructure.Services;
using Microsoft.Data.Sqlite;
namespace BlaxtorGym.Tests.Services;
[TestClass] public class BackupServiceTests
{
    private string _directorio = null!;
    [TestInitialize] public void Inicializar() { _directorio = Path.Combine(Path.GetTempPath(), "BlaxtorGym.Tests", Guid.NewGuid().ToString("N")); Directory.CreateDirectory(_directorio); }
    [TestCleanup] public void Limpiar() { if (Directory.Exists(_directorio)) Directory.Delete(_directorio, true); }
    [TestMethod] public async Task VerificarIntegridadAsync_BaseSqliteValida_RetornaVerdadero() { var ruta = Path.Combine(_directorio, "valida.db"); await using (var conexion = new SqliteConnection($"Data Source={ruta};Pooling=False")) { await conexion.OpenAsync(); await using var comando = conexion.CreateCommand(); comando.CommandText = "CREATE TABLE Prueba (Id INTEGER PRIMARY KEY);"; await comando.ExecuteNonQueryAsync(); } SqliteConnection.ClearAllPools(); var service = new BackupService(); Assert.IsTrue(await service.VerificarIntegridadAsync(ruta)); SqliteConnection.ClearAllPools(); }
    [TestMethod] public async Task VerificarIntegridadAsync_ArchivoCorrupto_RetornaFalso() { var ruta = Path.Combine(_directorio, "corrupta.db"); await File.WriteAllTextAsync(ruta, "no es sqlite"); var service = new BackupService(); Assert.IsFalse(await service.VerificarIntegridadAsync(ruta)); }
    [TestMethod] public async Task VerificarIntegridadAsync_ArchivoInexistente_RetornaFalso() { var service = new BackupService(); Assert.IsFalse(await service.VerificarIntegridadAsync(Path.Combine(_directorio, "no-existe.db"))); }
    [TestMethod] public async Task VerificarIntegridadAsync_RutaVacia_RetornaFalso() { var service = new BackupService(); Assert.IsFalse(await service.VerificarIntegridadAsync(" ")); }
    [TestMethod] public async Task CrearBackupEnRutaAsync_RutaVacia_RechazaAntesDeAccederABase() { var service = new BackupService(); await Assert.ThrowsExactlyAsync<ArgumentException>(() => service.CrearBackupEnRutaAsync(" ")); }
    [TestMethod] public async Task EliminarBackupAsync_ArchivoInexistente_LanzaExcepcion() { var service = new BackupService(); await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => service.EliminarBackupAsync(Path.Combine(_directorio, "no-existe.db"))); }
    [TestMethod] public async Task PrepararRestauracionAsync_ArchivoInexistente_LanzaExcepcion() { var service = new BackupService(); await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => service.PrepararRestauracionAsync(Path.Combine(_directorio, "no-existe.db"))); }
}
