using System.Text.Json.Serialization;
using AoTomato.Domain.Functions.Enums;

namespace AoTomato.domain.Functions.Dtos;

public class FunctionDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    [JsonPropertyName("route")]
    public string Route { get; set; } = string.Empty;
    [JsonPropertyName("method")]
    public FunctionMethods Method { get; set; } = FunctionMethods.Get;
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
    [JsonPropertyName("apiKey")]
    public string ApiKey { get; set; } = string.Empty;
    [JsonPropertyName("cron")]
    public string Cron { get; set; } = string.Empty;
}