using ModelContextProtocol.Server;
using System.ComponentModel;

// Funcion para crear la aplicación web
var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios MCP y HTTP
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

// Configuración del cliente HTTP para la API de Figma
builder.Services.AddHttpClient("FigmaClient", client =>
{
    // Configura la URL base para la API de Figma
    client.BaseAddress = new Uri("https://api.figma.com/v1/");
});

// Registro del servicio de token de autenticación
builder.Services.AddSingleton<IAuthTokenService, FigmaTokenService>();
// Registro de la clase de herramientas Figma
builder.Services.AddTransient<FigmaTools>();
// Construcción de la aplicación
var app = builder.Build();
// Mapeo del endpoint MCP
app.MapMcp(pattern: "/api/mcp");
// Ejecución de la aplicación
await app.RunAsync();

// Servicio para gestionar el token de autenticación de Figma
public interface IAuthTokenService
{
    Task<string> GetTokenAsync();
}

// Implementación del servicio de token de autenticación de Figma
public class FigmaTokenService : IAuthTokenService
{   // Token de acceso personal de Figma.
    private string _figmaToken = /*Agregrar el token de Figma*/;

    // Método para obtener el token de autenticación
    public Task<string> GetTokenAsync()
    {
        return Task.FromResult(_figmaToken);
    }
}
// Clase que contiene las herramientas para interactuar con la API de Figma
[McpServerToolType]
public class FigmaTools
{   // Cliente HTTP para realizar solicitudes a la API de Figma
    private readonly HttpClient _figmaClient;
    private readonly IAuthTokenService _tokenService;
    private readonly ILogger<FigmaTools> _logger;
    // Constructor que inyecta las dependencias necesarias
    public FigmaTools(IHttpClientFactory httpClientFactory, IAuthTokenService tokenService, ILogger<FigmaTools> logger)
    {
        _figmaClient = httpClientFactory.CreateClient("FigmaClient");
        _tokenService = tokenService;
        _logger = logger;
    }
    // Herramienta para obtener el contenido completo de un archivo de Figma
    [McpServerTool, Description("Obtiene la estructura JSON completa del documento Figma usando su ID de archivo.")]
    public async Task<string> GetFigmaFileContent(
        [Description("El ID (Key) del archivo de Figma.")]
        string fileId)
    {
        var endpoint = $"files/{fileId}";
        return await ExecuteAuthenticatedGet(endpoint, $"Archivo Figma con ID: {fileId}");
    }
    // Herramienta para obtener información detallada de un nodo específico dentro de un archivo de Figma
    [McpServerTool, Description("Obtiene la información detallada (JSON) de un nodo específico dentro de un archivo de Figma.")]
    // Un nodo puede ser una capa, componente, marco, etc.
    public async Task<string> GetFigmaNode(
        [Description("El ID (Key) del archivo de Figma.")]
        string fileId,
        [Description("El ID del nodo (capa, componente, marco) dentro del archivo.")]
        string nodeId)
    {
       
        var encodedNodeId = Uri.EscapeDataString(nodeId);
        var endpoint = $"files/{fileId}/nodes?ids={encodedNodeId}";
        return await ExecuteAuthenticatedGet(endpoint, $"Nodo Figma: {nodeId} en Archivo: {fileId}");
    }

    // Método privado para ejecutar solicitudes GET autenticadas a la API de Figma
    // Este método maneja la adición del token de autenticación y el procesamiento de la respuesta
    // Devuelve el contenido JSON o un mensaje de error según corresponda
    private async Task<string> ExecuteAuthenticatedGet(string endpoint, string resourceName)
    {
        try
        {
            var token = await _tokenService.GetTokenAsync();

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, endpoint);
            requestMessage.Headers.Add("X-Figma-Token", token);

            var response = await _figmaClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return $"Contenido de {resourceName} (JSON): {content}";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"Error al consultar {resourceName}. Status: {response.StatusCode}. Respuesta del servidor: {error}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocurrió un error al intentar acceder a {resourceName}");
            return $"Ocurrió un error al intentar acceder a {resourceName}: {ex.Message}";
        }
    }
}