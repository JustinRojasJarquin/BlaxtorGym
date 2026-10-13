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

namespace BlaxtorGym;

public partial class App : Application
{
    private AppDbContext? _context;
    private HttpClient? _httpClient;

    protected override async void OnStartup(
        StartupEventArgs e)
    {
        base.OnStartup(e);

        await DatabaseInitializer
            .InicializarAsync();

        _context =
            new AppDbContext();

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
            await EmailSettings.CargarAsync(
                rutaEmailSettings);

        var emailService =
            new EmailService(
                emailSettings);

        // =========================================
        // NOTIFICACIONES VISUALES
        // =========================================

        INotificationService notificationService =
            new MessageBoxNotificationService();

        // =========================================
        // LOGIN
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
                notificationService);

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
                tipoClienteRepository);

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
                tipoMembresiaRepository);

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
                precioRepository);

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
                membresiaRepository);

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

        var notificacionesViewModel =
            new NotificacionesViewModel(
                notificacionService,
                notificationService);

        // =========================================
        // LOGIN WINDOW
        // =========================================

        var loginWindow =
            new LoginWindow(
                loginViewModel,
                tipoClienteViewModel,
                clientesViewModel,
                tipoMembresiaViewModel,
                tarifasViewModel,
                membresiasViewModel,
                pagosViewModel,
                notificacionesViewModel);

        MainWindow =
            loginWindow;

        loginWindow.Show();
    }

    protected override void OnExit(
        ExitEventArgs e)
    {
        _httpClient?.Dispose();

        _context?.Dispose();

        base.OnExit(e);
    }
}