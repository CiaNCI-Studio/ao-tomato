namespace AoTomato.Domain.FunctionLogs.Mapping;

using AutoMapper;
using AoTomato.domain.FunctionLogs.Dtos;
using AoTomato.Domain.FunctionLogs.Models;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<FunctionLog, FunctionLogDto>().ReverseMap();
    }
}