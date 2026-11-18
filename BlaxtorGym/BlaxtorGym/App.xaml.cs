using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Application.Settings;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Repositories;
using BlaxtorGym.src.BlaxtorGym.WPF.Notifications;
using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using BlaxtorGym.src.BlaxtorGym.WPF.Views;
using System.IO;
using System.Net.Http;
using System.Windows;
using QuestPDF.Infrastructure;

namespace BlaxtorGym;

public partial class App : Application
{
    private AppDbContext? _context;
    private HttpClient? _httpClient;

    protected override async void OnStartup(
        StartupEventArgs e)
    {
        base.OnStartup(e);
        QuestPDF.Settings.License = LicenseType.Community;

        try
        {
            // =========================================
            // MIGRAR UBICACIÓN ANTIGUA DE LA BD
            // =========================================

            var databaseLocationService =
                new BlaxtorGym.src.BlaxtorGym
                    .Infraestructure.Services.DatabaseLocationService();

            await databaseLocationService
                .MigrarBaseAnteriorSiEsNecesarioAsync();

            // =========================================
            // BACKUP / RESTAURACIÓN PENDIENTE
            // =========================================

            var backupService =
                new BlaxtorGym.src.BlaxtorGym
                    .Infraestructure.Services.BackupService();

            await backupService
                .AplicarRestauracionPendienteAsync();

            // =========================================
            // BASE DE DATOS / EF CORE
            // =========================================

            await DatabaseInitializer
                .InicializarAsync();

            _context =
                new AppDbContext();

            // =========================================
            // HTTP
            // =========================================

            _httpClient =
                new HttpClient
                {
                    Timeout =
                        TimeSpan.FromSeconds(10)
                };

            // =========================================
            // CONFIGURACIÓN DE CORREO
            // =========================================

            string rutaEmailSettings =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "emailsettings.local.json");

            EmailSettings emailSettings =
                await EmailSettings
                    .CargarAsync(
                        rutaEmailSettings);

            var emailService =
                new EmailService(
                    emailSettings);

            // =========================================
            // NOTIFICACIONES VISUALES
            // =========================================

            INotificationService notificationService =
                new MessageBoxNotificationService();

            var sesionService = new SesionService();
            var bitacoraRepository = new BitacoraRepository(_context);
            var bitacoraService = new BitacoraService(bitacoraRepository);

            // =========================================
            // AUTENTICACIÓN
            // =========================================

            var usuarioRepository =
                new UsuarioRepository(
                    _context);

            var autenticacionService =
                new AutenticacionService(
                    usuarioRepository);

            var loginViewModel =
                new LoginViewModel(
                    autenticacionService,
                    notificationService,
                    sesionService);

            // =========================================
            // TIPOS DE CLIENTE
            // =========================================

            var tipoClienteRepository =
                new TipoClienteRepository(
                    _context);

            var tipoClienteService =
                new TipoClienteService(
                    tipoClienteRepository);

            var tipoClienteViewModel =
                new TipoClienteViewModel(
                    tipoClienteService,
                    notificationService);

            // =========================================
            // CLIENTES
            // =========================================

            IClienteRepository clienteRepository =
                new ClienteRepository(
                    _context);

            var clienteService =
                new ClienteService(
                    clienteRepository,
                    tipoClienteRepository,
                    bitacoraService,
                    sesionService);

            var consultaCedulaService =
                new ConsultaCedulaService(
                    _httpClient);

            var clientesViewModel =
                new ClientesViewModel(
                    clienteService,
                    tipoClienteService,
                    consultaCedulaService,
                    notificationService);

            // =========================================
            // MEDICIONES FÍSICAS
            // =========================================

            var medicionFisicaRepository =
                new MedicionFisicaRepository(
                    _context);

            var medicionFisicaService =
                new MedicionFisicaService(
                    medicionFisicaRepository,
                    clienteRepository,
                    bitacoraService,
                    sesionService);

            var medicionesViewModel =
                new MedicionesViewModel(
                    clienteService,
                    medicionFisicaService,
                    notificationService);

            // =========================================
            // TIPOS DE MEMBRESÍA
            // =========================================

            var tipoMembresiaRepository =
                new TipoMembresiaRepository(
                    _context);

            var tipoMembresiaService =
                new TipoMembresiaService(
                    tipoMembresiaRepository);

            var tipoMembresiaViewModel =
                new TipoMembresiaViewModel(
                    tipoMembresiaService,
                    notificationService);

            // =========================================
            // TARIFAS
            // =========================================

            var precioRepository =
                new PrecioRepository(
                    _context);

            var tarifaService =
                new TarifaService(
                    precioRepository,
                    tipoClienteRepository,
                    tipoMembresiaRepository,
                    bitacoraService,
                    sesionService);

            var tarifasViewModel =
                new TarifasViewModel(
                    tarifaService,
                    tipoClienteService,
                    tipoMembresiaService,
                    notificationService);

            // =========================================
            // MEMBRESÍAS
            // =========================================

            var membresiaRepository =
                new MembresiaRepository(
                    _context);

            var membresiaService =
                new MembresiaService(
                    membresiaRepository,
                    clienteRepository,
                    tipoMembresiaRepository,
                    precioRepository,
                    bitacoraService,
                    sesionService);

            var membresiasViewModel =
                new MembresiasViewModel(
                    membresiaService,
                    clienteService,
                    tarifaService,
                    notificationService);

            // =========================================
            // PAGOS
            // =========================================

            var pagoRepository =
                new PagoRepository(
                    _context);

            var pagoService =
                new PagoService(
                    pagoRepository,
                    membresiaRepository,
                    bitacoraService,
                    sesionService);

            var pagosViewModel =
                new PagosViewModel(
                    pagoService,
                    membresiaService,
                    notificationService);

            // =========================================
            // NOTIFICACIONES DE MEMBRESÍAS
            // =========================================

            var notificacionRepository =
                new NotificacionRepository(
                    _context);

            var notificacionService =
                new NotificacionService(
                    notificacionRepository,
                    membresiaRepository,
                    emailService);

            var reporteRepository = new ReporteRepository(_context);
            var reporteService = new ReporteService(reporteRepository);
            var pdfService = new BlaxtorGym.src.BlaxtorGym.Infraestructure.Services.PdfService();
            var reportesViewModel = new ReportesViewModel(reporteService, pdfService, clienteService, tipoClienteService, tipoMembresiaService, notificationService);
            var bitacoraViewModel = new BitacoraViewModel(bitacoraService, notificationService);

            var notificacionesViewModel =
                new NotificacionesViewModel(
                    notificacionService,
                    notificationService);

            // =========================================
            // COPIAS DE SEGURIDAD
            // =========================================

            var configuracionViewModel =
                new ConfiguracionViewModel(
                    backupService,
                    notificationService);

            // =========================================
            // LOGIN
            // =========================================

            var loginWindow =
                new LoginWindow(
                    loginViewModel,
                    tipoClienteViewModel,
                    clientesViewModel,
                    medicionesViewModel,
                    tipoMembresiaViewModel,
                    tarifasViewModel,
                    membresiasViewModel,
                    pagosViewModel,
                    notificacionesViewModel,
                    configuracionViewModel,
                    reportesViewModel,
                    bitacoraViewModel);

            MainWindow =
                loginWindow;

            loginWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "BlaxtorGym no pudo iniciar correctamente.\n\n" +
                ex.Message,
                "Error de inicio",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();
        }
    }

    protected override void OnExit(
        ExitEventArgs e)
    {
        _httpClient?.Dispose();

        _context?.Dispose();

        base.OnExit(e);
    }
}
