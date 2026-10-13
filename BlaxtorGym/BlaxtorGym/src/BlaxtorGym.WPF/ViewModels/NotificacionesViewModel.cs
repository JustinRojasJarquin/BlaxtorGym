using BlaxtorGym.src.BlaxtorGym.Application.Services;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.WPF.Commands;
using BlaxtorGym.src.BlaxtorGym.WPF.Notifications;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;

public class NotificacionesViewModel : INotifyPropertyChanged
{
    private readonly NotificacionService _notificacionService;
    private readonly INotificationService _notificationService;

    private Notificacion? _notificacionSeleccionada;
    private bool _procesando;
    private bool _mostrarSoloPendientes = true;

    public NotificacionesViewModel(
        NotificacionService notificacionService,
        INotificationService notificationService)
    {
        _notificacionService = notificacionService;
        _notificationService = notificationService;

        GenerarNotificacionesCommand =
            new RelayCommand(
                _ => GenerarNotificacionesAsync(),
                _ => !Procesando);

        EnviarCorreoCommand =
            new RelayCommand(
                _ => EnviarCorreoAsync(),
                _ =>
                    !Procesando &&
                    NotificacionSeleccionada is not null &&
                    !NotificacionSeleccionada.Enviada);

        MostrarPendientesCommand =
            new RelayCommand(
                _ => MostrarPendientesAsync(),
                _ => !Procesando);

        MostrarTodasCommand =
            new RelayCommand(
                _ => MostrarTodasAsync(),
                _ => !Procesando);
    }

    public ObservableCollection<Notificacion> Notificaciones { get; }
        = new();

    public Notificacion? NotificacionSeleccionada
    {
        get => _notificacionSeleccionada;

        set
        {
            if (_notificacionSeleccionada == value)
            {
                return;
            }

            _notificacionSeleccionada = value;

            OnPropertyChanged();

            CommandManager.InvalidateRequerySuggested();
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

    public bool MostrarSoloPendientes
    {
        get => _mostrarSoloPendientes;

        private set
        {
            if (_mostrarSoloPendientes == value)
            {
                return;
            }

            _mostrarSoloPendientes = value;

            OnPropertyChanged();
        }
    }

    public ICommand GenerarNotificacionesCommand { get; }

    public ICommand EnviarCorreoCommand { get; }

    public ICommand MostrarPendientesCommand { get; }

    public ICommand MostrarTodasCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task CargarAsync()
    {
        try
        {
            Procesando = true;

            await _notificacionService
                .GenerarNotificacionesAsync();

            await CargarPendientesAsync();
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "No fue posible cargar las notificaciones.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task GenerarNotificacionesAsync()
    {
        try
        {
            Procesando = true;

            await _notificacionService
                .GenerarNotificacionesAsync();

            _notificationService.MostrarExito(
                "Las notificaciones fueron actualizadas correctamente.");

            await RecargarListaActualAsync();
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "No fue posible generar las notificaciones.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task EnviarCorreoAsync()
    {
        if (NotificacionSeleccionada is null)
        {
            return;
        }

        try
        {
            Procesando = true;

            await _notificacionService
                .EnviarCorreoAsync(
                    NotificacionSeleccionada.Id);

            _notificationService.MostrarExito(
                "El correo fue enviado correctamente.");

            NotificacionSeleccionada = null;

            await RecargarListaActualAsync();
        }
        catch (ArgumentException ex)
        {
            _notificationService.MostrarAdvertencia(
                ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _notificationService.MostrarAdvertencia(
                ex.Message);
        }
        catch (Exception)
        {
            /*
             * Si Gmail, Internet o SMTP falla,
             * no marcamos la notificación como enviada.
             */
            _notificationService.MostrarError(
                "No fue posible enviar el correo. " +
                "La notificación continuará pendiente.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task MostrarPendientesAsync()
    {
        try
        {
            Procesando = true;

            MostrarSoloPendientes = true;

            await CargarPendientesAsync();
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "No fue posible cargar las notificaciones pendientes.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task MostrarTodasAsync()
    {
        try
        {
            Procesando = true;

            MostrarSoloPendientes = false;

            await CargarTodasAsync();
        }
        catch (Exception)
        {
            _notificationService.MostrarError(
                "No fue posible cargar las notificaciones.");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task RecargarListaActualAsync()
    {
        if (MostrarSoloPendientes)
        {
            await CargarPendientesAsync();
        }
        else
        {
            await CargarTodasAsync();
        }
    }

    private async Task CargarPendientesAsync()
    {
        List<Notificacion> notificaciones =
            await _notificacionService
                .ObtenerPendientesAsync();

        ActualizarLista(
            notificaciones);
    }

    private async Task CargarTodasAsync()
    {
        List<Notificacion> notificaciones =
            await _notificacionService
                .ObtenerTodasAsync();

        ActualizarLista(
            notificaciones);
    }

    private void ActualizarLista(
        IEnumerable<Notificacion> notificaciones)
    {
        Notificaciones.Clear();

        foreach (Notificacion notificacion in notificaciones)
        {
            Notificaciones.Add(
                notificacion);
        }
    }

    private void OnPropertyChanged(
        [CallerMemberName]
        string? nombrePropiedad = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                nombrePropiedad));
    }
}