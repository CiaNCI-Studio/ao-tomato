namespace AoTomato.Domain.Functions.Mapping;

using AoTomato.Domain.Functions.Models;
using AutoMapper;
using AoTomato.domain.Functions.Dtos;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<Function, FunctionDto>().ReverseMap();
    }
}