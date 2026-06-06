using AoTomato.Domain.Common.Models;

namespace AoTomato.Domain.Variables.Models;

public class Variable : EntityBase
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool ReadOnly { get; set; } = false;
}
