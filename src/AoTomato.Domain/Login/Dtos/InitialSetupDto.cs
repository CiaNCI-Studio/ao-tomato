namespace AoTomato.Domain.Login.Dtos;


public class InitialSetupDto
{
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty;
}