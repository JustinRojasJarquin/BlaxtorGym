using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Usuario> Usuarios { get; set; }

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(
                DatabaseSettings.ObtenerCadenaConexion());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigurarUsuario(modelBuilder);
    }

    private static void ConfigurarUsuario(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entidad =>
        {
            entidad.ToTable("usuarios");

            entidad.HasKey(usuario => usuario.Id);

            entidad.Property(usuario => usuario.NombreUsuario)
                .IsRequired()
                .HasMaxLength(50);

            entidad.Property(usuario => usuario.ContrasenaHash)
                .IsRequired()
                .HasMaxLength(100);

            entidad.Property(usuario => usuario.NombreCompleto)
                .IsRequired()
                .HasMaxLength(100);

            entidad.Property(usuario => usuario.Rol)
                .IsRequired();

            entidad.Property(usuario => usuario.Activo)
                .IsRequired();

            entidad.Property(usuario => usuario.IntentosFallidos)
                .IsRequired();

            entidad.Property(usuario => usuario.BloqueadoHasta)
                .IsRequired(false);

            entidad.HasIndex(usuario => usuario.NombreUsuario)
                .IsUnique();
        });
    }
}