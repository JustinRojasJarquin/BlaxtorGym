using BlaxtorGym.src.BlaxtorGym.Domain.Entities;
using BlaxtorGym.src.BlaxtorGym.Infraestructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<TipoCliente> TiposCliente { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<TipoMembresia> TiposMembresia { get; set; }
    public DbSet<Precio> Precios { get; set; }
    public DbSet<Membresia> Membresias { get; set; }
    public DbSet<Pago> Pagos { get; set; }
    public DbSet<MedicionFisica> MedicionesFisicas { get; set; }
    public DbSet<Notificacion> Notificaciones { get; set; }
    public DbSet<BitacoraCambio> BitacoraCambios { get; set; }

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
        ConfigurarTipoCliente(modelBuilder);
        ConfigurarCliente(modelBuilder);
        ConfigurarTipoMembresia(modelBuilder);
        ConfigurarPrecio(modelBuilder);
        ConfigurarMembresia(modelBuilder);
        ConfigurarPago(modelBuilder);
        ConfigurarMedicionFisica(modelBuilder);
        ConfigurarNotificacion(modelBuilder);
        ConfigurarBitacoraCambio(modelBuilder);
    }

    private static void ConfigurarUsuario(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entidad =>
        {
            entidad.ToTable("usuarios");

            entidad.HasKey(x => x.Id);

            entidad.Property(x => x.NombreUsuario)
                .IsRequired()
                .HasMaxLength(50);

            entidad.Property(x => x.ContrasenaHash)
                .IsRequired()
                .HasMaxLength(100);

            entidad.Property(x => x.NombreCompleto)
                .IsRequired()
                .HasMaxLength(100);

            entidad.Property(x => x.Rol)
                .IsRequired();

            entidad.Property(x => x.Activo)
                .IsRequired();

            entidad.Property(x => x.IntentosFallidos)
                .IsRequired();

            entidad.HasIndex(x => x.NombreUsuario)
                .IsUnique();
        });
    }

    private static void ConfigurarTipoCliente(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TipoCliente>(entidad =>
        {
            entidad.ToTable("tipos_cliente");

            entidad.HasKey(x => x.Id);

            entidad.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            entidad.Property(x => x.Descripcion)
                .HasMaxLength(250);

            entidad.Property(x => x.Activo)
                .IsRequired();

            entidad.HasIndex(x => x.Nombre)
                .IsUnique();
        });
    }

    private static void ConfigurarCliente(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entidad =>
        {
            entidad.ToTable("clientes");

            entidad.HasKey(x => x.Id);

            entidad.Property(x => x.Cedula)
                .IsRequired()
                .HasMaxLength(20);

            entidad.Property(x => x.NombreCompleto)
                .IsRequired()
                .HasMaxLength(120);

            entidad.Property(x => x.Telefono)
                .HasMaxLength(20);

            entidad.Property(x => x.CorreoElectronico)
                .HasMaxLength(150);

            entidad.Property(x => x.FechaNacimiento)
                .IsRequired();

            entidad.Property(x => x.FechaRegistro)
                .IsRequired();

            entidad.Property(x => x.Activo)
                .IsRequired();

            entidad.HasIndex(x => x.Cedula)
                .IsUnique();

            entidad.HasIndex(x => x.NombreCompleto);

            entidad.HasOne(x => x.TipoCliente)
                .WithMany()
                .HasForeignKey(x => x.TipoClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurarTipoMembresia(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TipoMembresia>(entidad =>
        {
            entidad.ToTable("tipos_membresia");

            entidad.HasKey(x => x.Id);

            entidad.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            entidad.Property(x => x.DuracionDias)
                .IsRequired();

            entidad.Property(x => x.Descripcion)
                .HasMaxLength(250);

            entidad.Property(x => x.Activo)
                .IsRequired();

            entidad.HasIndex(x => x.Nombre)
                .IsUnique();
        });
    }

    private static void ConfigurarPrecio(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Precio>(entidad =>
        {
            entidad.ToTable("precios");

            entidad.HasKey(x => x.Id);

            entidad.Property(x => x.Monto)
                .IsRequired()
                .HasPrecision(12, 2);

            entidad.Property(x => x.FechaInicioVigencia)
                .IsRequired();

            entidad.Property(x => x.Activo)
                .IsRequired();

            entidad.HasOne(x => x.TipoCliente)
                .WithMany()
                .HasForeignKey(x => x.TipoClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasOne(x => x.TipoMembresia)
                .WithMany()
                .HasForeignKey(x => x.TipoMembresiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasIndex(x => new
            {
                x.TipoClienteId,
                x.TipoMembresiaId,
                x.Activo
            });
        });
    }

    private static void ConfigurarMembresia(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Membresia>(entidad =>
        {
            entidad.ToTable("membresias");

            entidad.HasKey(x => x.Id);

            entidad.Property(x => x.FechaInicio)
                .IsRequired();

            entidad.Property(x => x.FechaVencimiento)
                .IsRequired();

            entidad.Property(x => x.PrecioAplicado)
                .IsRequired()
                .HasPrecision(12, 2);

            entidad.Property(x => x.Activa)
                .IsRequired();

            entidad.HasOne(x => x.Cliente)
                .WithMany()
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasOne(x => x.TipoMembresia)
                .WithMany()
                .HasForeignKey(x => x.TipoMembresiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasOne(x => x.Precio)
                .WithMany()
                .HasForeignKey(x => x.PrecioId)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasIndex(x => x.ClienteId);

            entidad.HasIndex(x => new
            {
                x.Activa,
                x.FechaVencimiento
            });
        });
    }

    private static void ConfigurarPago(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pago>(entidad =>
        {
            entidad.ToTable("pagos");

            entidad.HasKey(x => x.Id);

            entidad.Property(x => x.Monto)
                .IsRequired()
                .HasPrecision(12, 2);

            entidad.Property(x => x.FechaPago)
                .IsRequired();

            entidad.Property(x => x.MetodoPago)
                .IsRequired()
                .HasMaxLength(50);

            entidad.Property(x => x.Observacion)
                .HasMaxLength(500);

            entidad.HasOne(x => x.Membresia)
                .WithMany()
                .HasForeignKey(x => x.MembresiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasIndex(x => x.MembresiaId);

            entidad.HasIndex(x => x.FechaPago);
        });
    }

    private static void ConfigurarMedicionFisica(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MedicionFisica>(entidad =>
        {
            entidad.ToTable("mediciones_fisicas");

            entidad.HasKey(x => x.Id);

            entidad.Property(x => x.FechaMedicion)
                .IsRequired();

            entidad.Property(x => x.PesoKg)
                .IsRequired()
                .HasPrecision(6, 2);

            entidad.Property(x => x.EstaturaCm)
                .HasPrecision(6, 2);

            entidad.Property(x => x.PorcentajeGrasa)
                .HasPrecision(5, 2);

            entidad.Property(x => x.Observacion)
                .HasMaxLength(500);

            entidad.HasOne(x => x.Cliente)
                .WithMany()
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasIndex(x => new
            {
                x.ClienteId,
                x.FechaMedicion
            });
        });
    }

    private static void ConfigurarNotificacion(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notificacion>(entidad =>
        {
            entidad.ToTable("notificaciones");

            entidad.HasKey(x => x.Id);

            entidad.Property(x => x.Mensaje)
                .IsRequired()
                .HasMaxLength(500);

            entidad.Property(x => x.FechaCreacion)
                .IsRequired();

            entidad.Property(x => x.Enviada)
                .IsRequired();

            entidad.HasOne(x => x.Cliente)
                .WithMany()
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasOne(x => x.Membresia)
                .WithMany()
                .HasForeignKey(x => x.MembresiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasIndex(x => new
            {
                x.Enviada,
                x.FechaCreacion
            });

            entidad.HasIndex(x => x.MembresiaId);
        });
    }

    private static void ConfigurarBitacoraCambio(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BitacoraCambio>(entidad =>
        {
            entidad.ToTable("bitacora_cambios");

            entidad.HasKey(x => x.Id);

            entidad.Property(x => x.FechaHora)
                .IsRequired();

            entidad.Property(x => x.Accion)
                .IsRequired()
                .HasMaxLength(100);

            entidad.Property(x => x.Entidad)
                .IsRequired()
                .HasMaxLength(100);

            entidad.Property(x => x.Descripcion)
                .HasMaxLength(1000);

            entidad.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entidad.HasIndex(x => x.FechaHora);

            entidad.HasIndex(x => new
            {
                x.Entidad,
                x.EntidadId
            });
        });
    }
}