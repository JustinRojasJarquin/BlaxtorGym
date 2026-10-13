using BlaxtorGym.src.BlaxtorGym.Application.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BlaxtorGym.src.BlaxtorGym.Application.Services;

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(
        EmailSettings settings)
    {
        _settings = settings;
    }

    public async Task EnviarAsync(
        string destinatario,
        string asunto,
        string contenido)
    {
        ValidarDestinatario(destinatario);

        if (string.IsNullOrWhiteSpace(asunto))
        {
            throw new ArgumentException(
                "El asunto del correo es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(contenido))
        {
            throw new ArgumentException(
                "El contenido del correo es obligatorio.");
        }

        var mensaje = new MimeMessage();

        mensaje.From.Add(
            new MailboxAddress(
                _settings.NombreRemitente,
                _settings.Remitente));

        mensaje.To.Add(
            MailboxAddress.Parse(
                destinatario.Trim()));

        mensaje.Subject =
            asunto.Trim();

        mensaje.Body =
            new TextPart("plain")
            {
                Text = contenido.Trim()
            };

        using var smtpClient =
            new SmtpClient();

        try
        {
            await smtpClient.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await smtpClient.AuthenticateAsync(
                _settings.Usuario,
                _settings.ContrasenaAplicacion);

            await smtpClient.SendAsync(
                mensaje);
        }
        finally
        {
            if (smtpClient.IsConnected)
            {
                await smtpClient.DisconnectAsync(
                    true);
            }
        }
    }

    private static void ValidarDestinatario(
        string destinatario)
    {
        if (string.IsNullOrWhiteSpace(destinatario))
        {
            throw new ArgumentException(
                "El correo del destinatario es obligatorio.");
        }

        try
        {
            MailboxAddress.Parse(
                destinatario.Trim());
        }
        catch (ParseException)
        {
            throw new ArgumentException(
                "El correo del destinatario no tiene un formato válido.");
        }
    }
}