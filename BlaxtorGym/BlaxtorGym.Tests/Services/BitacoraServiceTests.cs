using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.Tests.Fakes;
namespace BlaxtorGym.Tests.Services;
[TestClass] public class BitacoraServiceTests
{
    private FakeBitacoraRepository _repository = null!; private BitacoraService _service = null!;
    [TestInitialize] public void Inicializar() { _repository = new(); _service = new(_repository); }
    [TestMethod] public async Task RegistrarAsync_DatosValidos_RegistraNormalizado() { await _service.RegistrarAsync(7, " crear ", " Cliente ", 4, " descripción "); var registro = _repository.Registros.Single(); Assert.AreEqual(7, registro.UsuarioId); Assert.AreEqual("CREAR", registro.Accion); Assert.AreEqual("Cliente", registro.Entidad); Assert.AreEqual(4, registro.EntidadId); Assert.AreEqual("descripción", registro.Descripcion); }
    [TestMethod] public async Task RegistrarAsync_UsuarioInvalido_Rechaza() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.RegistrarAsync(0, "CREAR", "Cliente", null, null)); }
    [TestMethod] public async Task RegistrarAsync_AccionVacia_Rechaza() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.RegistrarAsync(1, " ", "Cliente", null, null)); }
    [TestMethod] public async Task RegistrarAsync_EntidadVacia_Rechaza() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.RegistrarAsync(1, "CREAR", " ", null, null)); }
    [TestMethod] public async Task RegistrarAsync_DatosOpcionalesNulos_PermiteRegistro() { await _service.RegistrarAsync(1, "CREAR", "Cliente", null, null); Assert.IsNull(_repository.Registros[0].EntidadId); Assert.IsNull(_repository.Registros[0].Descripcion); }
    [TestMethod] public async Task ObtenerAsync_FiltrosValidos_LosPropagaNormalizados() { var inicio = DateTime.Today.AddDays(-1); var fin = DateTime.Today; await _service.ObtenerAsync(inicio, fin, " crear ", " Cliente ", 8); Assert.AreEqual(inicio, _repository.FechaInicio); Assert.AreEqual(fin, _repository.FechaFin); Assert.AreEqual("CREAR", _repository.Accion); Assert.AreEqual("Cliente", _repository.Entidad); Assert.AreEqual(8, _repository.UsuarioId); }
    [TestMethod] public async Task ObtenerAsync_FiltrosTodas_ConvierteANull() { await _service.ObtenerAsync(null, null, "Todas", " todas ", null); Assert.IsNull(_repository.Accion); Assert.IsNull(_repository.Entidad); }
    [TestMethod] public async Task ObtenerAsync_RangoInvertido_Rechaza() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.ObtenerAsync(DateTime.Today, DateTime.Today.AddDays(-1), null, null, null)); }
    [TestMethod] public async Task ObtenerAsync_RegistroConUsuario_TransformaDto() { _repository.Registros.Add(new BitacoraCambio { Id = 3, UsuarioId = 2, Accion = "CREAR", Entidad = "Cliente", Usuario = new Usuario { NombreCompleto = "Ana" } }); var resultado = await _service.ObtenerAsync(null, null, null, null, null); Assert.AreEqual("Ana", resultado[0].Usuario); Assert.AreEqual("CREAR", resultado[0].Accion); }
}
