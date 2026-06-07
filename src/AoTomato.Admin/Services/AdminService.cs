using AoTomato.Admin.Abstractions;
using AoTomato.Domain.Login.Dtos;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace AoTomato.Admin.Services;

public class AdminService : IAdminService
{
    private const string LOGIN_KEY = "login";

    private const string SETTINGS_KEY = "settings";

    public event EventHandler<EventArgs>? Updated;

    private readonly ILocalStorageService localStorageService;

    private readonly NavigationManager navigationManager;
    
    private readonly ILoginService loginService;
    
    private LoggedUserDto? LoggedUser;

    public IUsersService UsersService {get; private set;}

    public IFunctionsService FunctionsService {get; private set;}

    public IVariablesService VariablesService {get; private set;}

    public ISettingsService SettingsService {get; private set;}

    public bool IsLoggedIn => LoggedUser != null;

    public string? Token => LoggedUser?.Token;

    public AdminService(ILoginService loginService,
                        IUsersService usersService,
                        ISettingsService settingsService,
                        IFunctionsService functionsService,
                        IVariablesService variablesService,
                        NavigationManager navigationManager,
                        ILocalStorageService localStorageService)
    {
        UsersService = usersService;
        SettingsService = settingsService;
        FunctionsService = functionsService;
        VariablesService = variablesService;
        this.loginService = loginService;
        this.navigationManager = navigationManager;
        this.localStorageService = localStorageService;
    }

    public async Task<bool> CheckLoginAsync()
    {
        var login = await localStorageService.GetItemAsync<LoggedUserDto>(LOGIN_KEY);
        if (login != null)
        {
            LoggedUser = login;
            try
            {
                await UsersService.GetById(LoggedUser.User.Id, LoggedUser.Token);
                return true;
            }
            catch (Exception)
            {
                await LogoutAsync();
                return false;
            }
        }
        else
        {
            await LogoutAsync();
            return false;
        }
    }


    public async Task<bool>  LoginAsync(string user, string password, bool remindMe = false)
    {
        try
        {
            LoggedUser = await loginService.Login(user, password, remindMe);
            await localStorageService.SetItemAsync(LOGIN_KEY, LoggedUser);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Login Error");
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        LoggedUser = null;
        await localStorageService.RemoveItemAsync(LOGIN_KEY);
    }

    public void NotifyUpdate()
    {
        Updated?.Invoke(this, EventArgs.Empty);
    }

    public async Task<bool> InitialCheckAsync()
    {
        return await loginService.InitialCheckAsync();
    }

    public async Task<bool> InitialSetupAsync(InitialSetupDto initialSetupDto)
    {
        try
        {
             await loginService.InitialSetupAsync(initialSetupDto);
             return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Initial setup Error");
            return false;
        }
    }
}