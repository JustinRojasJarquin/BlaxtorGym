using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;

public partial class MedicionesView : UserControl
{
    private readonly MedicionesViewModel _viewModel;
    private bool _cargado;

    public MedicionesView(MedicionesViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += MedicionesView_Loaded;
    }

    private async void MedicionesView_Loaded(
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
