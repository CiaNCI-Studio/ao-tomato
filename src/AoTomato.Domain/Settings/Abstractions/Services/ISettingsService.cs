namespace AoTomato.Domain.Settings.Abstractions.Services;

using AoTomato.Domain.Common.Models;
using AoTomato.Domain.Settings.Models;
using AoTomato.Domain.Settings.Dtos;
using AoTomato.Domain.Login.Dtos;

public interface ISettingsService : IServiceBase< SettingDto,Setting>
{
    Task<Model> GetByModelAsync<Model>();

    Task<Model> SaveByModelAsync<Model>(Model model, LoggedUserDto? loggedUser);
}