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

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        await DatabaseInitializer.InicializarAsync();

        _context = new AppDbContext();

        var usuarioRepository =
            new UsuarioRepository(_context);

        var autenticacionService =
            new AutenticacionService(usuarioRepository);

        INotificationService notificationService =
            new MessageBoxNotificationService();

        var tipoClienteRepository =
            new TipoClienteRepository(_context);

        var tipoClienteService =
            new TipoClienteService(tipoClienteRepository);

        var tipoClienteViewModel =
            new TipoClienteViewModel(
                tipoClienteService,
                notificationService);

        IClienteRepository clienteRepository =
            new ClienteRepository(_context);

        var clienteService =
            new ClienteService(clienteRepository);

        var clientesViewModel =
            new ClientesViewModel(
                clienteService,
                notificationService);

        var loginViewModel =
            new LoginViewModel(
                autenticacionService,
                notificationService);

        var loginWindow =
            new LoginWindow(
                loginViewModel,
                tipoClienteViewModel,
                clientesViewModel);

        MainWindow = loginWindow;

        loginWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _context?.Dispose();

        base.OnExit(e);
    }
}