using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.Tests.Services;

[TestClass]
public class MedicionFisicaServiceTests
{
    private FakeMedicionFisicaRepository _medicionRepository = null!;
    private FakeClienteRepository _clienteRepository = null!;
    private MedicionFisicaService _service = null!;

    [TestInitialize]
    public void Inicializar()
    {
        _medicionRepository =
            new FakeMedicionFisicaRepository();
        _clienteRepository =
            new FakeClienteRepository
            {
                Cliente = new Cliente
                {
                    Id = 7,
                    NombreCompleto = "Cliente de prueba",
                    Activo = true
                }
            };
        _service =
            new MedicionFisicaService(
                _medicionRepository,
                _clienteRepository);
    }

    [TestMethod]
    public async Task RegistrarAsync_MedicionValida_GuardaDatosNormalizados()
    {
        DateTime fecha = DateTime.Today.AddDays(-1);

        MedicionFisica medicion =
            await _service.RegistrarAsync(
                7,
                fecha,
                72.45m,
                178m,
                18.5m,
                "  Control mensual  ");

        Assert.AreSame(
            medicion,
            _medicionRepository.MedicionGuardada);
        Assert.AreEqual(7, medicion.ClienteId);
        Assert.AreEqual(fecha, medicion.FechaMedicion);
        Assert.AreEqual(72.45m, medicion.PesoKg);
        Assert.AreEqual(178m, medicion.EstaturaCm);
        Assert.AreEqual(18.5m, medicion.PorcentajeGrasa);
        Assert.AreEqual("Control mensual", medicion.Observacion);
    }

    [TestMethod]
    public async Task RegistrarAsync_PesoFueraDeRango_RechazaSinGuardar()
    {
        await VerifyThrowsAsync<ArgumentOutOfRangeException>(
            () => _service.RegistrarAsync(
                7,
                DateTime.Today,
                0m,
                null,
                null,
                null));

        Assert.IsNull(_medicionRepository.MedicionGuardada);
    }

    [TestMethod]
    public async Task RegistrarAsync_DatosOpcionalesInvalidos_RechazaSinGuardar()
    {
        await VerifyThrowsAsync<ArgumentOutOfRangeException>(
            () => _service.RegistrarAsync(
                7,
                DateTime.Today,
                72m,
                301m,
                null,
                null));

        await VerifyThrowsAsync<ArgumentOutOfRangeException>(
            () => _service.RegistrarAsync(
                7,
                DateTime.Today,
                72m,
                null,
                100.01m,
                null));

        await VerifyThrowsAsync<ArgumentException>(
            () => _service.RegistrarAsync(
                7,
                DateTime.Today,
                72.456m,
                null,
                null,
                null));

        Assert.IsNull(_medicionRepository.MedicionGuardada);
    }

    [TestMethod]
    public async Task RegistrarAsync_FechaFutura_RechazaSinGuardar()
    {
        await VerifyThrowsAsync<ArgumentException>(
            () => _service.RegistrarAsync(
                7,
                DateTime.Today.AddDays(1),
                72m,
                null,
                null,
                null));

        Assert.IsNull(_medicionRepository.MedicionGuardada);
    }

    [TestMethod]
    public async Task RegistrarAsync_ClienteInactivo_RechazaSinGuardar()
    {
        _clienteRepository.Cliente!.Activo = false;

        await VerifyThrowsAsync<InvalidOperationException>(
            () => _service.RegistrarAsync(
                7,
                DateTime.Today,
                72m,
                null,
                null,
                null));

        Assert.IsNull(_medicionRepository.MedicionGuardada);
    }

    [TestMethod]
    public async Task RegistrarAsync_ClienteInexistente_RechazaSinGuardar()
    {
        _clienteRepository.Cliente = null;

        await VerifyThrowsAsync<InvalidOperationException>(
            () => _service.RegistrarAsync(
                7,
                DateTime.Today,
                72m,
                null,
                null,
                null));

        Assert.IsNull(_medicionRepository.MedicionGuardada);
    }

