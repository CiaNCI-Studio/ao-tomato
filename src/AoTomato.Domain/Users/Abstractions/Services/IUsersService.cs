namespace AoTomato.Domain.Users.Abstractions.Services;

using AoTomato.Domain.Common.Models;
using AoTomato.Domain.Users.Models;
using AoTomato.Domain.Users.Dtos;

public interface IUsersService : IServiceBase< UserDto,User>
{
}