namespace BlaxtorGym.src.BlaxtorGym.Application.DTOs;

public class ResultadoConsultaCedula
{
    public bool Encontrado { get; set; }

    public string? Cedula { get; set; }

    public string? NombreCompleto { get; set; }

    public string? Mensaje { get; set; }
}