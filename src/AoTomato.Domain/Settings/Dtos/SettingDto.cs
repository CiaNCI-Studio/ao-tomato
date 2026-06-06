using System.Text.Json.Serialization;

namespace AoTomato.Domain.Settings.Dtos;

public class SettingDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("group")]
    public string Group { get; set; } = string.Empty;
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}