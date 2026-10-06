using BlaxtorGym.src.BlaxtorGym.Domain.Entities;

namespace BlaxtorGym.src.BlaxtorGym.Application.DTOs;

public class ResultadoLogin
{
    public bool Exitoso { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public Usuario? Usuario { get; set; }
}