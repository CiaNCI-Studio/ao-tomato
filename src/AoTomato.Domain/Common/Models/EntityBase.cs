using AoTomato.Domain.Common.Enums;

namespace AoTomato.Domain.Common.Models;
public abstract class EntityBase
{
    public string Id { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
    
    public string CreatedBy { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;

    public string? DeletedBy { get; set; }

    public EntityStates State { get; set; } = EntityStates.Active;
}