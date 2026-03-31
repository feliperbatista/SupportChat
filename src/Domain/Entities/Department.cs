using System;

namespace Domain.Entities;

public class Department
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public ICollection<Conversation> Conversations { get; private set; } = [];

    private Department() {}

    public static Department Create(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name is required", nameof(name));

        return new Department
        {
            Name = name
        };
    }

    public void UpdateDepartment(string name)
    {
        Name = name;
    }
}
