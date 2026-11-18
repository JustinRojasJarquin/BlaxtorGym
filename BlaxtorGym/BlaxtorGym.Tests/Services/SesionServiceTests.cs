using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
namespace BlaxtorGym.Tests.Services;
[TestClass] public class SesionServiceTests
{
    [TestMethod] public void NuevaSesion_SinUsuario_NoEstaActiva() { var service = new SesionService(); Assert.IsFalse(service.HaySesionActiva); Assert.IsNull(service.UsuarioActual); Assert.IsNull(service.UsuarioId); Assert.AreEqual(string.Empty, service.NombreUsuario); }
    [TestMethod] public void IniciarSesion_UsuarioValido_ConservaDatosReales() { var service = new SesionService(); service.IniciarSesion(new Usuario { Id = 17, NombreUsuario = "juan" }); Assert.IsTrue(service.HaySesionActiva); Assert.AreEqual(17, service.UsuarioId); Assert.AreEqual("juan", service.NombreUsuario); }
    [TestMethod] public void CerrarSesion_SesionActiva_LimpiaUsuario() { var service = new SesionService(); service.IniciarSesion(new Usuario { Id = 17, NombreUsuario = "juan" }); service.CerrarSesion(); Assert.IsFalse(service.HaySesionActiva); Assert.IsNull(service.UsuarioActual); Assert.IsNull(service.UsuarioId); }
    [TestMethod] public void IniciarSesion_NuevaSesion_ReemplazaAnterior() { var service = new SesionService(); service.IniciarSesion(new Usuario { Id = 1, NombreUsuario = "uno" }); service.IniciarSesion(new Usuario { Id = 2, NombreUsuario = "dos" }); Assert.AreEqual(2, service.UsuarioId); Assert.AreEqual("dos", service.NombreUsuario); }
    [TestMethod] public void IniciarSesion_UsuarioNulo_LanzaExcepcion() { var service = new SesionService(); Assert.ThrowsExactly<ArgumentNullException>(() => service.IniciarSesion(null!)); }
}
