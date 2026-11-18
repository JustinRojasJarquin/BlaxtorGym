using BlaxtorGym.src.BlaxtorGym.Application.DTOs; using BlaxtorGym.src.BlaxtorGym.Application.Services; using BlaxtorGym.src.BlaxtorGym.WPF.Commands; using BlaxtorGym.src.BlaxtorGym.WPF.Notifications; using System.Collections.ObjectModel; using System.ComponentModel; using System.Runtime.CompilerServices; using System.Windows.Input;
namespace BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
public class BitacoraViewModel : INotifyPropertyChanged
{
    private readonly BitacoraService _service; private readonly INotificationService _notifications; public BitacoraViewModel(BitacoraService service, INotificationService notifications) { _service = service; _notifications = notifications; CargarCommand = new RelayCommand(_ => CargarAsync()); }
    public ObservableCollection<BitacoraCambioDto> Registros { get; } = new(); public DateTime? FechaInicio { get; set; } public DateTime? FechaFin { get; set; } public string Accion { get; set; } = "Todas"; public string Entidad { get; set; } = "Todas"; public ICommand CargarCommand { get; }
    public async Task CargarAsync() { try { Registros.Clear(); foreach (var x in await _service.ObtenerAsync(FechaInicio, FechaFin, Accion, Entidad, null)) Registros.Add(x); } catch (Exception ex) { _notifications.MostrarError(ex.Message); } }
    public event PropertyChangedEventHandler? PropertyChanged; private void OnPropertyChanged([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}
