namespace AoTomato.Domain.Variables.Mapping;

using AoTomato.Domain.Variables.Models;
using AutoMapper;
using AoTomato.Domain.Variables.Dtos;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<Variable, VariableDto>().ReverseMap();
    }
}