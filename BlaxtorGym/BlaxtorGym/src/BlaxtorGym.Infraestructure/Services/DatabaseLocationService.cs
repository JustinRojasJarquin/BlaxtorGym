using BlaxtorGym.src.BlaxtorGym.Infraestructure.Configuration;
using Microsoft.Data.Sqlite;
using System.IO;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Services;

public class DatabaseLocationService
{
    public async Task MigrarBaseAnteriorSiEsNecesarioAsync()
    {
        string rutaNueva =
            DatabaseSettings
                .ObtenerRutaBaseDatos();

        string rutaAnterior =
            DatabaseSettings
                .ObtenerRutaBaseDatosAnterior();

        // Ya existe la base en la ubicación nueva.
        if (File.Exists(rutaNueva))
        {
            return;
        }

        // No existe una base anterior.
        // DatabaseInitializer podrá crear una nueva.
        if (!File.Exists(rutaAnterior))
        {
            return;
        }

        string? carpetaNueva =
            Path.GetDirectoryName(
                rutaNueva);

        if (string.IsNullOrWhiteSpace(
                carpetaNueva))
        {
            throw new InvalidOperationException(
                "No fue posible determinar la nueva carpeta de datos.");
        }

        Directory.CreateDirectory(
            carpetaNueva);

        string cadenaAnterior =
            new SqliteConnectionStringBuilder
            {
                DataSource =
                    rutaAnterior
            }
            .ToString();

        string cadenaNueva =
            new SqliteConnectionStringBuilder
            {
                DataSource =
                    rutaNueva
            }
            .ToString();

        await using SqliteConnection origen =
            new(cadenaAnterior);

        await using SqliteConnection destino =
            new(cadenaNueva);

        await origen.OpenAsync();

        await destino.OpenAsync();

        origen.BackupDatabase(
            destino);

        bool valida =
            await VerificarIntegridadAsync(
                rutaNueva);

        if (!valida)
        {
            if (File.Exists(rutaNueva))
            {
                File.Delete(rutaNueva);
            }

            throw new InvalidOperationException(
                "La base de datos anterior no pudo migrarse correctamente a la nueva ubicación.");
        }
    }

    private static async Task<bool>
        VerificarIntegridadAsync(
            string rutaBaseDatos)
    {
        string cadenaConexion =
            new SqliteConnectionStringBuilder
            {
                DataSource =
                    rutaBaseDatos,

                Mode =
                    SqliteOpenMode.ReadOnly
            }
            .ToString();

        await using SqliteConnection conexion =
            new(cadenaConexion);

        await conexion.OpenAsync();

        await using SqliteCommand comando =
            conexion.CreateCommand();

        comando.CommandText =
            "PRAGMA integrity_check;";

        object? resultado =
            await comando.ExecuteScalarAsync();

        return string.Equals(
            resultado?.ToString(),
            "ok",
            StringComparison.OrdinalIgnoreCase);
    }
}