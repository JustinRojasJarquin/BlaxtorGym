using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;

public partial class NotificacionesView : UserControl
{
    private readonly NotificacionesViewModel _viewModel;
    private bool _cargado;

    public NotificacionesView(
        NotificacionesViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        DataContext = _viewModel;

        Loaded += NotificacionesView_Loaded;
    }

    private async void NotificacionesView_Loaded(
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