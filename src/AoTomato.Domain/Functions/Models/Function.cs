using AoTomato.Domain.Common.Models;
using AoTomato.Domain.Functions.Enums;

namespace AoTomato.Domain.Functions.Models;

public class Function : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public FunctionMethods Method { get; set; } = FunctionMethods.Get;
    public string Code { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Cron { get; set; } = string.Empty;
}