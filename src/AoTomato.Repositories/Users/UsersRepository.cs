namespace AoTomato.Repositories.Users;

using AoTomato.Domain.Common.Models;
using AoTomato.Domain.Users.Abstractions.Repositories;
using AoTomato.Domain.Users.Models;

public class UsersRepository : RepositoryBase<User>, IUsersRepository
{
    public UsersRepository(DbSettings settings) : base(settings, "users")
    {
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await Task.Run(() => collection.FindOne(item => item.Username == username));
    }

    public async Task<User> GetByUserEmailAsync(string email)
    {
        return await Task.Run(() => collection.FindOne(item => item.Email == email));
    }
}