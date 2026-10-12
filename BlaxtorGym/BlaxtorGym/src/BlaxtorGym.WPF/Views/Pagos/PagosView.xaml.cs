using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;

public partial class PagosView : UserControl
{
    private readonly PagosViewModel _viewModel;
    private bool _cargado;

    public PagosView(
        PagosViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        DataContext = _viewModel;

        Loaded += PagosView_Loaded;
    }

    private async void PagosView_Loaded(
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