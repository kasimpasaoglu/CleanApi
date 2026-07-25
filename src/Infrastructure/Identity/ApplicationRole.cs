namespace Infrastructure.Identity;

public class ApplicationRole : IdentityRole
{
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public RoleDto ToRoleDto()
    {
        return new RoleDto
        {
            Id = Id,
            Name = Name,
            Description = Description
        };
    }
}