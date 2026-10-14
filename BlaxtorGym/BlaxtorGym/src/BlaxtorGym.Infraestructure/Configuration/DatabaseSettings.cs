using System.IO;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Configuration;

public static class DatabaseSettings
{
    private const string NombreBaseDatos =
        "blaxtorgym.db";

    public static string ObtenerRutaBaseDatos()
    {
        string localAppData =
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);

        string carpetaDatos =
            Path.Combine(
                localAppData,
                "BlaxtorGym",
                "Data");

        Directory.CreateDirectory(
            carpetaDatos);

        return Path.Combine(
            carpetaDatos,
            NombreBaseDatos);
    }

    public static string ObtenerCadenaConexion()
    {
        string ruta =
            ObtenerRutaBaseDatos();

        return $"Data Source={ruta}";
    }

    public static string ObtenerRutaBaseDatosAnterior()
    {
        return Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            NombreBaseDatos);
    }
}