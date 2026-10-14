using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using BlaxtorGym.src.BlaxtorGym.Application.Interfaces;
using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Repositories;

public class ReporteRepository : IReporteRepository
{
    private readonly AppDbContext _context;

    public ReporteRepository(
        AppDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // CLIENTES
    // =========================================================

    public async Task<List<ReporteClienteItemDto>>
        ObtenerClientesAsync(
            ReporteFiltroDto filtro)
    {
        IQueryable<Cliente> consulta =
            _context.Clientes
                .Include(cliente =>
                    cliente.TipoCliente)
                .AsNoTracking();

        if (filtro.ClienteId.HasValue)
        {
            consulta =
                consulta.Where(
                    cliente =>
                        cliente.Id ==
                        filtro.ClienteId.Value);
        }

        if (filtro.TipoClienteId.HasValue)
        {
            consulta =
                consulta.Where(
                    cliente =>
                        cliente.TipoClienteId ==
                        filtro.TipoClienteId.Value);
        }

        if (filtro.FechaInicio.HasValue)
        {
            DateTime inicio =
                filtro.FechaInicio.Value.Date;

            consulta =
                consulta.Where(
                    cliente =>
                        cliente.FechaRegistro >=
                        inicio);
        }

        if (filtro.FechaFin.HasValue)
        {
            DateTime finExclusivo =
                filtro.FechaFin.Value.Date
                    .AddDays(1);

            consulta =
                consulta.Where(
                    cliente =>
                        cliente.FechaRegistro <
                        finExclusivo);
        }

        return await consulta
            .OrderBy(
                cliente =>
                    cliente.NombreCompleto)
            .Select(
                cliente =>
                    new ReporteClienteItemDto
                    {
                        Id =
                            cliente.Id,

                        Cedula =
                            cliente.Cedula,

                        NombreCompleto =
                            cliente.NombreCompleto,

                        TipoCliente =
                            cliente.TipoCliente.Nombre,

                        Telefono =
                            cliente.Telefono,

                        CorreoElectronico =
                            cliente.CorreoElectronico,

                        FechaNacimiento =
                            cliente.FechaNacimiento,

                        FechaRegistro =
                            cliente.FechaRegistro,

                        Activo =
                            cliente.Activo
                    })
            .ToListAsync();
    }

    // =========================================================
    // MEMBRESÍAS
    // =========================================================

    public async Task<List<ReporteMembresiaItemDto>>
        ObtenerMembresiasAsync(
            ReporteFiltroDto filtro)
    {
        IQueryable<Membresia> consulta =
            _context.Membresias
                .Include(membresia =>
                    membresia.Cliente)
                    .ThenInclude(cliente =>
                        cliente.TipoCliente)
                .Include(membresia =>
                    membresia.TipoMembresia)
                .AsNoTracking();

        if (filtro.ClienteId.HasValue)
        {
            consulta =
                consulta.Where(
                    membresia =>
                        membresia.ClienteId ==
                        filtro.ClienteId.Value);
        }

        if (filtro.TipoClienteId.HasValue)
        {
            consulta =
                consulta.Where(
                    membresia =>
                        membresia.Cliente.TipoClienteId ==
                        filtro.TipoClienteId.Value);
        }

        if (filtro.TipoMembresiaId.HasValue)
        {
            consulta =
                consulta.Where(
                    membresia =>
                        membresia.TipoMembresiaId ==
                        filtro.TipoMembresiaId.Value);
        }

        if (filtro.FechaInicio.HasValue)
        {
            DateTime inicio =
                filtro.FechaInicio.Value.Date;

            consulta =
                consulta.Where(
                    membresia =>
                        membresia.FechaInicio >=
                        inicio);
        }

        if (filtro.FechaFin.HasValue)
        {
            DateTime finExclusivo =
                filtro.FechaFin.Value.Date
                    .AddDays(1);

            consulta =
                consulta.Where(
                    membresia =>
                        membresia.FechaInicio <
                        finExclusivo);
        }

        List<Membresia> membresias =
            await consulta
                .OrderByDescending(
                    membresia =>
                        membresia.FechaInicio)
                .ToListAsync();

        IEnumerable<Membresia> resultado =
            membresias;

        if (!string.IsNullOrWhiteSpace(
                filtro.EstadoMembresia) &&
            !string.Equals(
                filtro.EstadoMembresia,
                "Todos",
                StringComparison.OrdinalIgnoreCase))
        {
            resultado =
                resultado.Where(
                    membresia =>
                        string.Equals(
                            membresia.Estado,
                            filtro.EstadoMembresia,
                            StringComparison.OrdinalIgnoreCase));
        }

        return resultado
            .Select(
                membresia =>
                    new ReporteMembresiaItemDto
                    {
                        Id =
                            membresia.Id,

                        ClienteId =
                            membresia.ClienteId,

                        CedulaCliente =
                            membresia.Cliente.Cedula,

                        Cliente =
                            membresia.Cliente.NombreCompleto,

                        TipoCliente =
                            membresia.Cliente.TipoCliente.Nombre,

                        TipoMembresiaId =
                            membresia.TipoMembresiaId,

                        TipoMembresia =
                            membresia.TipoMembresia.Nombre,

                        FechaInicio =
                            membresia.FechaInicio,

                        FechaVencimiento =
                            membresia.FechaVencimiento,

                        PrecioAplicado =
                            membresia.PrecioAplicado,

                        Activa =
                            membresia.Activa,

                        Estado =
                            membresia.Estado
                    })
            .ToList();
    }

    // =========================================================
    // PAGOS
    // =========================================================

    public async Task<List<ReportePagoItemDto>>
        ObtenerPagosAsync(
            ReporteFiltroDto filtro)
    {
        IQueryable<Pago> consulta =
            _context.Pagos
                .Include(pago =>
                    pago.Membresia)
                    .ThenInclude(membresia =>
                        membresia.Cliente)
                            .ThenInclude(cliente =>
                                cliente.TipoCliente)
                .Include(pago =>
                    pago.Membresia)
                    .ThenInclude(membresia =>
                        membresia.TipoMembresia)
                .AsNoTracking();

        if (filtro.ClienteId.HasValue)
        {
            consulta =
                consulta.Where(
                    pago =>
                        pago.Membresia.ClienteId ==
                        filtro.ClienteId.Value);
        }

        if (filtro.TipoClienteId.HasValue)
        {
            consulta =
                consulta.Where(
                    pago =>
                        pago.Membresia.Cliente.TipoClienteId ==
                        filtro.TipoClienteId.Value);
        }

        if (filtro.TipoMembresiaId.HasValue)
        {
            consulta =
                consulta.Where(
                    pago =>
                        pago.Membresia.TipoMembresiaId ==
                        filtro.TipoMembresiaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(
                filtro.MetodoPago) &&
            !string.Equals(
                filtro.MetodoPago,
                "Todos",
                StringComparison.OrdinalIgnoreCase))
        {
            consulta =
                consulta.Where(
                    pago =>
                        pago.MetodoPago ==
                        filtro.MetodoPago);
        }

        if (filtro.FechaInicio.HasValue)
        {
            DateTime inicio =
                filtro.FechaInicio.Value.Date;

            consulta =
                consulta.Where(
                    pago =>
                        pago.FechaPago >= inicio);
        }

        if (filtro.FechaFin.HasValue)
        {
            DateTime finExclusivo =
                filtro.FechaFin.Value.Date
                    .AddDays(1);

            consulta =
                consulta.Where(
                    pago =>
                        pago.FechaPago <
                        finExclusivo);
        }

        return await consulta
            .OrderByDescending(
                pago =>
                    pago.FechaPago)
            .Select(
                pago =>
                    new ReportePagoItemDto
                    {
                        Id =
                            pago.Id,

                        ClienteId =
                            pago.Membresia.ClienteId,

                        CedulaCliente =
                            pago.Membresia.Cliente.Cedula,

                        Cliente =
                            pago.Membresia.Cliente.NombreCompleto,

                        TipoCliente =
                            pago.Membresia.Cliente.TipoCliente.Nombre,

                        TipoMembresia =
                            pago.Membresia.TipoMembresia.Nombre,

                        Monto =
                            pago.Monto,

                        FechaPago =
                            pago.FechaPago,

                        MetodoPago =
                            pago.MetodoPago,

                        Observacion =
                            pago.Observacion ?? string.Empty
                    })
            .ToListAsync();
    }

    // =========================================================
    // MEDICIONES
    // =========================================================

    public async Task<List<ReporteMedicionItemDto>>
        ObtenerMedicionesAsync(
            ReporteFiltroDto filtro)
    {
        IQueryable<MedicionFisica> consulta =
            _context.MedicionesFisicas
                .Include(medicion =>
                    medicion.Cliente)
                    .ThenInclude(cliente =>
                        cliente.TipoCliente)
                .AsNoTracking();

        if (filtro.ClienteId.HasValue)
        {
            consulta =
                consulta.Where(
                    medicion =>
                        medicion.ClienteId ==
                        filtro.ClienteId.Value);
        }

        if (filtro.TipoClienteId.HasValue)
        {
            consulta =
                consulta.Where(
                    medicion =>
                        medicion.Cliente.TipoClienteId ==
                        filtro.TipoClienteId.Value);
        }

        if (filtro.FechaInicio.HasValue)
        {
            DateTime inicio =
                filtro.FechaInicio.Value.Date;

            consulta =
                consulta.Where(
                    medicion =>
                        medicion.FechaMedicion >=
                        inicio);
        }

        if (filtro.FechaFin.HasValue)
        {
            DateTime finExclusivo =
                filtro.FechaFin.Value.Date
                    .AddDays(1);

            consulta =
                consulta.Where(
                    medicion =>
                        medicion.FechaMedicion <
                        finExclusivo);
        }

        List<MedicionFisica> mediciones =
            await consulta
                .OrderByDescending(
                    medicion =>
                        medicion.FechaMedicion)
                .ToListAsync();

        return mediciones
            .Select(
                medicion =>
                    new ReporteMedicionItemDto
                    {
                        Id =
                            medicion.Id,

                        ClienteId =
                            medicion.ClienteId,

                        CedulaCliente =
                            medicion.Cliente.Cedula,

                        Cliente =
                            medicion.Cliente.NombreCompleto,

                        FechaMedicion =
                            medicion.FechaMedicion,

                        PesoKg =
                            medicion.PesoKg,

                        EstaturaCm =
                            medicion.EstaturaCm,

                        PorcentajeGrasa =
                            medicion.PorcentajeGrasa,

                        Imc =
                            medicion.Imc,

                        Observacion =
                            medicion.Observacion ??
                            string.Empty
                    })
            .ToList();
    }
}