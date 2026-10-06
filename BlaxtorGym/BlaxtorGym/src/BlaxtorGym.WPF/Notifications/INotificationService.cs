namespace BlaxtorGym.src.BlaxtorGym.WPF.Notifications;

public interface INotificationService
{
    void MostrarInformacion(string mensaje);

    void MostrarExito(string mensaje);

    void MostrarAdvertencia(string mensaje);

    void MostrarError(string mensaje);
}