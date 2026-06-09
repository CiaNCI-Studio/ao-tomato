namespace AoTomato.Domain.Functions.Models;

public class FunctionCron
{
    public string Id { get; set; } = string.Empty;
    public string Cron { get; set; } = string.Empty;
    public DateTime? NextOccourence { get; set; }
}