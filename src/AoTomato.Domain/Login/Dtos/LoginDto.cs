using System.Text.Json.Serialization;

namespace AoTomato.Domain.Login.Dtos;

public class LoginDto
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    public bool RemindMe { get; set; } = false;
}