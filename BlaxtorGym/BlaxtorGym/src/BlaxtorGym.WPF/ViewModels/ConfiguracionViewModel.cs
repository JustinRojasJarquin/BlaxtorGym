using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Services;
using BlaxtorGym.src.BlaxtorGym.WPF.Commands;
using BlaxtorGym.src.BlaxtorGym.WPF.Notifications;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace BlaxtorGym.src.BlaxtorGym.WPF.ViewModels;

public class ConfiguracionViewModel : INotifyPropertyChanged
{
    private readonly BackupService _backupService;
    private readonly INotificationService _notificationService;

    private BackupInfo? _backupSeleccionado;
    private bool _procesando;

    public ConfiguracionViewModel(
        BackupService backupService,
        INotificationService notificationService)
    {
        _backupService = backupService;
        _notificationService = notificationService;

        CrearBackupCommand =
            new RelayCommand(
                _ => CrearBackupAsync(),
                _ => !Procesando);

        CargarBackupCommand =
            new RelayCommand(
                _ => CargarBackupAsync(),
                _ => !Procesando);

        VerificarCommand =
            new RelayCommand(
                _ => VerificarBackupAsync(),
                _ =>
                    !Procesando &&
                    BackupSeleccionado is not null);

        RestaurarCommand =
            new RelayCommand(
                _ => RestaurarBackupAsync(),
                _ =>
                    !Procesando &&
                    BackupSeleccionado is not null);

        EliminarCommand =
            new RelayCommand(
                _ => EliminarBackupAsync(),
                _ =>
                    !Procesando &&
                    BackupSeleccionado is not null);
    }

    public ObservableCollection<BackupInfo> Backups { get; }
        = new();

    public BackupInfo? BackupSeleccionado
    {
        get => _backupSeleccionado;

        set
        {
            if (_backupSeleccionado == value)
            {
                return;
            }

            _backupSeleccionado = value;

            OnPropertyChanged();

            CommandManager
                .InvalidateRequerySuggested();
        }
    }

    public bool Procesando
    {
        get => _procesando;

        private set
        {
            if (_procesando == value)
            {
                return;
            }

            _procesando = value;

            OnPropertyChanged();

            OnPropertyChanged(
                nameof(EstadoTexto));

            CommandManager
                .InvalidateRequerySuggested();
        }
    }

    public int TotalBackups =>
        Backups.Count;

    public string EstadoTexto =>
        Procesando
            ? "Procesando..."
            : "Sistema de respaldos listo";

    public string UltimoBackupTexto
    {
        get
        {
            BackupInfo? ultimo =
                Backups.FirstOrDefault();

            if (ultimo is null)
            {
                return "Sin respaldos cargados";
            }

            return ultimo.FechaCreacion
                .ToString(
                    "dd/MM/yyyy hh:mm tt");
        }
    }

    public string TamanoUltimoBackupTexto
    {
        get
        {
            BackupInfo? ultimo =
                Backups.FirstOrDefault();

            return ultimo?.TamanoFormateado
                   ?? "—";
        }
    }

    public ICommand CrearBackupCommand { get; }

    public ICommand CargarBackupCommand { get; }

    public ICommand VerificarCommand { get; }

    public ICommand RestaurarCommand { get; }

    public ICommand EliminarCommand { get; }

    public event PropertyChangedEventHandler?
        PropertyChanged;

    public Task CargarAsync()
    {
        return Task.CompletedTask;
    }

