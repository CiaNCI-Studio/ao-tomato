namespace AoTomato.Domain.Login.Abstractions.Services;

using AoTomato.Domain.Login.Dtos;

public interface ILoginService
{
    Task<bool> InitialCheck();
    Task<bool> InitialSetup(InitialSetupDto initialSetupDto);
    Task<LoggedUserDto> Login(LoginDto loginDto);
}