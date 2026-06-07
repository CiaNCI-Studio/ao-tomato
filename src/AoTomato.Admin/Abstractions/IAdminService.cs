using AoTomato.Admin.Services;
using AoTomato.Domain.Login.Dtos;

namespace AoTomato.Admin.Abstractions;


public interface IAdminService
{
    event EventHandler<EventArgs> Updated;
    IUsersService UsersService {get;}
    IFunctionsService FunctionsService {get;}
    IVariablesService VariablesService {get;}
    ISettingsService SettingsService {get;}
    bool IsLoggedIn {get;}
    string? Token { get; }
    Task<bool> CheckLoginAsync();
    Task<bool> LoginAsync(string user, string password, bool remindMe);
    Task LogoutAsync();
    void NotifyUpdate();
    Task<bool> InitialCheckAsync();
    Task<bool> InitialSetupAsync(InitialSetupDto initialSetupDto);
}

