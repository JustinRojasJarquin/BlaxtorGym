using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;

namespace BlaxtorGym.src.BlaxtorGym.Application.Services;

public class NotificacionService
{
    private const int DiasAvisoVencimiento = 3;

    private readonly INotificacionRepository _notificacionRepository;
    private readonly IMembresiaRepository _membresiaRepository;
    private readonly EmailService _emailService;

    public NotificacionService(
        INotificacionRepository notificacionRepository,
        IMembresiaRepository membresiaRepository,
        EmailService emailService)
    {
        _notificacionRepository = notificacionRepository;
        _membresiaRepository = membresiaRepository;
        _emailService = emailService;
    }

    public async Task<List<Notificacion>> ObtenerTodasAsync()
    {
        return await _notificacionRepository
            .ObtenerTodasAsync();
    }

    public async Task<List<Notificacion>> ObtenerPendientesAsync()
    {
        return await _notificacionRepository
            .ObtenerPendientesAsync();
    }

    public async Task GenerarNotificacionesAsync()
    {
        List<Membresia> membresias =
            await _membresiaRepository
                .ObtenerTodasAsync();

        DateTime hoy =
            DateTime.Today;

        DateTime limiteAviso =
            hoy.AddDays(DiasAvisoVencimiento);

        foreach (Membresia membresia in membresias)
        {
            if (membresia.Cliente is null)
            {
                continue;
            }

            if (!membresia.Cliente.Activo)
            {
                continue;
            }

            /*
             * Si la membresía fue desactivada manualmente,
             * no generamos notificaciones.
             */
            if (!membresia.Activa)
            {
                continue;
            }

            DateTime vencimiento =
                membresia.FechaVencimiento.Date;

            /*
             * Membresía ya vencida.
             */
            if (vencimiento < hoy)
            {
                await GenerarNotificacionVencidaAsync(
                    membresia);

                continue;
            }

            /*
             * Membresía que vence hoy.
             */
            if (vencimiento == hoy)
            {
                await GenerarNotificacionVenceHoyAsync(
                    membresia);

                continue;
            }

            /*
             * Membresía próxima a vencer.
             */
            if (vencimiento > hoy &&
                vencimiento <= limiteAviso)
            {
                await GenerarNotificacionProximaAVencerAsync(
                    membresia);
            }
        }
    }

    public async Task EnviarCorreoAsync(
        int notificacionId)
    {
        Notificacion? notificacion =
            await _notificacionRepository
                .ObtenerPorIdAsync(
                    notificacionId);

        if (notificacion is null)
        {
            throw new InvalidOperationException(
                "La notificación seleccionada no existe.");
        }

        if (notificacion.Enviada)
        {
            throw new InvalidOperationException(
                "La notificación ya fue enviada.");
        }

        if (notificacion.Cliente is null)
        {
            throw new InvalidOperationException(
                "La notificación no tiene un cliente asociado.");
        }

        if (string.IsNullOrWhiteSpace(
                notificacion.Cliente.CorreoElectronico))
        {
            throw new InvalidOperationException(
                "El cliente no tiene un correo electrónico registrado.");
        }

        string asunto =
            "Aviso de vencimiento de membresía - BlaxtorGym";

        string contenido =
            ConstruirContenidoCorreo(
                notificacion);

        /*
         * Primero enviamos el correo.
         *
         * Solo si MailKit confirma el envío correctamente,
         * modificamos el estado de la notificación.
         */
        await _emailService.EnviarAsync(
            notificacion.Cliente.CorreoElectronico,
            asunto,
            contenido);

        notificacion.Enviada = true;
        notificacion.FechaEnvio = DateTime.Now;

        await _notificacionRepository
            .ActualizarAsync(
                notificacion);
    }

    private static string ConstruirContenidoCorreo(
        Notificacion notificacion)
    {
        Cliente cliente =
            notificacion.Cliente;

        Membresia? membresia =
            notificacion.Membresia;

        string tipoMembresia =
            membresia?.TipoMembresia?.Nombre
            ?? "membresía";

        return
            $"Hola {cliente.NombreCompleto},\n\n" +
            $"{notificacion.Mensaje}\n\n" +
            $"Tipo de membresía: {tipoMembresia}\n" +
            $"BlaxtorGym\n\n" +
            "Si desea continuar utilizando el gimnasio, " +
            "puede realizar la renovación de su membresía.\n\n" +
            "Saludos,\n" +
            "BlaxtorGym";
    }

    private async Task GenerarNotificacionVenceHoyAsync(
        Membresia membresia)
    {
        string mensaje =
            $"La membresía de {membresia.Cliente.NombreCompleto} " +
            $"vence hoy, {membresia.FechaVencimiento:dd/MM/yyyy}.";

        await CrearSiNoExisteAsync(
            membresia,
            mensaje);
    }

    private async Task GenerarNotificacionProximaAVencerAsync(
        Membresia membresia)
    {
        int diasRestantes =
            (membresia.FechaVencimiento.Date -
             DateTime.Today).Days;

        string mensaje =
            $"La membresía de {membresia.Cliente.NombreCompleto} " +
            $"vence el {membresia.FechaVencimiento:dd/MM/yyyy} " +
            $"({diasRestantes} día" +
            $"{(diasRestantes == 1 ? "" : "s")} restante" +
            $"{(diasRestantes == 1 ? "" : "s")}).";

        await CrearSiNoExisteAsync(
            membresia,
            mensaje);
    }

    private async Task GenerarNotificacionVencidaAsync(
        Membresia membresia)
    {
        string mensaje =
            $"La membresía de {membresia.Cliente.NombreCompleto} " +
            $"venció el {membresia.FechaVencimiento:dd/MM/yyyy}.";

        await CrearSiNoExisteAsync(
            membresia,
            mensaje);
    }

    private async Task CrearSiNoExisteAsync(
        Membresia membresia,
        string mensaje)
    {
        bool existe =
            await _notificacionRepository
                .ExisteAsync(
                    membresia.Id,
                    mensaje);

        if (existe)
        {
            return;
        }

        var notificacion = new Notificacion
        {
            Mensaje = mensaje,
            FechaCreacion = DateTime.Now,
            FechaEnvio = null,
            Enviada = false,
            ClienteId = membresia.ClienteId,
            MembresiaId = membresia.Id
        };

        await _notificacionRepository
            .AgregarAsync(
                notificacion);
    }
}