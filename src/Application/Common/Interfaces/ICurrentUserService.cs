namespace Application.Common.Interfaces;

/// <summary>
///     Token icindeki claimlere bakarak ve http context icinden user bilgilerini bulur. Veritabanindan cekmez.
///     Bu servis, uygulama genelinde kullanici bilgilerine erisim saglar.
/// </summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserEmail { get; }
    string? FullName { get; }
    IReadOnlyCollection<string> UserRoles { get; }
    string? UserBrach { get; }
    Guid? UserDepartmentId { get; }
    bool IsInRole(string role);
    bool IsInAnyRole(params string[] roles);
    string? IpAddress { get; }
}