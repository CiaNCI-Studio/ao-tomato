using AoTomato.Domain.Common.Models;

namespace AoTomato.Domain.Settings.Models;

public class Setting : EntityBase
{
    public string Group { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}