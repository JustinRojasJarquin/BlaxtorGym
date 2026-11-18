using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.Tests.Fakes;

namespace BlaxtorGym.Tests.Services;

[TestClass] public class TipoClienteServiceTests
{
    private FakeTipoClienteRepository _repository = null!; private TipoClienteService _service = null!;
    [TestInitialize] public void Inicializar() { _repository = new(); _service = new(_repository); }
    [TestMethod] public async Task CrearAsync_DatosValidos_CreaTipoClienteActivo() { await _service.CrearAsync("  Regular ", "  Descripción "); Assert.AreEqual("Regular", _repository.Agregado!.Nombre); Assert.IsTrue(_repository.Agregado.Activo); }
    [TestMethod] public async Task CrearAsync_NombreDuplicado_LanzaExcepcion() { _repository.Tipos.Add(new TipoCliente { Id = 1, Nombre = "Regular" }); await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.CrearAsync("Regular", "x")); }
    [TestMethod] public async Task CrearAsync_NombreVacio_LanzaExcepcion() { await Assert.ThrowsExactlyAsync<ArgumentException>(() => _service.CrearAsync(" ", "x")); }
    [TestMethod] public async Task ModificarAsync_Existente_ActualizaDatos() { _repository.Tipos.Add(new TipoCliente { Id = 2, Nombre = "Viejo" }); await _service.ModificarAsync(2, "Nuevo", "Desc"); Assert.AreEqual("Nuevo", _repository.Actualizado!.Nombre); }
    [TestMethod] public async Task CambiarEstadoAsync_Inexistente_LanzaExcepcion() { await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => _service.CambiarEstadoAsync(99, false)); }
}
