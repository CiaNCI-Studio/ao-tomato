using System.Text.Json.Serialization;
using AoTomato.Domain.Users.Dtos;

namespace AoTomato.Domain.Login.Dtos;

public class LoggedUserDto
{

    public LoggedUserDto()
    {
    }

    public LoggedUserDto(UserDto user, string token)
    {
        User = user;
        Token = token;
    }
    
    [JsonPropertyName("user")]
    public UserDto User { get; set; } = new UserDto();
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}