    private async Task CrearBackupAsync()
    {
        try
        {
            SaveFileDialog dialogo =
                new()
                {
                    Title =
                        "Guardar respaldo de BlaxtorGym",

                    Filter =
                        "Base de datos SQLite (*.db)|*.db",

                    FileName =
                        $"BlaxtorGym_Backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.db",

                    AddExtension =
                        true,

                    DefaultExt =
                        ".db",

                    OverwritePrompt =
                        true
                };

            bool? resultado =
                dialogo.ShowDialog();

            if (resultado != true ||
                string.IsNullOrWhiteSpace(
                    dialogo.FileName))
            {
                return;
            }

            Procesando = true;

            BackupInfo backup =
                await _backupService
                    .CrearBackupEnRutaAsync(
                        dialogo.FileName);

            AgregarOActualizarBackup(
                backup);

            BackupSeleccionado =
                backup;

            _notificationService.MostrarExito(
                $"Respaldo creado correctamente.\n\n" +
                $"{backup.NombreArchivo}\n\n" +
                $"Ubicación:\n{backup.RutaCompleta}\n\n" +
                $"Tamaño: {backup.TamanoFormateado}");
        }
        catch (Exception ex)
        {
            _notificationService.MostrarError(
                $"No fue posible crear el respaldo.\n\n{ex.Message}");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task CargarBackupAsync()
    {
        try
        {
            OpenFileDialog dialogo =
                new()
                {
                    Title =
                        "Seleccionar respaldo de BlaxtorGym",

                    Filter =
                        "Base de datos SQLite (*.db)|*.db|" +
                        "Todos los archivos (*.*)|*.*",

                    CheckFileExists =
                        true,

                    Multiselect =
                        false
                };

            bool? resultado =
                dialogo.ShowDialog();

            if (resultado != true ||
                string.IsNullOrWhiteSpace(
                    dialogo.FileName))
            {
                return;
            }

            Procesando = true;

            bool valido =
                await _backupService
                    .VerificarIntegridadAsync(
                        dialogo.FileName);

            if (!valido)
            {
                _notificationService.MostrarError(
                    "El archivo seleccionado no superó la verificación de integridad de SQLite.");

                return;
            }

            FileInfo archivo =
                new(dialogo.FileName);

            BackupInfo backup =
                new()
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

            AgregarOActualizarBackup(
                backup);

            BackupSeleccionado =
                backup;

            _notificationService.MostrarExito(
                $"Respaldo seleccionado correctamente.\n\n" +
                $"{backup.NombreArchivo}\n\n" +
                "Ahora puede utilizar RESTAURAR RESPALDO.");
        }
        catch (Exception ex)
        {
            _notificationService.MostrarError(
                $"No fue posible seleccionar el respaldo.\n\n{ex.Message}");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task VerificarBackupAsync()
    {
        if (BackupSeleccionado is null)
        {
            return;
        }

        try
        {
            Procesando = true;

            bool valido =
                await _backupService
                    .VerificarIntegridadAsync(
                        BackupSeleccionado.RutaCompleta);

            if (valido)
            {
                _notificationService.MostrarExito(
                    "El respaldo está íntegro y puede utilizarse de forma segura.");
            }
            else
            {
                _notificationService.MostrarError(
                    "SQLite detectó un problema de integridad en el respaldo.");
            }
        }
        catch (Exception ex)
        {
            _notificationService.MostrarError(
                $"No fue posible verificar el respaldo.\n\n{ex.Message}");
        }
        finally
        {
            Procesando = false;
        }
    }

    private async Task RestaurarBackupAsync()
    {
        if (BackupSeleccionado is null)
        {
            return;
        }

        MessageBoxResult confirmacion =
            MessageBox.Show(
                "Está a punto de restaurar la base de datos de BlaxtorGym.\n\n" +
                $"Respaldo:\n{BackupSeleccionado.NombreArchivo}\n\n" +
                "La información actual será reemplazada por la información del respaldo.\n\n" +
                "Antes de restaurar se conservará una copia de seguridad de la base actual.\n\n" +
                "BlaxtorGym se cerrará completamente.\n" +
                "Después deberá abrirlo nuevamente para completar la restauración.\n\n" +
                "¿Desea continuar?",
                "Restaurar respaldo",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

        if (confirmacion !=
            MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            Procesando = true;

            await _backupService
                .PrepararRestauracionAsync(
                    BackupSeleccionado.RutaCompleta);

            MessageBox.Show(
                "La restauración quedó preparada correctamente.\n\n" +
                "BlaxtorGym se cerrará ahora.\n\n" +
                "Vuelva a abrir la aplicación para completar la restauración.",
                "Restauración preparada",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            System.Windows.Application
                .Current
                .Shutdown();
        }
        catch (Exception ex)
        {
            _notificationService.MostrarError(
                $"No fue posible preparar la restauración.\n\n{ex.Message}");

            Procesando = false;
        }
    }

    private async Task EliminarBackupAsync()
    {
        if (BackupSeleccionado is null)
        {
            return;
        }

        MessageBoxResult confirmacion =
            MessageBox.Show(
                $"¿Desea eliminar permanentemente este respaldo?\n\n" +
                $"{BackupSeleccionado.NombreArchivo}\n\n" +
                "Esta acción no se puede deshacer.",
                "Eliminar respaldo",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

        if (confirmacion !=
            MessageBoxResult.Yes)
        {
            return;
        }

        string nombre =
            BackupSeleccionado.NombreArchivo;

        string ruta =
            BackupSeleccionado.RutaCompleta;

        try
        {
            Procesando = true;

            await _backupService
                .EliminarBackupAsync(
                    ruta);

            BackupInfo? existente =
                Backups.FirstOrDefault(
                    x =>
                        string.Equals(
                            x.RutaCompleta,
                            ruta,
                            StringComparison.OrdinalIgnoreCase));

            if (existente is not null)
            {
                Backups.Remove(
                    existente);
            }

            BackupSeleccionado = null;

            ActualizarResumen();

            _notificationService.MostrarExito(
                $"El respaldo fue eliminado correctamente.\n\n{nombre}");
        }
        catch (Exception ex)
        {
            _notificationService.MostrarError(
                $"No fue posible eliminar el respaldo.\n\n{ex.Message}");
        }
        finally
        {
            Procesando = false;
        }
    }

    private void AgregarOActualizarBackup(
        BackupInfo backup)
    {
        BackupInfo? existente =
            Backups.FirstOrDefault(
                x =>
                    string.Equals(
                        x.RutaCompleta,
                        backup.RutaCompleta,
                        StringComparison.OrdinalIgnoreCase));

        if (existente is not null)
        {
            Backups.Remove(
                existente);
        }

        Backups.Insert(
            0,
            backup);

        ActualizarResumen();
    }

    private void ActualizarResumen()
    {
        OnPropertyChanged(
            nameof(TotalBackups));

        OnPropertyChanged(
            nameof(UltimoBackupTexto));

        OnPropertyChanged(
            nameof(TamanoUltimoBackupTexto));

        OnPropertyChanged(
            nameof(EstadoTexto));
    }

    private void OnPropertyChanged(
        [CallerMemberName]
        string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                propertyName));
    }
}