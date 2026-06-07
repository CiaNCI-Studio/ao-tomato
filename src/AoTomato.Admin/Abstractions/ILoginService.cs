using AoTomato.Domain.Login.Dtos;

namespace AoTomato.Admin.Abstractions;

public interface ILoginService
{
    Task<LoggedUserDto?> Login(string username, string password, bool RemindMe = false);

    Task<bool> InitialCheckAsync();

    Task InitialSetupAsync(InitialSetupDto initialSetupDto);
}