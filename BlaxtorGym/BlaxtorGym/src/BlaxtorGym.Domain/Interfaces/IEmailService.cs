using System;
using System.Collections.Generic;
using System.Text;

namespace BlaxtorGym.src.BlaxtorGym.Domain.Interfaces
{
    public interface IEmailService
    { Task EnviarAsync(string destinatario, string asunto, string contenido); }
}
