using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using System.Windows.Controls;

namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;

public partial class ClientesView : UserControl
{
    private readonly ClientesViewModel _viewModel;

    public ClientesView(ClientesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += ClientesView_Loaded;
    }

    private async void ClientesView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        Loaded -= ClientesView_Loaded;
        await _viewModel.CargarAsync();
    }
}
