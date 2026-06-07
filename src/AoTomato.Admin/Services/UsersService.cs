namespace AoTomato.Admin.Services;

using AoTomato.Admin.Abstractions;
using AoTomato.Domain.Users.Dtos;
using Cordel.Client.Services;

public class UsersService : ServiceBase<UserDto>, IUsersService
{
    public UsersService(HttpClient httpClient) : base(httpClient, "/v1/User")
    {
    }
}