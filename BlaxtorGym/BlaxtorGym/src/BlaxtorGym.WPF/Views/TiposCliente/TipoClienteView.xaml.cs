using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using System.Windows.Controls;

namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;

public partial class TipoClienteView : UserControl
{
    private readonly TipoClienteViewModel _viewModel;

    public TipoClienteView(TipoClienteViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += TipoClienteView_Loaded;
    }

    private async void TipoClienteView_Loaded(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        Loaded -= TipoClienteView_Loaded;

        await _viewModel.CargarAsync();
    }
}