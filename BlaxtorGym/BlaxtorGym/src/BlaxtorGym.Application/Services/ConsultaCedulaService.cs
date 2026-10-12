using BlaxtorGym.src.BlaxtorGym.Application.DTOs;
using System.Net.Http;
using System.Text.Json;

namespace BlaxtorGym.src.BlaxtorGym.Application.Services;

public class ConsultaCedulaService
{
    private readonly HttpClient _httpClient;

    public ConsultaCedulaService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResultadoConsultaCedula> ConsultarAsync(
        string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula))
        {
            return new ResultadoConsultaCedula
            {
                Encontrado = false,
                Mensaje = "Debe ingresar una cédula."
            };
        }

        string cedulaLimpia =
            LimpiarCedula(cedula);

        if (cedulaLimpia.Length < 9)
        {
            return new ResultadoConsultaCedula
            {
                Encontrado = false,
                Cedula = cedulaLimpia,
                Mensaje = "La cédula ingresada no tiene un formato válido."
            };
        }

        try
        {
            string url =
                $"https://apis.gometa.org/cedulas/{Uri.EscapeDataString(cedulaLimpia)}";

            using HttpResponseMessage respuesta =
                await _httpClient.GetAsync(url);

            if (!respuesta.IsSuccessStatusCode)
            {
                return new ResultadoConsultaCedula
                {
                    Encontrado = false,
                    Cedula = cedulaLimpia,
                    Mensaje =
                        "No fue posible consultar la cédula en este momento."
                };
            }

            string json =
                await respuesta.Content.ReadAsStringAsync();

            return ProcesarRespuesta(
                json,
                cedulaLimpia);
        }
        catch (HttpRequestException)
        {
            return new ResultadoConsultaCedula
            {
                Encontrado = false,
                Cedula = cedulaLimpia,
                Mensaje =
                    "No fue posible conectarse con el servicio de consulta de cédulas."
            };
        }
        catch (TaskCanceledException)
        {
            return new ResultadoConsultaCedula
            {
                Encontrado = false,
                Cedula = cedulaLimpia,
                Mensaje =
                    "La consulta de cédula tardó demasiado tiempo."
            };
        }
        catch (JsonException)
        {
            return new ResultadoConsultaCedula
            {
                Encontrado = false,
                Cedula = cedulaLimpia,
                Mensaje =
                    "El servicio devolvió una respuesta que no pudo ser procesada."
            };
        }
    }

    private static ResultadoConsultaCedula ProcesarRespuesta(
        string json,
        string cedula)
    {
        using JsonDocument documento =
            JsonDocument.Parse(json);

        JsonElement raiz =
            documento.RootElement;

        /*
         * GoMeta puede devolver una colección de resultados.
         * Buscamos primero un arreglo directamente en la raíz.
         */
        if (raiz.ValueKind == JsonValueKind.Array)
        {
            return BuscarEnArreglo(
                raiz,
                cedula);
        }

        /*
         * También contemplamos respuestas donde los resultados
         * vengan contenidos dentro de una propiedad.
         */
        if (raiz.ValueKind == JsonValueKind.Object)
        {
            if (raiz.TryGetProperty(
                    "results",
                    out JsonElement resultados) &&
                resultados.ValueKind == JsonValueKind.Array)
            {
                return BuscarEnArreglo(
                    resultados,
                    cedula);
            }

            if (raiz.TryGetProperty(
                    "resultados",
                    out JsonElement resultadosEspanol) &&
                resultadosEspanol.ValueKind == JsonValueKind.Array)
            {
                return BuscarEnArreglo(
                    resultadosEspanol,
                    cedula);
            }

            ResultadoConsultaCedula? resultadoDirecto =
                ExtraerResultado(
                    raiz,
                    cedula);

            if (resultadoDirecto is not null)
            {
                return resultadoDirecto;
            }
        }

        return new ResultadoConsultaCedula
        {
            Encontrado = false,
            Cedula = cedula,
            Mensaje = "No se encontró información para la cédula indicada."
        };
    }

    private static ResultadoConsultaCedula BuscarEnArreglo(
        JsonElement arreglo,
        string cedula)
    {
        foreach (JsonElement elemento in arreglo.EnumerateArray())
        {
            ResultadoConsultaCedula? resultado =
                ExtraerResultado(
                    elemento,
                    cedula);

            if (resultado is not null)
            {
                return resultado;
            }
        }

        return new ResultadoConsultaCedula
        {
            Encontrado = false,
            Cedula = cedula,
            Mensaje = "No se encontró información para la cédula indicada."
        };
    }

    private static ResultadoConsultaCedula? ExtraerResultado(
        JsonElement elemento,
        string cedulaBuscada)
    {
        if (elemento.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string? nombre =
            ObtenerTexto(
                elemento,
                "nombre")
            ?? ObtenerTexto(
                elemento,
                "Nombre")
            ?? ObtenerTexto(
                elemento,
                "fullname")
            ?? ObtenerTexto(
                elemento,
                "name");

        if (string.IsNullOrWhiteSpace(nombre))
        {
            return null;
        }

        string? cedulaResultado =
            ObtenerTexto(
                elemento,
                "cedula")
            ?? ObtenerTexto(
                elemento,
                "Cedula")
            ?? ObtenerTexto(
                elemento,
                "identificacion");

        return new ResultadoConsultaCedula
        {
            Encontrado = true,
            Cedula = string.IsNullOrWhiteSpace(cedulaResultado)
                ? cedulaBuscada
                : cedulaResultado,
            NombreCompleto = nombre.Trim(),
            Mensaje = "Cédula encontrada correctamente."
        };
    }

    private static string? ObtenerTexto(
        JsonElement elemento,
        string propiedad)
    {
        if (!elemento.TryGetProperty(
                propiedad,
                out JsonElement valor))
        {
            return null;
        }

        if (valor.ValueKind == JsonValueKind.String)
        {
            return valor.GetString();
        }

        if (valor.ValueKind == JsonValueKind.Number)
        {
            return valor.ToString();
        }

        return null;
    }

    private static string LimpiarCedula(
        string cedula)
    {
        return new string(
            cedula
                .Where(char.IsDigit)
                .ToArray());
    }
}