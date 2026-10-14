namespace BlaxtorGym.src.BlaxtorGym.Application.DTOs;

public class ReporteGeneralDto
{
    public DateTime FechaGeneracion { get; set; } =
        DateTime.Now;

    public int TotalClientes { get; set; }

    public int ClientesActivos { get; set; }

    public int ClientesInactivos { get; set; }

    public int MembresiasVigentes { get; set; }

    public int MembresiasVencidas { get; set; }

    public int MembresiasPendientes { get; set; }

    public int MembresiasInactivas { get; set; }

    public int TotalPagos { get; set; }

    public decimal TotalIngresos { get; set; }

    public List<ReporteClienteItemDto> Clientes { get; set; } =
        new();

    public List<ReporteMembresiaItemDto> Membresias { get; set; } =
        new();

    public List<ReportePagoItemDto> Pagos { get; set; } =
        new();

    public List<ReporteMedicionItemDto> Mediciones { get; set; } =
        new();
}

public class ReporteClienteItemDto
{
    public int Id { get; set; }

    public string Cedula { get; set; } =
        string.Empty;

    public string NombreCompleto { get; set; } =
        string.Empty;

    public string TipoCliente { get; set; } =
        string.Empty;

    public string Telefono { get; set; } =
        string.Empty;

    public string CorreoElectronico { get; set; } =
        string.Empty;

    public DateTime FechaNacimiento { get; set; }

    public DateTime FechaRegistro { get; set; }

    public bool Activo { get; set; }

    public string Estado =>
        Activo
            ? "Activo"
            : "Inactivo";
}

public class ReporteMembresiaItemDto
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public string CedulaCliente { get; set; } =
        string.Empty;

    public string Cliente { get; set; } =
        string.Empty;

    public string TipoCliente { get; set; } =
        string.Empty;

    public int TipoMembresiaId { get; set; }

    public string TipoMembresia { get; set; } =
        string.Empty;

    public DateTime FechaInicio { get; set; }

    public DateTime FechaVencimiento { get; set; }

    public decimal PrecioAplicado { get; set; }

    public bool Activa { get; set; }

    public string Estado { get; set; } =
        string.Empty;
}

public class ReportePagoItemDto
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public string CedulaCliente { get; set; } =
        string.Empty;

    public string Cliente { get; set; } =
        string.Empty;

    public string TipoCliente { get; set; } =
        string.Empty;

    public string TipoMembresia { get; set; } =
        string.Empty;

    public decimal Monto { get; set; }

    public DateTime FechaPago { get; set; }

    public string MetodoPago { get; set; } =
        string.Empty;

    public string Observacion { get; set; } =
        string.Empty;
}

public class ReporteMedicionItemDto
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public string CedulaCliente { get; set; } =
        string.Empty;

    public string Cliente { get; set; } =
        string.Empty;

    public DateTime FechaMedicion { get; set; }

    public decimal PesoKg { get; set; }

    public decimal? EstaturaCm { get; set; }

    public decimal? PorcentajeGrasa { get; set; }

    public decimal? Imc { get; set; }

    public string Observacion { get; set; } =
        string.Empty;
}