    [TestMethod]
    public async Task Repository_ConsultaHistorialPersistidoFiltradoYOrdenado()
    {
        await using var conexion =
            new SqliteConnection("Data Source=:memory:");
        await conexion.OpenAsync();

        DbContextOptions<AppDbContext> opciones =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(conexion)
                .Options;

        int clienteId;

        await using (var contexto = new AppDbContext(opciones))
        {
            await contexto.Database.EnsureCreatedAsync();

            var tipoCliente = new TipoCliente
            {
                Nombre = "Regular",
                Activo = true
            };
            contexto.TiposCliente.Add(tipoCliente);
            await contexto.SaveChangesAsync();

            var cliente = new Cliente
            {
                Cedula = "123456789",
                NombreCompleto = "Cliente uno",
                FechaNacimiento = new DateTime(1990, 1, 1),
                TipoClienteId = tipoCliente.Id,
                Activo = true
            };
            var otroCliente = new Cliente
            {
                Cedula = "987654321",
                NombreCompleto = "Cliente dos",
                FechaNacimiento = new DateTime(1992, 1, 1),
                TipoClienteId = tipoCliente.Id,
                Activo = true
            };
            contexto.Clientes.AddRange(cliente, otroCliente);
            await contexto.SaveChangesAsync();
            clienteId = cliente.Id;

            contexto.MedicionesFisicas.AddRange(
                new MedicionFisica
                {
                    ClienteId = cliente.Id,
                    FechaMedicion = DateTime.Today.AddDays(-2),
                    PesoKg = 75m
                },
                new MedicionFisica
                {
                    ClienteId = cliente.Id,
                    FechaMedicion = DateTime.Today,
                    PesoKg = 73m
                },
                new MedicionFisica
                {
                    ClienteId = otroCliente.Id,
                    FechaMedicion = DateTime.Today,
                    PesoKg = 60m
                });
            await contexto.SaveChangesAsync();
        }

        await using var contextoReabierto =
            new AppDbContext(opciones);

        List<MedicionFisica> historial =
            await new MedicionFisicaRepository(contextoReabierto)
                .ObtenerPorClienteAsync(clienteId);

        Assert.HasCount(2, historial);
        Assert.AreEqual(73m, historial[0].PesoKg);
        Assert.AreEqual(75m, historial[1].PesoKg);
        Assert.IsFalse(contextoReabierto.ChangeTracker
            .Entries<MedicionFisica>()
            .Any());
    }

    private static async Task VerifyThrowsAsync<TException>(
        Func<Task> operacion)
        where TException : Exception
    {
        try
        {
            await operacion();
        }
        catch (TException)
        {
            return;
        }

        Assert.Fail(
            $"Se esperaba una excepción de tipo {typeof(TException).Name}.");
    }

    private sealed class FakeMedicionFisicaRepository
        : IMedicionFisicaRepository
    {
        public MedicionFisica? MedicionGuardada { get; private set; }

        public Task AgregarAsync(MedicionFisica medicion)
        {
            MedicionGuardada = medicion;
            return Task.CompletedTask;
        }

        public Task<List<MedicionFisica>> ObtenerPorClienteAsync(
            int clienteId)
        {
            List<MedicionFisica> historial =
                MedicionGuardada?.ClienteId == clienteId
                    ? new List<MedicionFisica>
                    {
                        MedicionGuardada
                    }
                    : new List<MedicionFisica>();

            return Task.FromResult(historial);
        }
    }

    private sealed class FakeClienteRepository : IClienteRepository
    {
        public Cliente? Cliente { get; set; }

        public Task<List<Cliente>> BuscarAsync(
            string criterio,
            int limite = 100)
        {
            return Task.FromResult(new List<Cliente>());
        }

        public Task<List<Cliente>> ObtenerActivosAsync()
        {
            List<Cliente> clientes =
                Cliente is { Activo: true }
                    ? new List<Cliente> { Cliente }
                    : new List<Cliente>();

            return Task.FromResult(clientes);
        }

        public Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            return Task.FromResult(
                Cliente?.Id == id
                    ? Cliente
                    : null);
        }

        public Task<Cliente?> ObtenerPorCedulaAsync(string cedula)
        {
            return Task.FromResult<Cliente?>(null);
        }

        public Task<bool> ExisteCedulaAsync(
            string cedula,
            int? idExcluir = null)
        {
            return Task.FromResult(false);
        }

        public Task AgregarAsync(Cliente cliente)
        {
            return Task.CompletedTask;
        }

        public Task ActualizarAsync(Cliente cliente)
        {
            return Task.CompletedTask;
        }
    }
}
