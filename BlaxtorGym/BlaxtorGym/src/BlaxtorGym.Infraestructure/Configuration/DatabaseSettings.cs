using System.IO;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Configuration;

public static class DatabaseSettings
{
    public static string ObtenerRutaBaseDatos()
    {
        string carpetaDatos = Path.Combine(
            AppContext.BaseDirectory,
            "Data");

        Directory.CreateDirectory(carpetaDatos);

        return Path.Combine(carpetaDatos, "blaxtorgym.db");
    }

    public static string ObtenerCadenaConexion()
    {
        return $"Data Source={ObtenerRutaBaseDatos()}";
    }
}