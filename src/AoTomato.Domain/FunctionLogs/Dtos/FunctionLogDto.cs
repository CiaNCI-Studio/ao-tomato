using System.Text.Json.Serialization;

namespace AoTomato.domain.FunctionLogs.Dtos;

public class FunctionLogDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("functionId")]
    public string FunctionId { get; set; } = string.Empty;

    [JsonPropertyName("executionId")]
    public string ExecutionId { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("headers")]
    public string Headers { get; set; } = string.Empty;
    
    [JsonPropertyName("queryParameters")]
    public string QueryParameters { get; set; } = string.Empty;    

    [JsonPropertyName("result")]
    public string Result { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}