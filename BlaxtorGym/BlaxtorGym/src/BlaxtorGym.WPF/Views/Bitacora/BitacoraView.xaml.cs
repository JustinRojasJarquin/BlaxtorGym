using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels; using System.Windows.Controls;
namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;
public partial class BitacoraView : UserControl { public BitacoraView(BitacoraViewModel viewModel) { InitializeComponent(); DataContext = viewModel; } }
