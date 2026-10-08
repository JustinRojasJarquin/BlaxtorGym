using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using BlaxtorGym.src.BlaxtorGym.WPF.Views;
using System.Windows;

namespace BlaxtorGym;

public partial class MainWindow : Window
{
    private readonly TipoClienteViewModel
        _tipoClienteViewModel;

    private readonly ClientesViewModel
        _clientesViewModel;

    private readonly TipoMembresiaViewModel
        _tipoMembresiaViewModel;

    public MainWindow(
        TipoClienteViewModel tipoClienteViewModel,
        ClientesViewModel clientesViewModel,
        TipoMembresiaViewModel tipoMembresiaViewModel)
    {
        InitializeComponent();

        _tipoClienteViewModel =
            tipoClienteViewModel;

        _clientesViewModel =
            clientesViewModel;

        _tipoMembresiaViewModel =
            tipoMembresiaViewModel;

        MostrarTiposMembresia();
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
        MostrarTiposMembresia();
    }

    private void MostrarTiposMembresia()
    {
        ContenidoPrincipal.Content =
            new TipoMembresiaView(
                _tipoMembresiaViewModel);
    }
}