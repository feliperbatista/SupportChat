using System;

namespace Domain.Entities;

public class ConversationCategory
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public Guid DepartmentId { get; private set; }
    public Department Department { get; private set; } = null!;

    private ConversationCategory() {}

    public static ConversationCategory Create(string name, Guid departmentId)
    {
        return new ConversationCategory
        {
            Name = name,
            DepartmentId = departmentId
        };
    }
}
