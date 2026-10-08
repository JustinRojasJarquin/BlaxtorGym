using BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BlaxtorGym.src.BlaxtorGym.WPF.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _loginViewModel;
    private readonly TipoClienteViewModel _tipoClienteViewModel;
    private readonly ClientesViewModel _clientesViewModel;

    public LoginWindow(
        LoginViewModel loginViewModel,
        TipoClienteViewModel tipoClienteViewModel,
        ClientesViewModel clientesViewModel)
    {
        InitializeComponent();

        _loginViewModel = loginViewModel;
        _tipoClienteViewModel = tipoClienteViewModel;
        _clientesViewModel = clientesViewModel;

        DataContext = _loginViewModel;

        _loginViewModel.LoginExitoso += AbrirVentanaPrincipal;
    }

    private void PasswordBox_PasswordChanged(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            _loginViewModel.Contrasena = passwordBox.Password;
        }
    }

    private void AbrirVentanaPrincipal()
    {
        var ventanaPrincipal =
            new MainWindow(
                _tipoClienteViewModel,
                _clientesViewModel);

        System.Windows.Application.Current.MainWindow =
            ventanaPrincipal;

        ventanaPrincipal.Show();

        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        _loginViewModel.LoginExitoso -= AbrirVentanaPrincipal;

        base.OnClosed(e);
    }
}