using System.Windows;

namespace BlaxtorGym.src.BlaxtorGym.WPF.Notifications;

public class MessageBoxNotificationService
    : NotificationServiceBase
{
    protected override void Mostrar(
        string mensaje,
        string titulo,
        TipoNotificacion tipo)
    {
        MessageBoxImage icono = tipo switch
        {
            TipoNotificacion.Informacion =>
                MessageBoxImage.Information,

            TipoNotificacion.Exito =>
                MessageBoxImage.Information,

            TipoNotificacion.Advertencia =>
                MessageBoxImage.Warning,

            TipoNotificacion.Error =>
                MessageBoxImage.Error,

            _ => MessageBoxImage.None
        };

        MessageBox.Show(
            mensaje,
            titulo,
            MessageBoxButton.OK,
            icono);
    }
}