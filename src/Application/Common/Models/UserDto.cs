namespace Application.Common.Models;

public class UserDto : UserDtoBase
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public bool IsCompletedRegistration { get; set; } 
    public int AccessFailedCount { get; set; }
    public bool IsLockedOut { get; set; } 
    public DateTimeOffset? LockoutEndDate { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}


public class UserDtoBase
{
    public string Id { get; set; } = null!;
    public string FullName { get; set; } = null!;
    
}