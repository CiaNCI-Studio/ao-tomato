using AoTomato.Domain.Common.Abstractions.Repositories;
using AoTomato.Domain.Users.Models;

namespace AoTomato.Domain.Users.Abstractions.Repositories;

public interface IUsersRepository : IRepositoryBase<User>
{
    Task<User> GetByUsernameAsync(string username);

    Task<User> GetByUserEmailAsync(string email);
}