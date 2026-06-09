using AoTomato.API.Helpers;
using AoTomato.Domain.Functions.Abstractions.Services;
using AoTomato.Domain.Functions.Models;
using Cronos;

namespace AoTomato.API.Workers;

public class FunctionsWorker : BackgroundService
{
    private readonly IFunctionsService functionsService;
    private readonly Serilog.ILogger logger;

    public FunctionsWorker(Serilog.ILogger logger)
    {
        functionsService = ServiceProviderFactory.ServiceProvider!.GetRequiredService<IFunctionsService>();
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.Information("Functions worker started");
        var functions= new List<FunctionCron>();
        while (!stoppingToken.IsCancellationRequested)
        {
            var utcNow = DateTime.UtcNow;
            var newFunctions = await functionsService.GetCronAsync();
            functions.AddRange(newFunctions.Where((nf) => !functions.Any((f)=>f.Id == nf.Id)));
            foreach(var function in functions)
            {
                try
                {
                    if(newFunctions.Any((nf) => nf.Id == function.Id && nf.Cron != function.Cron))
                    {
                        function.Cron = newFunctions.First((nf) => nf.Id == function.Id).Cron;
                    }
                    var cronExpression = CronExpression.Parse(function.Cron);
                    if(function.NextOccourence == null)
                    {
                        function.NextOccourence = cronExpression.GetNextOccurrence(utcNow, TimeZoneInfo.Utc);
                    }
                    if(function.NextOccourence <= utcNow)
                    {
                        try
                        {
                            logger.Information("Next execution scheduled at: {Time} for function: {id}", function.NextOccourence.Value.ToLocalTime(), function.Id);
                            await functionsService.ExecuteFunctionCronAsync(function.Id);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, "An error occurred executing the background task.");
                        }
                        function.NextOccourence = cronExpression.GetNextOccurrence(utcNow, TimeZoneInfo.Utc);
                    }
                }
                catch (Exception ex)
                {
                     logger.Error(ex, "An error occurred executing the background task.");
                }
            }
            await Task.Delay(60000, stoppingToken); 
        }
        logger.Information("Functions worker stopped");
    }
}