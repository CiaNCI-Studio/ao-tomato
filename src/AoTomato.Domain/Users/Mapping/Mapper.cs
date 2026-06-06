namespace AoTomato.Domain.Users.Mapping;

using AoTomato.Domain.Users.Models;
using AutoMapper;
using AoTomato.Domain.Users.Dtos;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}