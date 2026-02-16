using System.Windows;
using FifoCleanup.Engine.Services;
using FifoCleanup.Engine.Services.Interfaces;

namespace FifoCleanup.UI;

public partial class App : Application
{
    // Servicios compartidos (poor-man's DI para simplicidad en WPF low-power)
    public static IConfigurationService ConfigService { get; } = new ConfigurationService();
    public static IBitacoraService BitacoraService { get; } = new BitacoraService();
    public static IInventoryService InventoryService { get; } = new InventoryService();
    public static ICleanupService CleanupService { get; private set; } = null!;
    public static ISimulationService SimulationService { get; private set; } = null!;
    public static IScheduledCleanupService ScheduledService { get; private set; } = null!;
    public static IPreventiveMonitorService PreventiveService { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Inicializar servicios con dependencias
        CleanupService = new CleanupService(BitacoraService);
        SimulationService = new SimulationService(InventoryService, CleanupService);
        ScheduledService = new ScheduledCleanupService(InventoryService, CleanupService, BitacoraService);
        PreventiveService = new PreventiveMonitorService(InventoryService, CleanupService, BitacoraService);

        // Inicializar bitácora
        await BitacoraService.InitializeAsync("bitacora");

        // Manejo global de excepciones para evitar que errores en hilos de fondo cierren la app
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        // Detener servicios en segundo plano
        if (ScheduledService.IsRunning)
            await ScheduledService.StopAsync();
        if (PreventiveService.IsRunning)
            await PreventiveService.StopAsync();
        if (SimulationService.IsContinuousSimulationRunning)
            await SimulationService.StopContinuousSimulationAsync();

        base.OnExit(e);
    }

    private void App_DispatcherUnhandledException(object? sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        // Registrar y evitar cierre inmediato
        _ = BitacoraService.LogAsync(new FifoCleanup.Engine.Models.BitacoraEntry
        {
            EventType = FifoCleanup.Engine.Models.BitacoraEventType.Error,
            Action = "UNHANDLED_UI_EXCEPTION",
            Details = e.Exception.Message,
            Result = "ERROR"
        });

        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        // Registrar; no siempre se puede evitar el cierre
        var ex = e.ExceptionObject as Exception;
        _ = BitacoraService.LogAsync(new FifoCleanup.Engine.Models.BitacoraEntry
        {
            EventType = FifoCleanup.Engine.Models.BitacoraEventType.Error,
            Action = "UNHANDLED_DOMAIN_EXCEPTION",
            Details = ex?.Message ?? "Unknown",
            Result = "ERROR"
        });
    }

    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        _ = BitacoraService.LogAsync(new FifoCleanup.Engine.Models.BitacoraEntry
        {
            EventType = FifoCleanup.Engine.Models.BitacoraEventType.Error,
            Action = "UNOBSERVED_TASK_EXCEPTION",
            Details = e.Exception.Message,
            Result = "ERROR"
        });

        e.SetObserved();
    }
}
