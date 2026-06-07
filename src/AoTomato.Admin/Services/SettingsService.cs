namespace AoTomato.Admin.Services;

using AoTomato.Admin.Abstractions;
using AoTomato.Domain.Settings.Dtos;
using Cordel.Client.Services;

public class SettingsService : ServiceBase<SettingDto>, ISettingsService
{
    public SettingsService(HttpClient httpClient) : base(httpClient, "/v1/Setting")
    {
    }
}