namespace AoTomato.Domain.Settings.Mapping;

using AoTomato.Domain.Settings.Models;
using AutoMapper;
using AoTomato.Domain.Settings.Dtos;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<Setting, SettingDto>().ReverseMap();
    }
}