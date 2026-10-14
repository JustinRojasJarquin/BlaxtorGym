using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using BlaxtorGym.src.BlaxtorGym.WPF.Views;
using System.Windows;

namespace BlaxtorGym;

public partial class MainWindow : Window
{
    private readonly TipoClienteViewModel _tipoClienteViewModel;
    private readonly ClientesViewModel _clientesViewModel;
    private readonly MedicionesViewModel _medicionesViewModel;
    private readonly TipoMembresiaViewModel _tipoMembresiaViewModel;
    private readonly TarifasViewModel _tarifasViewModel;
    private readonly MembresiasViewModel _membresiasViewModel;
    private readonly PagosViewModel _pagosViewModel;
    private readonly NotificacionesViewModel _notificacionesViewModel;
    private readonly ConfiguracionViewModel _configuracionViewModel;

    public MainWindow(
        TipoClienteViewModel tipoClienteViewModel,
        ClientesViewModel clientesViewModel,
        MedicionesViewModel medicionesViewModel,
        TipoMembresiaViewModel tipoMembresiaViewModel,
        TarifasViewModel tarifasViewModel,
        MembresiasViewModel membresiasViewModel,
        PagosViewModel pagosViewModel,
        NotificacionesViewModel notificacionesViewModel,
        ConfiguracionViewModel configuracionViewModel)
    {
        InitializeComponent();

        _tipoClienteViewModel =
            tipoClienteViewModel;

        _clientesViewModel =
            clientesViewModel;

        _medicionesViewModel =
            medicionesViewModel;

        _tipoMembresiaViewModel =
            tipoMembresiaViewModel;

        _tarifasViewModel =
            tarifasViewModel;

        _membresiasViewModel =
            membresiasViewModel;

        _pagosViewModel =
            pagosViewModel;

        _notificacionesViewModel =
            notificacionesViewModel;

        _configuracionViewModel =
            configuracionViewModel;

        MostrarNotificaciones();
    }

    private void TipoCliente_Click(
        object sender,
        RoutedEventArgs e)
    {
        ContenidoPrincipal.Content =
            new TipoClienteView(
                _tipoClienteViewModel);
    }

    private void Clientes_Click(
        object sender,
        RoutedEventArgs e)
    {
        ContenidoPrincipal.Content =
            new ClientesView(
                _clientesViewModel);
    }

    private void Mediciones_Click(
        object sender,
        RoutedEventArgs e)
    {
        ContenidoPrincipal.Content =
            new MedicionesView(
                _medicionesViewModel);
    }

    private void TipoMembresia_Click(
        object sender,
        RoutedEventArgs e)
    {
        ContenidoPrincipal.Content =
            new TipoMembresiaView(
                _tipoMembresiaViewModel);
    }

    private void Tarifas_Click(
        object sender,
        RoutedEventArgs e)
    {
        ContenidoPrincipal.Content =
            new TarifasView(
                _tarifasViewModel);
    }

    private void Membresias_Click(
        object sender,
        RoutedEventArgs e)
    {
        ContenidoPrincipal.Content =
            new MembresiasView(
                _membresiasViewModel);
    }

    private void Pagos_Click(
        object sender,
        RoutedEventArgs e)
    {
        ContenidoPrincipal.Content =
            new PagosView(
                _pagosViewModel);
    }

    private void Notificaciones_Click(
        object sender,
        RoutedEventArgs e)
    {
        MostrarNotificaciones();
    }

    private void Configuracion_Click(
        object sender,
        RoutedEventArgs e)
    {
        ContenidoPrincipal.Content =
            new ConfiguracionView(
                _configuracionViewModel);
    }

    private void MostrarNotificaciones()
    {
        ContenidoPrincipal.Content =
            new NotificacionesView(
                _notificacionesViewModel);
    }
}