using AoTomato.API.Helpers;
using AoTomato.Domain.FunctionLogs.Abstractions.Services;

namespace AoTomato.API.Workers;

public class FunctionLogsWorker : BackgroundService
{
    private readonly IFunctionLogsService functionLogsService;
    private readonly Serilog.ILogger logger;

    public FunctionLogsWorker(Serilog.ILogger logger)
    {
        functionLogsService = ServiceProviderFactory.ServiceProvider!.GetRequiredService<IFunctionLogsService>();
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.Information("Function Logs worker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                 var beforeDate = DateTime.UtcNow.AddDays(-7);
                 await functionLogsService.DeleteByDateAsync(beforeDate);
            } catch (Exception ex)
            {
                    logger.Error(ex, "An error occurred executing the logs background task.");
            }
            await Task.Delay(60*60000, stoppingToken); 
        }
        logger.Information("FunctionLogs worker stopped");
    }
}