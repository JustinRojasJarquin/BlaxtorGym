using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels; using System.Windows; using System.Windows.Controls;
namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;
public partial class ReportesView : UserControl { public ReportesView(ReportesViewModel viewModel) { InitializeComponent(); DataContext = viewModel; } private async void Cargar(object sender, RoutedEventArgs e) { if (DataContext is ReportesViewModel vm) await vm.GenerarAsync(); } }
