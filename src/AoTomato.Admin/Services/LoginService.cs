namespace AoTomato.Admin.Services;

using System.Text;
using System.Text.Json;
using AoTomato.Admin.Abstractions;
using AoTomato.Domain.Common.Dtos;
using AoTomato.Domain.Common.Enums;
using AoTomato.Domain.Exceptions;
using AoTomato.Domain.Login.Dtos;

public class LoginService : ILoginService
{
    protected HttpClient httpClient;

    public LoginService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<bool> InitialCheckAsync()
    {
        var response = await httpClient.GetAsync($"v1/login/initialCheck");
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response<bool>>(resultString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result?.Code == ResponseCode.OK)
            {
                return result.Payload;
            }
            else
            {
                throw new HttpRequestException(result?.Message);
            }
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            throw new DuplicatedException();
        }
        else
        {
            throw new HttpRequestException(response.ReasonPhrase);
        }
    }

    public async Task InitialSetupAsync(InitialSetupDto initialSetupDto)
    {
        var content = new StringContent(JsonSerializer.Serialize(initialSetupDto), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"/v1/login/initialSetup", content);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response<bool>>(resultString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result?.Code == ResponseCode.OK)
            {
                return;
            }
            else
            {
                throw new HttpRequestException(result?.Message);
            }
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            throw new DuplicatedException();
        }
        else
        {
            throw new HttpRequestException(response.ReasonPhrase);
        }
    }

    public async Task<LoggedUserDto?> Login(string username, string password, bool RemindMe = false)
    {
        var login = new LoginDto
        {
           Username = username,
           Password = password,
           RemindMe = RemindMe  
        };
        var content = new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"/v1/login", content);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response<LoggedUserDto>>(resultString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result?.Code == ResponseCode.OK)
            {
                return result.Payload;
            }
            else
            {
                throw new HttpRequestException(result?.Message);
            }
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            throw new DuplicatedException();
        }
        else
        {
            throw new HttpRequestException(response.ReasonPhrase);
        }
    }
}