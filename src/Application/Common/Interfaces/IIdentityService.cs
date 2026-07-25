using System.Security.Claims;
using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IIdentityService
{
    Task<UserDto?> RegisterUserAsync(string firstName, string lastName, string phone,string email, string password, Guid departmentId);
    Task<UserDto?> CompleteRegistrationAsync(UserDto user, string oldPassword, string newPassword);
    Task<bool> DeleteUserAsync(string userId);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<IEnumerable<UserDto>> GetAllUsersIncludingDeletedAsync();
    Task<bool> IsUserWithEmailExistsAsync(string email);
    Task<Result<ClaimsPrincipal>> AuthenticateAsync(string email, string password);
    Task<Result<ClaimsPrincipal>> BuildPrincipalAsync(string userId);
    Task<UserDto?> GetUserInfoByIdAsync(string userId);
    Task<UserDto?> ResetPasswordAsync(string userId, string newPassword);
    Task<Result<IEnumerable<RoleDto>>> GetAllRolesAsync();
    Task<RoleDto?> CreateRoleAsync(string roleName, string description);
    Task<bool> DeleteRoleAsync(string roleId);
    Task<UserDto?> UpdateRolesAsync(string userId, IEnumerable<string> newRoleIds);
    Task<IReadOnlyDictionary<string, string?>> ResolveDisplayNamesAsync(IEnumerable<string?> userIds, CancellationToken ct);
    Task<string?> ResolveDisplayNameAsync(string? userId, CancellationToken ct);
    Task<Guid?> GetUserDepartmentIdAsync(string userId, CancellationToken ct);
    Task<List<UserDtoBase>> GetUsersByRolesAsync(IEnumerable<string> roleNames, CancellationToken ct);
    Task<Dictionary<string, string>> GetUsersByRoleAndBranchAsync(IEnumerable<string> roleNames, CancellationToken ct);
    Task<bool> IsUserExistsAsync(string userId, CancellationToken cancellationToken);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<Dictionary<string, UserDto>> GetUserInfoByIdsAsync(List<string> userIds, CancellationToken cancellationToken, bool includeDeleted = false);
    Task<List<string>> GetUserRolesAsync(string userId);
}
