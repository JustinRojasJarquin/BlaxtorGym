using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Configuration;
using Microsoft.Data.Sqlite;
using System.IO;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Services;

public class BackupService
{
    private const string ArchivoRestauracionPendiente =
        "restore.pending";

    private const int MaximoCopiasRecovery =
        3;

    private readonly string _rutaBaseDatos;

    public BackupService()
    {
        _rutaBaseDatos =
            DatabaseSettings.ObtenerRutaBaseDatos();
    }

    // =========================================================
    // CREAR RESPALDO MANUAL
    // =========================================================

    public async Task<BackupInfo> CrearBackupEnRutaAsync(
        string rutaDestino)
    {
        if (string.IsNullOrWhiteSpace(
                rutaDestino))
        {
            throw new ArgumentException(
                "Debe seleccionar dónde guardar el respaldo.");
        }

        if (!File.Exists(
                _rutaBaseDatos))
        {
            throw new FileNotFoundException(
                "No se encontró la base de datos de BlaxtorGym.");
        }

        string rutaOrigenCompleta =
            Path.GetFullPath(
                _rutaBaseDatos);

        string rutaDestinoCompleta =
            Path.GetFullPath(
                rutaDestino);

        if (string.Equals(
                rutaOrigenCompleta,
                rutaDestinoCompleta,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "El respaldo no puede guardarse sobre la base de datos actualmente utilizada.");
        }

        string? carpetaDestino =
            Path.GetDirectoryName(
                rutaDestinoCompleta);

        if (string.IsNullOrWhiteSpace(
                carpetaDestino))
        {
            throw new InvalidOperationException(
                "No fue posible determinar la carpeta de destino.");
        }

        Directory.CreateDirectory(
            carpetaDestino);

        // El SaveFileDialog ya pregunta si desea reemplazar,
        // pero eliminamos el archivo anterior antes de crear
        // la nueva copia SQLite.
        if (File.Exists(
                rutaDestinoCompleta))
        {
            File.Delete(
                rutaDestinoCompleta);
        }

        await CopiarBaseSqliteAsync(
            _rutaBaseDatos,
            rutaDestinoCompleta);

        bool valido =
            await VerificarIntegridadAsync(
                rutaDestinoCompleta);

        if (!valido)
        {
            if (File.Exists(
                    rutaDestinoCompleta))
            {
                File.Delete(
                    rutaDestinoCompleta);
            }

            throw new InvalidOperationException(
                "El respaldo fue creado, pero no superó la verificación de integridad.");
        }

        return CrearBackupInfo(
            new FileInfo(
                rutaDestinoCompleta));
    }

    // =========================================================
    // VERIFICAR INTEGRIDAD
    // =========================================================

