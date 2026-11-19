using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection;
using System.IO;

namespace BlaxtorGym.src.BlaxtorGym.Infraestructure.Services;

public class PdfService
{
    private static readonly byte[]? Logo = CargarLogo();
    public Task GenerarAsync(ReporteGeneralDto reporte, string titulo, string ruta)
    {
        ArgumentNullException.ThrowIfNull(reporte);
        if (string.IsNullOrWhiteSpace(ruta)) throw new ArgumentException("La ruta del PDF es obligatoria.", nameof(ruta));
        if (string.IsNullOrWhiteSpace(titulo)) throw new ArgumentException("El título del reporte es obligatorio.", nameof(titulo));

        Document.Create(document => document.Page(page =>
        {
            page.Size(PageSizes.A4); page.Margin(34); page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontFamily("Lato").FontSize(9).FontColor(Colors.Grey.Darken3));
            page.Header().Element(header => Header(header, reporte, titulo));
            page.Content().PaddingTop(18).Column(column =>
            {
                column.Spacing(14);
                column.Item().Element(container => Filters(container, titulo));
                column.Item().Element(container => Summary(container, reporte, titulo));
                if (reporte.Clientes.Count > 0) column.Item().Element(container => Clientes(container, reporte.Clientes));
                if (reporte.Membresias.Count > 0) column.Item().Element(container => Membresias(container, reporte.Membresias));
                if (reporte.Pagos.Count > 0) column.Item().Element(container => Pagos(container, reporte.Pagos));
                if (reporte.Mediciones.Count > 0) column.Item().Element(container => Mediciones(container, reporte.Mediciones));
                if (reporte.Clientes.Count == 0 && reporte.Membresias.Count == 0 && reporte.Pagos.Count == 0 && reporte.Mediciones.Count == 0) column.Item().PaddingTop(20).AlignCenter().Text("No se encontraron registros para los filtros seleccionados.").Italic();
            });
            page.Footer().AlignCenter().Text(text => { text.Span("BlaxtorGym   ·   "); text.CurrentPageNumber(); text.Span(" de "); text.TotalPages(); });
        })).GeneratePdf(ruta);
        return Task.CompletedTask;
    }

    private static void Header(IContainer container, ReporteGeneralDto reporte, string titulo) => container.Column(c => { c.Item().Row(row => { row.ConstantItem(62).Height(62).AlignMiddle().Element(x => { if (Logo is not null) x.Image(Logo).FitArea(); else x.AlignCenter().AlignMiddle().Text("BLX GYM").Bold().FontColor(Colors.Pink.Medium); }); row.RelativeItem().PaddingLeft(12).Column(x => { x.Item().Text("Gestión Blaxtor Gym").FontSize(22).Bold().FontColor(Colors.Pink.Medium); x.Item().PaddingTop(6).Text(titulo.Trim().ToUpperInvariant()).FontSize(15).Bold().FontColor(Colors.Grey.Darken4); }); row.ConstantItem(150).AlignRight().AlignMiddle().Text($"Generado\n{reporte.FechaGeneracion:dd/MM/yyyy HH:mm}").FontSize(8); }); c.Item().PaddingTop(12).LineHorizontal(1).LineColor(Colors.Pink.Medium); });
    private static byte[]? CargarLogo() { using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BlaxtorGym.logo.png"); if (stream is null) return null; using var memory = new MemoryStream(); stream.CopyTo(memory); return memory.ToArray(); }
    private static void Filters(IContainer container, string titulo) => container.Background(Colors.Grey.Lighten4).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Text(titulo.Contains("GENERAL", StringComparison.OrdinalIgnoreCase) ? "Información general del sistema" : "Sin filtros aplicados").FontSize(9);
    private static void Summary(IContainer container, ReporteGeneralDto r, string titulo) => container.Column(c => { c.Item().Text("RESUMEN").FontSize(11).Bold().FontColor(Colors.Grey.Darken4); c.Item().PaddingTop(6).Table(t => { t.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); columns.RelativeColumn(); }); Cell(t, "Clientes", r.TotalClientes.ToString()); Cell(t, "Activos", r.ClientesActivos.ToString()); Cell(t, "Inactivos", r.ClientesInactivos.ToString()); Cell(t, "Membresías vigentes", r.MembresiasVigentes.ToString()); Cell(t, "Membresías vencidas", r.MembresiasVencidas.ToString()); Cell(t, "Pagos", r.TotalPagos.ToString()); Cell(t, "Ingresos", $"₡{r.TotalIngresos:N2}"); }); });
    private static void Cell(TableDescriptor table, string label, string value) => table.Cell().Background(Colors.Grey.Lighten4).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(7).Column(c => { c.Item().Text(label).FontSize(7).FontColor(Colors.Grey.Darken1); c.Item().Text(value).Bold(); });
    private static void TableHeader(TableDescriptor t, params string[] headers) { foreach (var h in headers) t.Cell().Background(Colors.Grey.Darken3).Padding(5).Text(h).FontColor(Colors.White).Bold().FontSize(8); }
    private static void Clientes(IContainer container, List<ReporteClienteItemDto> rows) => container.Column(c => { c.Item().Text("CLIENTES").Bold().FontSize(11); c.Item().Table(t => { t.ColumnsDefinition(x => { x.RelativeColumn(1); x.RelativeColumn(2); x.RelativeColumn(1.5f); x.RelativeColumn(2); x.RelativeColumn(1); x.RelativeColumn(1); }); TableHeader(t, "Cédula", "Nombre", "Teléfono", "Correo", "Tipo", "Estado"); foreach (var r in rows) { t.Cell().Text(r.Cedula); t.Cell().Text(r.NombreCompleto); t.Cell().Text(r.Telefono); t.Cell().Text(r.CorreoElectronico); t.Cell().Text(r.TipoCliente); t.Cell().Text(r.Estado); } }); });
    private static void Membresias(IContainer container, List<ReporteMembresiaItemDto> rows) => container.Column(c => { c.Item().Text("MEMBRESÍAS").Bold().FontSize(11); c.Item().Table(t => { t.ColumnsDefinition(x => { x.RelativeColumn(2); x.RelativeColumn(1.5f); x.RelativeColumn(1.2f); x.RelativeColumn(1.2f); x.RelativeColumn(1.2f); x.RelativeColumn(1); }); TableHeader(t, "Cliente", "Tipo", "Inicio", "Vencimiento", "Precio", "Estado"); foreach (var r in rows) { t.Cell().Text(r.Cliente); t.Cell().Text(r.TipoMembresia); t.Cell().Text(r.FechaInicio.ToString("dd/MM/yyyy")); t.Cell().Text(r.FechaVencimiento.ToString("dd/MM/yyyy")); t.Cell().Text($"₡{r.PrecioAplicado:N2}"); t.Cell().Text(r.Estado); } }); });
    private static void Pagos(IContainer container, List<ReportePagoItemDto> rows) => container.Column(c => { c.Item().Text("PAGOS E INGRESOS").Bold().FontSize(11); c.Item().Table(t => { t.ColumnsDefinition(x => { x.RelativeColumn(1.2f); x.RelativeColumn(2); x.RelativeColumn(1.4f); x.RelativeColumn(1.3f); x.RelativeColumn(1.3f); }); TableHeader(t, "Fecha", "Cliente", "Membresía", "Monto", "Método"); foreach (var r in rows) { t.Cell().Text(r.FechaPago.ToString("dd/MM/yyyy")); t.Cell().Text(r.Cliente); t.Cell().Text(r.TipoMembresia); t.Cell().Text($"₡{r.Monto:N2}"); t.Cell().Text(r.MetodoPago); } }); });
    private static void Mediciones(IContainer container, List<ReporteMedicionItemDto> rows) => container.Column(c => { c.Item().Text("MEDICIONES FÍSICAS").Bold().FontSize(11); c.Item().Table(t => { t.ColumnsDefinition(x => { x.RelativeColumn(1.2f); x.RelativeColumn(2); x.RelativeColumn(1.2f); x.RelativeColumn(1.2f); x.RelativeColumn(1.2f); }); TableHeader(t, "Fecha", "Cliente", "Peso (kg)", "Estatura (cm)", "Grasa (%)"); foreach (var r in rows) { t.Cell().Text(r.FechaMedicion.ToString("dd/MM/yyyy")); t.Cell().Text(r.Cliente); t.Cell().Text(r.PesoKg.ToString("N2")); t.Cell().Text(r.EstaturaCm?.ToString("N2") ?? "—"); t.Cell().Text(r.PorcentajeGrasa?.ToString("N2") ?? "—"); } }); c.Item().PaddingTop(5).Table(t => { t.ColumnsDefinition(x => { x.RelativeColumn(1.2f); x.RelativeColumn(2); x.RelativeColumn(1); x.RelativeColumn(1); x.RelativeColumn(1); x.RelativeColumn(1); x.RelativeColumn(1); x.RelativeColumn(1); }); TableHeader(t, "Fecha", "Cliente", "Hombros", "Pecho", "Bíceps D/I", "Cintura", "Muslo D/I", "cm"); foreach (var r in rows) { t.Cell().Text(r.FechaMedicion.ToString("dd/MM/yyyy")); t.Cell().Text(r.Cliente); t.Cell().Text(r.Hombros?.ToString("N2") ?? "—"); t.Cell().Text(r.Pecho?.ToString("N2") ?? "—"); t.Cell().Text($"{r.BicepsDerecho?.ToString("N2") ?? "—"} / {r.BicepsIzquierdo?.ToString("N2") ?? "—"}"); t.Cell().Text(r.Cintura?.ToString("N2") ?? "—"); t.Cell().Text($"{r.MusloDerecho?.ToString("N2") ?? "—"} / {r.MusloIzquierdo?.ToString("N2") ?? "—"}"); t.Cell().Text("cm"); } }); });
}
