namespace AoTomato.Services.Users;

using AoTomato.Domain.Users.Abstractions.Repositories;
using AoTomato.Domain.Users.Abstractions.Services;
using AoTomato.Domain.Users.Dtos;
using AoTomato.Domain.Users.Models;
using AutoMapper;
using Serilog;

public class UsersService : ServiceBase<UserDto, User>, IUsersService
{
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository, ILogger logger, IMapper mapper) : base(usersRepository, logger,  mapper)
    {
        _usersRepository = usersRepository;
    }    
}