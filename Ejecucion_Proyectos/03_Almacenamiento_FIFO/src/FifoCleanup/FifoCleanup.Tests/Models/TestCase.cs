namespace FifoCleanup.Tests.Models;

public class TestCase
{
    public string Id { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Precondiciones { get; set; } = string.Empty;
    public string Pasos { get; set; } = string.Empty;
    public string ResultadoEsperado { get; set; } = string.Empty;
    public string Prioridad { get; set; } = "Media";
    public string CARef { get; set; } = string.Empty;
    public string Tipo { get; set; } = "Funcional";
    public string Estado { get; set; } = "Pendiente";
    public string Resultado { get; set; } = string.Empty;
    public string FechaEjecucion { get; set; } = string.Empty;
    public string Ejecutor { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;

    public Func<Task<(bool passed, string observation)>>? TestAction { get; set; }

    public async Task ExecuteAsync(string ejecutor)
    {
        Ejecutor = ejecutor;
        FechaEjecucion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        if (TestAction == null)
        {
            Estado = "No Implementado";
            Resultado = "N/A";
            Observaciones = "Prueba no automatizable o feature no implementado";
            return;
        }

        try
        {
            var (passed, obs) = await TestAction();
            Estado = "Ejecutado";
            Resultado = passed ? "PASÓ" : "FALLÓ";
            Observaciones = obs;
        }
        catch (Exception ex)
        {
            Estado = "Error";
            Resultado = "FALLÓ";
            Observaciones = $"Excepción: {ex.Message}";
        }
    }
}
