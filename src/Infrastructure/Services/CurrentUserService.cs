using System.Security;
using System.Text.RegularExpressions;

namespace Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor http) : ICurrentUserService
{
    private ClaimsPrincipal? User => http.HttpContext?.User;
    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? UserEmail => User?.FindFirstValue(ClaimTypes.Email);

    public string? UserBrach => User?.FindFirstValue("branch");
    public Guid? UserDepartmentId => ParseGuid("departmentId");
    public string? FullName => User?.FindFirstValue("fullName") ;
    
    public IReadOnlyCollection<string> UserRoles =>
        User?.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
        ?? Array.Empty<string>();
    
    public string? IpAddress
    {
        get
        {
            var context = http.HttpContext;
            if (context is null) return null;

            // Reverse proxy / load balancer varsa
            var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            return !string.IsNullOrWhiteSpace(forwarded) ? forwarded.Split(',')[0].Trim() : context.Connection.RemoteIpAddress?.ToString();
        }
    }

    public bool IsInRole(string role) =>
        !string.IsNullOrWhiteSpace(role) &&
        UserRoles.Contains(role, StringComparer.OrdinalIgnoreCase);

    public bool IsInAnyRole(params string[] roles) =>
        roles != null && roles.Any(IsInRole);
    
    private Guid? ParseGuid(string claimType)
    {
        var value = User?.FindFirstValue(claimType);
        if (string.IsNullOrWhiteSpace(value)) return null;

        return !Guid.TryParse(value, out var guid) ? throw new SecurityException($"Invalid GUID in claim '{claimType}'") : guid;
    }
}
