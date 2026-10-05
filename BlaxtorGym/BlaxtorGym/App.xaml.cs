using BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;
using System.Windows;

namespace BlaxtorGym;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        await DatabaseInitializer.InicializarAsync();

        base.OnStartup(e);
    }
}
