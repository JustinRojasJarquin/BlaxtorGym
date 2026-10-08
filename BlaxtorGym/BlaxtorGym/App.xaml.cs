using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Repositories;
using BlaxtorGym.src.BlaxtorGym.WPF.Notifications;
using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using BlaxtorGym.src.BlaxtorGym.WPF.Views;
using System.Windows;

namespace BlaxtorGym;

public partial class App : Application
{
    private AppDbContext? _context;

    protected override async void OnStartup(
        StartupEventArgs e)
    {
        base.OnStartup(e);

        await DatabaseInitializer.InicializarAsync();

        _context = new AppDbContext();

        // -------------------------
        // Notificaciones
        // -------------------------

        INotificationService notificationService =
            new MessageBoxNotificationService();

        // -------------------------
        // Login
        // -------------------------

        var usuarioRepository =
            new UsuarioRepository(_context);

        var autenticacionService =
            new AutenticacionService(
                usuarioRepository);

        var loginViewModel =
            new LoginViewModel(
                autenticacionService,
                notificationService);

        // -------------------------
        // Tipos de cliente
        // -------------------------

        var tipoClienteRepository =
            new TipoClienteRepository(_context);

        var tipoClienteService =
            new TipoClienteService(
                tipoClienteRepository);

        var tipoClienteViewModel =
            new TipoClienteViewModel(
                tipoClienteService,
                notificationService);

        // -------------------------
        // Clientes
        // -------------------------

        IClienteRepository clienteRepository =
            new ClienteRepository(_context);

        var clienteService =
            new ClienteService(
                clienteRepository,
                tipoClienteRepository);

        var clientesViewModel =
            new ClientesViewModel(
                clienteService,
                tipoClienteService,
                notificationService);

        // -------------------------
        // Tipos de membresía
        // -------------------------

        var tipoMembresiaRepository =
            new TipoMembresiaRepository(_context);

        var tipoMembresiaService =
            new TipoMembresiaService(
                tipoMembresiaRepository);

        var tipoMembresiaViewModel =
            new TipoMembresiaViewModel(
                tipoMembresiaService,
                notificationService);

        // -------------------------
        // Ventana de Login
        // -------------------------

        var loginWindow =
            new LoginWindow(
                loginViewModel,
                tipoClienteViewModel,
                clientesViewModel,
                tipoMembresiaViewModel);

        MainWindow = loginWindow;

        loginWindow.Show();
    }

    protected override void OnExit(
        ExitEventArgs e)
    {
        _context?.Dispose();

        base.OnExit(e);
    }
}