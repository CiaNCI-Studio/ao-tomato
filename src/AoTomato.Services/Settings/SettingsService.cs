namespace AoTomato.Services.Settings;

using AoTomato.Domain.Login.Dtos;
using AoTomato.Domain.Settings.Abstractions.Repositories;
using AoTomato.Domain.Settings.Abstractions.Services;
using AoTomato.Domain.Settings.Dtos;
using AoTomato.Domain.Settings.Models;
using AutoMapper;
using Serilog;

public class SettingsService : ServiceBase<SettingDto, Setting>, ISettingsService
{
    private readonly ISettingsRepository settingsRepository;

    public SettingsService(ISettingsRepository settingsRepository, ILogger logger, IMapper mapper) : base(settingsRepository, logger, mapper)
    {
        this.settingsRepository = settingsRepository;
    }

    public async Task<Model> GetByModelAsync<Model>()
    {
        var settings = await settingsRepository.GetByGroupAsync(typeof(Model).Name);
        var model = Activator.CreateInstance<Model>();
        foreach (var setting in settings)
        {
            var property = typeof(Model).GetProperty(setting.Key);
            if (property != null && property.CanWrite)
            {
                var value = Convert.ChangeType(setting.Value, property.PropertyType);
                property.SetValue(model, value);
            }
        }
        return model;
    }

    public async Task<Model> SaveByModelAsync<Model>(Model model, LoggedUserDto? loggedUser)
    {
        var properties = typeof(Model).GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(model)?.ToString() ?? string.Empty;
            var setting = await settingsRepository.GetByKeyAsync(property.Name);
            if (setting != null)
            {
                setting.Value = value;
                await settingsRepository.UpdateAsync(setting, loggedUser?.User.Id ?? "System");
            }
            else
            {
                setting = new Setting
                {
                    Key = property.Name,
                    Value = value,
                    Group = typeof(Model).Name
                };
                await settingsRepository.InsertAsync(setting, loggedUser?.User.Id ?? string.Empty);
            }
        }
        return model;
    }
}
