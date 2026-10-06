using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using BlaxtorGym.src.BlaxtorGym.WPF.Views;
using System.Windows;

namespace BlaxtorGym;

public partial class MainWindow : Window
{
    public MainWindow(
        TipoClienteViewModel tipoClienteViewModel)
    {
        InitializeComponent();

        var tipoClienteView =
            new TipoClienteView(tipoClienteViewModel);

        ContenidoPrincipal.Content =
            tipoClienteView;
    }
}