using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using BlaxtorGym.src.BlaxtorGym.WPF.Views;
using System.Windows;

namespace BlaxtorGym;

public partial class MainWindow : Window
{
    private readonly TipoClienteViewModel _tipoClienteViewModel;
    private readonly ClientesViewModel _clientesViewModel;
    private readonly TipoMembresiaViewModel _tipoMembresiaViewModel;
    private readonly TarifasViewModel _tarifasViewModel;
    private readonly MembresiasViewModel _membresiasViewModel;
    private readonly PagosViewModel _pagosViewModel;

    public MainWindow(
        TipoClienteViewModel tipoClienteViewModel,
        ClientesViewModel clientesViewModel,
        TipoMembresiaViewModel tipoMembresiaViewModel,
        TarifasViewModel tarifasViewModel,
        MembresiasViewModel membresiasViewModel,
        PagosViewModel pagosViewModel)
    {
        InitializeComponent();

        _tipoClienteViewModel =
            tipoClienteViewModel;

        _clientesViewModel =
            clientesViewModel;

        _tipoMembresiaViewModel =
            tipoMembresiaViewModel;

        _tarifasViewModel =
            tarifasViewModel;

        _membresiasViewModel =
            membresiasViewModel;

        _pagosViewModel =
            pagosViewModel;

        MostrarPagos();
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
        MostrarPagos();
    }

    private void MostrarPagos()
    {
        ContenidoPrincipal.Content =
            new PagosView(
                _pagosViewModel);
    }
}