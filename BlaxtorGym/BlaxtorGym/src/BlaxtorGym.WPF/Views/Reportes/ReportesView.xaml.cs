using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;
public partial class ReportesView : UserControl
{
    public ReportesView(ReportesViewModel viewModel) { InitializeComponent(); DataContext = viewModel; Loaded += CargarCatalogos; }
    private async void CargarCatalogos(object sender, RoutedEventArgs e) { Loaded -= CargarCatalogos; if (DataContext is ReportesViewModel vm) await vm.CargarCatalogosAsync(); }
}
