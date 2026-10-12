using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;

public static class DatabaseSeeder
{
    public static async Task EjecutarAsync(
        AppDbContext context)
    {
        await CrearUsuarioAdministradorAsync(context);
        await CrearTiposClienteAsync(context);
        await CrearTiposMembresiaAsync(context);
        await CrearTarifasAsync(context);
    }

    private static async Task CrearUsuarioAdministradorAsync(
        AppDbContext context)
    {
        bool existeAdministrador =
            await context.Usuarios
                .AnyAsync(usuario =>
                    usuario.NombreUsuario == "admin");

        if (existeAdministrador)
        {
            return;
        }

        var administrador = new Usuario
        {
            NombreUsuario = "admin",
            NombreCompleto = "Administrador",
            ContrasenaHash =
                BCrypt.Net.BCrypt.HashPassword(
                    "Admin123*"),
            Rol = RolUsuario.Administrador,
            Activo = true,
            IntentosFallidos = 0,
            BloqueadoHasta = null
        };

        await context.Usuarios.AddAsync(
            administrador);

        await context.SaveChangesAsync();
    }

    private static async Task CrearTiposClienteAsync(
        AppDbContext context)
    {
        if (await context.TiposCliente.AnyAsync())
        {
            return;
        }

        var tiposCliente = new List<TipoCliente>
        {
            new()
            {
                Nombre = "Estándar",
                Descripcion =
                    "Cliente con tarifa estándar del gimnasio.",
                Activo = true
            },
            new()
            {
                Nombre = "Estudiante de colegio",
                Descripcion =
                    "Estudiante de colegio con tarifa especial.",
                Activo = true
            },
            new()
            {
                Nombre = "Policía",
                Descripcion =
                    "Funcionario policial con tarifa especial.",
                Activo = true
            },
            new()
            {
                Nombre = "Universitario",
                Descripcion =
                    "Estudiante universitario con tarifa especial.",
                Activo = true
            }
        };

        await context.TiposCliente
            .AddRangeAsync(tiposCliente);

        await context.SaveChangesAsync();
    }

    private static async Task CrearTiposMembresiaAsync(
        AppDbContext context)
    {
        if (await context.TiposMembresia.AnyAsync())
        {
            return;
        }

        var tiposMembresia =
            new List<TipoMembresia>
            {
                new()
                {
                    Nombre = "Sesión",
                    DuracionDias = 1,
                    Descripcion =
                        "Acceso al gimnasio durante un día.",
                    Activo = true
                },
                new()
                {
                    Nombre = "Semana",
                    DuracionDias = 7,
                    Descripcion =
                        "Membresía con duración de 7 días.",
                    Activo = true
                },
                new()
                {
                    Nombre = "Quincena",
                    DuracionDias = 15,
                    Descripcion =
                        "Membresía con duración de 15 días.",
                    Activo = true
                },
                new()
                {
                    Nombre = "Mes",
                    DuracionDias = 30,
                    Descripcion =
                        "Membresía con duración de 30 días.",
                    Activo = true
                }
            };

        await context.TiposMembresia
            .AddRangeAsync(tiposMembresia);

        await context.SaveChangesAsync();
    }

    private static async Task CrearTarifasAsync(
        AppDbContext context)
    {
        if (await context.Precios.AnyAsync())
        {
            return;
        }

        TipoCliente estandar =
            await ObtenerTipoClienteAsync(
                context,
                "Estándar");

        TipoCliente estudianteColegio =
            await ObtenerTipoClienteAsync(
                context,
                "Estudiante de colegio");

        TipoCliente policia =
            await ObtenerTipoClienteAsync(
                context,
                "Policía");

        TipoCliente universitario =
            await ObtenerTipoClienteAsync(
                context,
                "Universitario");

        TipoMembresia sesion =
            await ObtenerTipoMembresiaAsync(
                context,
                "Sesión");

        TipoMembresia semana =
            await ObtenerTipoMembresiaAsync(
                context,
                "Semana");

        TipoMembresia quincena =
            await ObtenerTipoMembresiaAsync(
                context,
                "Quincena");

        TipoMembresia mes =
            await ObtenerTipoMembresiaAsync(
                context,
                "Mes");

        DateTime fechaInicio =
            DateTime.Today;

        var tarifas = new List<Precio>
        {
            // Estándar
            CrearPrecio(
                estandar.Id,
                sesion.Id,
                2_000m,
                fechaInicio),

            CrearPrecio(
                estandar.Id,
                semana.Id,
                5_000m,
                fechaInicio),

            CrearPrecio(
                estandar.Id,
                quincena.Id,
                10_000m,
                fechaInicio),

            CrearPrecio(
                estandar.Id,
                mes.Id,
                20_000m,
                fechaInicio),

            // Estudiante de colegio
            CrearPrecio(
                estudianteColegio.Id,
                semana.Id,
                3_000m,
                fechaInicio),

            CrearPrecio(
                estudianteColegio.Id,
                mes.Id,
                12_000m,
                fechaInicio),

            // Policía
            CrearPrecio(
                policia.Id,
                semana.Id,
                4_000m,
                fechaInicio),

            CrearPrecio(
                policia.Id,
                mes.Id,
                16_000m,
                fechaInicio),

            // Universitario
            CrearPrecio(
                universitario.Id,
                mes.Id,
                15_000m,
                fechaInicio)
        };

        await context.Precios
            .AddRangeAsync(tarifas);

        await context.SaveChangesAsync();
    }

    private static Precio CrearPrecio(
        int tipoClienteId,
        int tipoMembresiaId,
        decimal monto,
        DateTime fechaInicio)
    {
        return new Precio
        {
            TipoClienteId = tipoClienteId,
            TipoMembresiaId = tipoMembresiaId,
            Monto = monto,
            FechaInicioVigencia = fechaInicio,
            FechaFinVigencia = null,
            Activo = true
        };
    }

    private static async Task<TipoCliente>
        ObtenerTipoClienteAsync(
            AppDbContext context,
            string nombre)
    {
        TipoCliente? tipoCliente =
            await context.TiposCliente
                .FirstOrDefaultAsync(
                    tipo =>
                        tipo.Nombre == nombre);

        if (tipoCliente is null)
        {
            throw new InvalidOperationException(
                $"No se encontró el tipo de cliente '{nombre}'.");
        }

        return tipoCliente;
    }

    private static async Task<TipoMembresia>
        ObtenerTipoMembresiaAsync(
            AppDbContext context,
            string nombre)
    {
        TipoMembresia? tipoMembresia =
            await context.TiposMembresia
                .FirstOrDefaultAsync(
                    tipo =>
                        tipo.Nombre == nombre);

        if (tipoMembresia is null)
        {
            throw new InvalidOperationException(
                $"No se encontró el tipo de membresía '{nombre}'.");
        }

        return tipoMembresia;
    }
}