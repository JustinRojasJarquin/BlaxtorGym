using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using System.Windows.Controls;

namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;

public partial class MembresiasView : UserControl
{
    private readonly MembresiasViewModel _viewModel;

    public MembresiasView(
        MembresiasViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += MembresiasView_Loaded;
    }

    private async void MembresiasView_Loaded(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        Loaded -= MembresiasView_Loaded;

        await _viewModel.CargarAsync();
    }
}