using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.WPF.Commands;
using BlaxtorGym.src.BlaxtorGym.WPF.Notifications;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly AutenticacionService _autenticacionService;
    private readonly INotificationService _notificationService;

    private string _nombreUsuario = string.Empty;
    private string _contrasena = string.Empty;
    private bool _procesando;

    public LoginViewModel(
        AutenticacionService autenticacionService,
        INotificationService notificationService)
    {
        _autenticacionService = autenticacionService;
        _notificationService = notificationService;

        IniciarSesionCommand = new RelayCommand(
            _ => IniciarSesionAsync(),
            _ => !Procesando);
    }

    public string NombreUsuario
    {
        get => _nombreUsuario;
        set
        {
            if (_nombreUsuario == value)
            {
                return;
            }

            _nombreUsuario = value;
            OnPropertyChanged();
        }
    }

    public string Contrasena
    {
        get => _contrasena;
        set
        {
            if (_contrasena == value)
            {
                return;
            }

            _contrasena = value;
            OnPropertyChanged();
        }
    }

    public bool Procesando
    {
        get => _procesando;
        private set
        {
            if (_procesando == value)
            {
                return;
            }

            _procesando = value;
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public ICommand IniciarSesionCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event Action? LoginExitoso;

    private async Task IniciarSesionAsync()
    {
        if (Procesando)
        {
            return;
        }

        try
        {
            Procesando = true;

            var solicitud = new LoginRequestDto
            {
                NombreUsuario = NombreUsuario,
                Contrasena = Contrasena
            };

            ResultadoLogin resultado =
                await _autenticacionService.AutenticarAsync(solicitud);

            if (resultado.Exitoso)
            {
                _notificationService.MostrarExito(
                    resultado.Mensaje);

                Contrasena = string.Empty;

                LoginExitoso?.Invoke();

                return;
            }

            _notificationService.MostrarAdvertencia(
                resultado.Mensaje);
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "Ocurrió un error inesperado durante el inicio de sesión.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? nombrePropiedad = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nombrePropiedad));
    }
}