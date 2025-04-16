using UserIdentity.Domain.Common;

namespace UserIdentity.Domain.Entities;

public class Role(string name, string? description = null) : BaseEntity
{
    public string Name { get; private set; } = name;
    public string? Description { get; private set; } = description;
    public DateTime CreatedAt { get; private set; }

    public void UpdateDescription(string description)
    {
        Description = description;
    }
}

