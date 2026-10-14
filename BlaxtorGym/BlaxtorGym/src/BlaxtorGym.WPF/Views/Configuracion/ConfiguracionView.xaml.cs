using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;

public partial class ConfiguracionView : UserControl
{
    private readonly ConfiguracionViewModel
        _viewModel;

    public ConfiguracionView(
        ConfiguracionViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        DataContext = _viewModel;

        Loaded +=
            ConfiguracionView_Loaded;
    }

    private async void ConfiguracionView_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        Loaded -=
            ConfiguracionView_Loaded;

        await _viewModel
            .CargarAsync();
    }
}