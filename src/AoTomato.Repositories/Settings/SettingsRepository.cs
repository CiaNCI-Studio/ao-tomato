namespace AoTomato.Repositories.Settings;

using AoTomato.Domain.Common.Models;
using AoTomato.Domain.Settings.Abstractions.Repositories;
using AoTomato.Domain.Settings.Models;

public class SettingsRepository : RepositoryBase<Setting>, ISettingsRepository
{
    public SettingsRepository(DbSettings settings) : base(settings, "settings")
    {
    }

    public async Task<IEnumerable<Setting>> GetByGroupAsync(string group)
    {
        return await Task.Run(() => collection.Find(item => item.Group == group));
    }

    public async Task<Setting> GetByKeyAsync(string key)
    {
        return await Task.Run(() => collection.FindOne(item => item.Key == key));
    }
}
