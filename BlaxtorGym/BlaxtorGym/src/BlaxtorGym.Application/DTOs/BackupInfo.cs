namespace BlaxtorGym.src.BlaxtorGym.Application.DTOs;

public class BackupInfo
{
    public string NombreArchivo { get; set; } = string.Empty;

    public string RutaCompleta { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; }

    public long TamanoBytes { get; set; }

    public string TamanoFormateado
    {
        get
        {
            if (TamanoBytes < 1024)
            {
                return $"{TamanoBytes} B";
            }

            double kilobytes =
                TamanoBytes / 1024d;

            if (kilobytes < 1024)
            {
                return $"{kilobytes:N1} KB";
            }

            double megabytes =
                kilobytes / 1024d;

            if (megabytes < 1024)
            {
                return $"{megabytes:N1} MB";
            }

            double gigabytes =
                megabytes / 1024d;

            return $"{gigabytes:N2} GB";
        }
    }
}