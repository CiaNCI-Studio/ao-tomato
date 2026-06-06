namespace AoTomato.domain.Login.Models;

public class AuthSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string ClientKey { get; set; } = string.Empty;
    public int ExpireDays { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}