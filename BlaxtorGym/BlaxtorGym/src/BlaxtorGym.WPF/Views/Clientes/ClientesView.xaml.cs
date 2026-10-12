using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;

public partial class ClientesView : UserControl
{
    private readonly ClientesViewModel _viewModel;
    private bool _cargado;

    public ClientesView(
        ClientesViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        DataContext = _viewModel;

        Loaded += ClientesView_Loaded;
    }

    private async void ClientesView_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        if (_cargado)
        {
            return;
        }

        _cargado = true;

        await _viewModel.CargarAsync();
    }
}