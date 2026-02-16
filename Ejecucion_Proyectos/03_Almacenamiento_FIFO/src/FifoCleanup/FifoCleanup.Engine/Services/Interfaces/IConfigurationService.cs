namespace FifoCleanup.Engine.Services.Interfaces;

/// <summary>
/// Servicio de gestión de configuración.
/// Lee/escribe la configuración JSON del sistema FIFO.
/// </summary>
public interface IConfigurationService
{
    /// <summary>Cargar configuración desde archivo JSON</summary>
    Task<Models.FifoConfiguration> LoadAsync(string path);

    /// <summary>Guardar configuración a archivo JSON</summary>
    Task SaveAsync(Models.FifoConfiguration config, string path);

    /// <summary>Validar configuración y retornar lista de errores</summary>
    List<string> Validate(Models.FifoConfiguration config);

    /// <summary>Obtener configuración por defecto</summary>
    Models.FifoConfiguration GetDefault();
}
