using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.Tests.Fakes;

namespace BlaxtorGym.Tests.Services;

[TestClass] public class TipoMembresiaServiceTests
{
    private FakeTipoMembresiaRepository _repository = null!; private TipoMembresiaService _service = null!;
    [TestInitialize] public void Inicializar() { _repository = new(); _service = new(_repository); }
    [TestMethod] public async Task CrearAsync_DatosValidos_CreaTipoMembresiaActiva() { await _service.CrearAsync("  Mensual ", 30, "  Plan "); Assert.AreEqual("Mensual", _repository.Agregado!.Nombre); Assert.AreEqual(30, _repository.Agregado.DuracionDias); }
    [TestMethod] public async Task CrearAsync_DuracionFueraDeRango_LanzaExcepcion() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.CrearAsync("Mensual", 366, "")); }
    [TestMethod] public async Task CrearAsync_NombreDuplicado_LanzaExcepcion() { _repository.Tipos.Add(new TipoMembresia { Id = 1, Nombre = "Mensual" }); await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.CrearAsync("Mensual", 30, "")); }
    [TestMethod] public async Task ModificarAsync_Existente_ActualizaDuracion() { _repository.Tipos.Add(new TipoMembresia { Id = 3, Nombre = "Mensual", DuracionDias = 30 }); await _service.ModificarAsync(3, "Anual", 365, "x"); Assert.AreEqual(365, _repository.Actualizado!.DuracionDias); }
    [TestMethod] public async Task ObtenerPorIdAsync_IdInvalido_RetornaNulo() { Assert.IsNull(await _service.ObtenerPorIdAsync(-1)); }
}
