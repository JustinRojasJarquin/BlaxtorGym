using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using BlaxtorGym.src.BlaxtorGym.WPF.Views;
using System.Windows;

namespace BlaxtorGym;

public partial class MainWindow : Window
{
    private readonly TipoClienteViewModel _tipoClienteViewModel;
    private readonly ClientesViewModel _clientesViewModel;

    public MainWindow(
        TipoClienteViewModel tipoClienteViewModel,
        ClientesViewModel clientesViewModel)
    {
        InitializeComponent();

        _tipoClienteViewModel = tipoClienteViewModel;
        _clientesViewModel = clientesViewModel;

        MostrarTiposCliente();
    }

    private void MostrarTiposCliente_Click(
        object sender,
        RoutedEventArgs e)
    {
        MostrarTiposCliente();
    }

    private void MostrarClientes_Click(
        object sender,
        RoutedEventArgs e)
    {
        ContenidoPrincipal.Content =
            new ClientesView(_clientesViewModel);
    }

    private void MostrarTiposCliente()
    {
        ContenidoPrincipal.Content =
            new TipoClienteView(_tipoClienteViewModel);
    }
}