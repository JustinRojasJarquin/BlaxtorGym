using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using System.Windows.Controls;

namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;

public partial class TipoMembresiaView : UserControl
{
    private readonly TipoMembresiaViewModel _viewModel;

    public TipoMembresiaView(
        TipoMembresiaViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += TipoMembresiaView_Loaded;
    }

    private async void TipoMembresiaView_Loaded(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        Loaded -= TipoMembresiaView_Loaded;

        await _viewModel.CargarAsync();
    }
}