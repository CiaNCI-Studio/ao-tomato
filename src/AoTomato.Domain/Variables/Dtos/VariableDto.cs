using System.Text.Json.Serialization;

namespace AoTomato.Domain.Variables.Dtos;

public class VariableDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("readOnly")]
    public bool ReadOnly { get; set; } = false;
}