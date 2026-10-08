using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using System.Windows.Controls;

namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;

public partial class TarifasView : UserControl
{
    private readonly TarifasViewModel _viewModel;

    public TarifasView(TarifasViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += TarifasView_Loaded;
    }

    private async void TarifasView_Loaded(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        Loaded -= TarifasView_Loaded;

        await _viewModel.CargarAsync();
    }
}