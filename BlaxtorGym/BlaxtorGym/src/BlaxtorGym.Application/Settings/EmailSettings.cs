using System.IO;
using System.Text.Json;

namespace BlaxtorGym.src.BlaxtorGym.Application.Settings;

public class EmailSettings
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Usuario { get; set; } = string.Empty;

    public string ContrasenaAplicacion { get; set; } = string.Empty;

    public string Remitente { get; set; } = string.Empty;

    public string NombreRemitente { get; set; } = "BlaxtorGym";

    public static async Task<EmailSettings> CargarAsync(
        string rutaArchivo)
    {
        if (!File.Exists(rutaArchivo))
        {
            throw new FileNotFoundException(
                "No se encontró el archivo local de configuración de correo.",
                rutaArchivo);
        }

        string json =
            await File.ReadAllTextAsync(rutaArchivo);

        EmailSettings? configuracion =
            JsonSerializer.Deserialize<EmailSettings>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (configuracion is null)
        {
            throw new InvalidOperationException(
                "No fue posible leer la configuración de correo.");
        }

        configuracion.Validar();

        return configuracion;
    }

    private void Validar()
    {
        if (string.IsNullOrWhiteSpace(Host))
        {
            throw new InvalidOperationException(
                "El servidor SMTP no está configurado.");
        }

        if (Port <= 0)
        {
            throw new InvalidOperationException(
                "El puerto SMTP no es válido.");
        }

        if (string.IsNullOrWhiteSpace(Usuario))
        {
            throw new InvalidOperationException(
                "El usuario de correo no está configurado.");
        }

        if (string.IsNullOrWhiteSpace(ContrasenaAplicacion))
        {
            throw new InvalidOperationException(
                "La contraseña de aplicación no está configurada.");
        }

        if (string.IsNullOrWhiteSpace(Remitente))
        {
            throw new InvalidOperationException(
                "El correo remitente no está configurado.");
        }

        /*
         * Google suele mostrar las contraseñas de aplicación
         * separadas por espacios. SMTP necesita la contraseña
         * sin esos espacios.
         */
        ContrasenaAplicacion =
            ContrasenaAplicacion.Replace(
                " ",
                string.Empty);
    }
}