using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;

public static class DatabaseSeeder
{
    public static async Task EjecutarAsync(AppDbContext context)
    {
        await CrearUsuarioAdministradorAsync(context);
    }

    private static async Task CrearUsuarioAdministradorAsync(
        AppDbContext context)
    {
        bool existeAdministrador = await context.Usuarios
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
            ContrasenaHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
            Rol = RolUsuario.Administrador,
            Activo = true,
            IntentosFallidos = 0,
            BloqueadoHasta = null
        };

        await context.Usuarios.AddAsync(administrador);
        await context.SaveChangesAsync();
    }
}