    public async Task<bool> VerificarIntegridadAsync(
        string rutaBackup)
    {
        if (string.IsNullOrWhiteSpace(
                rutaBackup))
        {
            return false;
        }

        if (!File.Exists(
                rutaBackup))
        {
            return false;
        }

        try
        {
            string cadenaConexion =
                new SqliteConnectionStringBuilder
                {
                    DataSource =
                        rutaBackup,

                    Mode =
                        SqliteOpenMode.ReadOnly,

                    Pooling =
                        false
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
        catch
        {
            return false;
        }
    }

    // =========================================================
    // ELIMINAR RESPALDO MANUAL
    // =========================================================

    public async Task EliminarBackupAsync(
        string rutaBackup)
    {
        if (string.IsNullOrWhiteSpace(
                rutaBackup))
        {
            throw new ArgumentException(
                "Debe seleccionar un respaldo.");
        }

        string rutaCompleta =
            Path.GetFullPath(
                rutaBackup);

        if (!File.Exists(
                rutaCompleta))
        {
            throw new FileNotFoundException(
                "El respaldo seleccionado ya no existe.");
        }

        await Task.Run(
            () =>
            {
                File.Delete(
                    rutaCompleta);
            });
    }

    // =========================================================
    // PREPARAR RESTAURACIÓN
    // =========================================================

    public async Task PrepararRestauracionAsync(
        string rutaBackup)
    {
        if (string.IsNullOrWhiteSpace(
                rutaBackup))
        {
            throw new ArgumentException(
                "Debe seleccionar un respaldo.");
        }

        if (!File.Exists(
                rutaBackup))
        {
            throw new FileNotFoundException(
                "El respaldo seleccionado ya no existe.");
        }

        string rutaBackupCompleta =
            Path.GetFullPath(
                rutaBackup);

        string rutaBaseCompleta =
            Path.GetFullPath(
                _rutaBaseDatos);

        if (string.Equals(
                rutaBackupCompleta,
                rutaBaseCompleta,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "No se puede restaurar utilizando la base de datos actualmente utilizada por BlaxtorGym.");
        }

        bool valido =
            await VerificarIntegridadAsync(
                rutaBackupCompleta);

        if (!valido)
        {
            throw new InvalidOperationException(
                "El respaldo seleccionado no superó la verificación de integridad.");
        }

        string rutaMarcador =
            ObtenerRutaMarcadorRestauracion();

        string? carpetaMarcador =
            Path.GetDirectoryName(
                rutaMarcador);

        if (string.IsNullOrWhiteSpace(
                carpetaMarcador))
        {
            throw new InvalidOperationException(
                "No fue posible determinar la carpeta para preparar la restauración.");
        }

        Directory.CreateDirectory(
            carpetaMarcador);

        await File.WriteAllTextAsync(
            rutaMarcador,
            rutaBackupCompleta);
    }

    // =========================================================
    // APLICAR RESTAURACIÓN PENDIENTE
    // =========================================================

    public async Task<bool>
        AplicarRestauracionPendienteAsync()
    {
        string rutaMarcador =
            ObtenerRutaMarcadorRestauracion();

        if (!File.Exists(
                rutaMarcador))
        {
            return false;
        }

        string rutaBackup =
            (await File.ReadAllTextAsync(
                rutaMarcador))
            .Trim();

        // -----------------------------------------------------
        // Validar respaldo pendiente
        // -----------------------------------------------------

        if (string.IsNullOrWhiteSpace(
                rutaBackup))
        {
            EliminarMarcadorSiExiste(
                rutaMarcador);

            throw new InvalidOperationException(
                "La restauración pendiente no contiene una ruta válida.");
        }

        if (!File.Exists(
                rutaBackup))
        {
            EliminarMarcadorSiExiste(
                rutaMarcador);

            throw new FileNotFoundException(
                "El respaldo pendiente de restauración ya no existe.");
        }

        bool respaldoValido =
            await VerificarIntegridadAsync(
                rutaBackup);

        if (!respaldoValido)
        {
            EliminarMarcadorSiExiste(
                rutaMarcador);

            throw new InvalidOperationException(
                "El respaldo pendiente no superó la verificación de integridad.");
        }

        // -----------------------------------------------------
        // Liberar pools SQLite
        // -----------------------------------------------------

        SqliteConnection.ClearAllPools();

        string? carpetaBase =
            Path.GetDirectoryName(
                _rutaBaseDatos);

        if (string.IsNullOrWhiteSpace(
                carpetaBase))
        {
            throw new InvalidOperationException(
                "No fue posible determinar la carpeta de la base de datos.");
        }

        Directory.CreateDirectory(
            carpetaBase);

        // =====================================================
        // 1. CREAR COPIA AUTOMÁTICA DE RECUPERACIÓN
        // =====================================================

        if (File.Exists(
                _rutaBaseDatos))
        {
            await CrearCopiaRecoveryAsync();
        }

        // =====================================================
        // 2. RESTAURAR RESPALDO
        // =====================================================

        await RestaurarBaseSqliteAsync(
            rutaBackup,
            _rutaBaseDatos);

        // Liberamos nuevamente conexiones SQLite temporales.
        SqliteConnection.ClearAllPools();

        // =====================================================
        // 3. VERIFICAR RESTAURACIÓN
        // =====================================================

        bool restauracionValida =
            await VerificarIntegridadAsync(
                _rutaBaseDatos);

        if (!restauracionValida)
        {
            throw new InvalidOperationException(
                "La base restaurada no superó la verificación de integridad.");
        }

        // =====================================================
        // 4. ELIMINAR restore.pending
        // =====================================================

        EliminarMarcadorSiExiste(
            rutaMarcador);

        return true;
    }

    // =========================================================
    // CREAR COPIA DE RECUPERACIÓN AUTOMÁTICA
    // =========================================================

    private async Task CrearCopiaRecoveryAsync()
    {
        string carpetaRecovery =
            ObtenerCarpetaRecovery();

        Directory.CreateDirectory(
            carpetaRecovery);

        string nombreArchivo =
            $"BlaxtorGym_AntesRestauracion_" +
            $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss-fff}.db";

        string rutaRecovery =
            Path.Combine(
                carpetaRecovery,
                nombreArchivo);

        await CopiarBaseSqliteAsync(
            _rutaBaseDatos,
            rutaRecovery);

        bool valida =
            await VerificarIntegridadAsync(
                rutaRecovery);

        if (!valida)
        {
            if (File.Exists(
                    rutaRecovery))
            {
                File.Delete(
                    rutaRecovery);
            }

            throw new InvalidOperationException(
                "No fue posible crear una copia de recuperación válida antes de restaurar.");
        }

        await LimpiarCopiasRecoveryAntiguasAsync();
    }

    // =========================================================
    // LIMPIAR COPIAS RECOVERY ANTIGUAS
    // =========================================================

    private static async Task
        LimpiarCopiasRecoveryAntiguasAsync()
    {
        await Task.Run(
            () =>
            {
                string carpetaRecovery =
                    ObtenerCarpetaRecovery();

                if (!Directory.Exists(
                        carpetaRecovery))
                {
                    return;
                }

                List<FileInfo> archivos =
                    new DirectoryInfo(
                        carpetaRecovery)
                    .GetFiles(
                        "BlaxtorGym_AntesRestauracion_*.db")
                    .OrderByDescending(
                        archivo =>
                            archivo.LastWriteTimeUtc)
                    .ToList();

                if (archivos.Count <=
                    MaximoCopiasRecovery)
                {
                    return;
                }

                IEnumerable<FileInfo> archivosAEliminar =
                    archivos.Skip(
                        MaximoCopiasRecovery);

                foreach (FileInfo archivo
                         in archivosAEliminar)
                {
                    try
                    {
                        archivo.Delete();
                    }
                    catch
                    {
                        // La limpieza de una copia antigua
                        // no debe provocar que falle una
                        // restauración válida.
                    }
                }
            });
    }

    // =========================================================
    // COPIAR BASE SQLITE
    // =========================================================

    private static async Task CopiarBaseSqliteAsync(
        string rutaOrigen,
        string rutaDestino)
    {
        if (!File.Exists(
                rutaOrigen))
        {
            throw new FileNotFoundException(
                "No se encontró la base de datos de origen.",
                rutaOrigen);
        }

        string? carpetaDestino =
            Path.GetDirectoryName(
                rutaDestino);

        if (!string.IsNullOrWhiteSpace(
                carpetaDestino))
        {
            Directory.CreateDirectory(
                carpetaDestino);
        }

        string cadenaOrigen =
            new SqliteConnectionStringBuilder
            {
                DataSource =
                    rutaOrigen,

                Mode =
                    SqliteOpenMode.ReadOnly,

                Pooling =
                    false
            }
            .ToString();

        string cadenaDestino =
            new SqliteConnectionStringBuilder
            {
                DataSource =
                    rutaDestino,

                Mode =
                    SqliteOpenMode.ReadWriteCreate,

                Pooling =
                    false
            }
            .ToString();

        await using SqliteConnection origen =
            new(cadenaOrigen);

        await using SqliteConnection destino =
            new(cadenaDestino);

        await origen.OpenAsync();

        await destino.OpenAsync();

        origen.BackupDatabase(
            destino);
    }

    // =========================================================
    // RESTAURAR BASE SQLITE
    // =========================================================

    private static async Task RestaurarBaseSqliteAsync(
        string rutaBackup,
        string rutaBaseDestino)
    {
        if (!File.Exists(
                rutaBackup))
        {
            throw new FileNotFoundException(
                "No se encontró el respaldo que se desea restaurar.");
        }

        string? carpetaDestino =
            Path.GetDirectoryName(
                rutaBaseDestino);

        if (!string.IsNullOrWhiteSpace(
                carpetaDestino))
        {
            Directory.CreateDirectory(
                carpetaDestino);
        }

        string cadenaBackup =
            new SqliteConnectionStringBuilder
            {
                DataSource =
                    rutaBackup,

                Mode =
                    SqliteOpenMode.ReadOnly,

                Pooling =
                    false
            }
            .ToString();

        string cadenaDestino =
            new SqliteConnectionStringBuilder
            {
                DataSource =
                    rutaBaseDestino,

                Mode =
                    SqliteOpenMode.ReadWriteCreate,

                Pooling =
                    false
            }
            .ToString();

        await using SqliteConnection backup =
            new(cadenaBackup);

        await using SqliteConnection destino =
            new(cadenaDestino);

        await backup.OpenAsync();

        await destino.OpenAsync();

        backup.BackupDatabase(
            destino);
    }

    // =========================================================
    // CREAR BACKUP INFO
    // =========================================================

    private static BackupInfo CrearBackupInfo(
        FileInfo archivo)
    {
        return new BackupInfo
        {
            NombreArchivo =
                archivo.Name,

            RutaCompleta =
                archivo.FullName,

            FechaCreacion =
                archivo.LastWriteTime,

            TamanoBytes =
                archivo.Length
        };
    }

    // =========================================================
    // CARPETA RECOVERY
    // =========================================================

    private static string ObtenerCarpetaRecovery()
    {
        string localAppData =
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);

        return Path.Combine(
            localAppData,
            "BlaxtorGym",
            "Recovery");
    }

    // =========================================================
    // RESTORE.PENDING
    // =========================================================

    private static string
        ObtenerRutaMarcadorRestauracion()
    {
        string localAppData =
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);

        string carpeta =
            Path.Combine(
                localAppData,
                "BlaxtorGym");

        return Path.Combine(
            carpeta,
            ArchivoRestauracionPendiente);
    }

    private static void EliminarMarcadorSiExiste(
        string rutaMarcador)
    {
        if (File.Exists(
                rutaMarcador))
        {
            File.Delete(
                rutaMarcador);
        }
    }
}