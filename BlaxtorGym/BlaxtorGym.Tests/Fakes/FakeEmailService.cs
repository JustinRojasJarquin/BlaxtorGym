using BlaxtorGym.src.BlaxtorGym.Domain.Interfaces;
namespace BlaxtorGym.Tests.Fakes;
public sealed class FakeEmailService : IEmailService
{
    public string? Destinatario { get; private set; } public string? Asunto { get; private set; } public string? Contenido { get; private set; } public int CantidadLlamadas { get; private set; }
    public Task EnviarAsync(string destinatario, string asunto, string contenido) { Destinatario = destinatario; Asunto = asunto; Contenido = contenido; CantidadLlamadas++; return Task.CompletedTask; }
}
