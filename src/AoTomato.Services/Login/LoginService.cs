namespace AoTomato.Services.Login;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AoTomato.domain.Login.Models;
using AoTomato.Domain.Helpers;
using AoTomato.Domain.Login.Abstractions.Services;
using AoTomato.Domain.Login.Dtos;
using AoTomato.Domain.Settings.Abstractions.Services;
using AoTomato.Domain.Users.Abstractions.Repositories;
using AoTomato.Domain.Users.Dtos;
using AoTomato.Domain.Users.Models;
using Microsoft.IdentityModel.Tokens;
using Serilog;

public class LoginService : ILoginService
{
    private readonly IUsersRepository usersRepository;
    private readonly AuthSettings authSettings;
    private readonly ISettingsService settingsService;
    private readonly ILogger logger;

    public LoginService(IUsersRepository usersRepository, AuthSettings authSettings, ISettingsService settingsService, ILogger logger)
    {
        this.usersRepository = usersRepository;
        this.authSettings = authSettings;
        this.settingsService = settingsService;
        this.logger = logger;
    }

    public async Task<bool> InitialCheck()
    {
        var users = await usersRepository.GetAllAsync();
        return !users.Any();
    }

    public async Task<bool> InitialSetup(InitialSetupDto initialSetupDto)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = initialSetupDto.UserName,
            Email = initialSetupDto.UserEmail,
            Password = initialSetupDto.UserPassword.HashSha1()
        };
        await usersRepository.InsertAsync(user, user.Id);
        return true;
    }

    public async Task<LoggedUserDto> Login(LoginDto loginDto)
    {
        var user = await usersRepository.GetByUsernameAsync(loginDto.Username);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }
        if (!user.Password.Equals(loginDto.Password.HashSha1()))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }
        return await GetLoginResponseAsync(user, loginDto.RemindMe);
    }

    private async Task<LoggedUserDto> GetLoginResponseAsync(User user, bool remind, bool expired = false)
    {
        await validateJwtConfig();
        var response = new LoggedUserDto
        {
            User = OpenClearUser(user)
        };
        var claimsdata = new[]
        {
             new  Claim("user", JsonSerializer.Serialize(response.User, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault})),
             new  Claim("key", authSettings?.ClientKey ?? string.Empty)
        };
        if (remind)
        {
            claimsdata = claimsdata.Append(new Claim("refresh_until", DateTime.UtcNow.AddDays(7).ToString())).ToArray();
        }
        else
        {
            claimsdata = claimsdata.Append(new Claim("refresh_until", DateTime.UtcNow.AddDays(1).ToString())).ToArray();
        }
        var expiry = DateTime.UtcNow.AddDays(1);
        if (expired)
        {
            expiry = DateTime.UtcNow.AddMinutes(-10);
        }
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings?.SecretKey ?? ""));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer: authSettings?.Issuer, audience: authSettings?.Audience,
            expires: expiry, signingCredentials: credentials, claims: claimsdata);
        var tokenHandler = new JwtSecurityTokenHandler();
        response.Token = tokenHandler.WriteToken(token);
        return response;
    }

    private UserDto OpenClearUser(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    private async Task validateJwtConfig()
    {
        var modified = false;
        if (string.IsNullOrEmpty(authSettings?.SecretKey))
        {
            authSettings!.SecretKey = Guid.NewGuid().ToString().HashSha1();
            logger.Warning("JWT SecretKey was not configured. A new key was generated for this instance");
            modified = true;
        }
        if (string.IsNullOrEmpty(authSettings?.ClientKey))
        {
            authSettings!.ClientKey = Guid.NewGuid().ToString().HashSha1();
            logger.Warning("JWT ClientKey was not configured. A new key was generated for this instance");
            modified = true;
        }
        if (string.IsNullOrEmpty(authSettings?.Audience))
        {
            authSettings!.Audience = "ao-tomato.com";
            logger.Warning("JWT Audience was not configured. A new key was generated for this instance");
            modified = true;
        }
        if (string.IsNullOrEmpty(authSettings?.Issuer))
        {
            authSettings!.Issuer = "ao-tomato.com";
            logger.Warning("JWT Issuer was not configured. A new key was generated for this instance");
            modified = true;
        }
        if (modified)
        {
            await settingsService.SaveByModelAsync(authSettings, null);
        }
    }
}