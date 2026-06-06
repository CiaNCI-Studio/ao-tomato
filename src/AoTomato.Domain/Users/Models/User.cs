using AoTomato.Domain.Common.Models;

namespace AoTomato.Domain.Users.Models;

public class User : EntityBase
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password  { get; set; } = string.Empty;
}
