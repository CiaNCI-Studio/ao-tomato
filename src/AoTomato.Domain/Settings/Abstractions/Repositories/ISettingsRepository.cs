using AoTomato.Domain.Common.Abstractions.Repositories;
using AoTomato.Domain.Settings.Models;

namespace AoTomato.Domain.Settings.Abstractions.Repositories;

public interface ISettingsRepository : IRepositoryBase<Setting>
{
    Task<Setting> GetByKeyAsync(string key);
    Task<IEnumerable<Setting>> GetByGroupAsync(string group);
}