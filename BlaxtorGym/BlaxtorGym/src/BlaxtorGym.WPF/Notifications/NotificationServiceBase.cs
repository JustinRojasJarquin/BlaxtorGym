namespace BlaxtorGym.src.BlaxtorGym.WPF.Notifications;

public abstract class NotificationServiceBase : INotificationService
{
    public void MostrarInformacion(string mensaje)
    {
        Mostrar(
            mensaje,
            "Información",
            TipoNotificacion.Informacion);
    }

    public void MostrarExito(string mensaje)
    {
        Mostrar(
            mensaje,
            "Éxito",
            TipoNotificacion.Exito);
    }

    public void MostrarAdvertencia(string mensaje)
    {
        Mostrar(
            mensaje,
            "Advertencia",
            TipoNotificacion.Advertencia);
    }

    public void MostrarError(string mensaje)
    {
        Mostrar(
            mensaje,
            "Error",
            TipoNotificacion.Error);
    }

    protected abstract void Mostrar(
        string mensaje,
        string titulo,
        TipoNotificacion tipo);
}