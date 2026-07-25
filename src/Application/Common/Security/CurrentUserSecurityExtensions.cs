using Application.Common.Exceptions;
namespace Application.Common.Security;

public static class CurrentUserSecurityExtensions
{
    public static readonly IReadOnlyList<string> DefaultElevatedRoles = new[]
    {
        Roles.Administrator,
    };

    public static void EnsureCanActAs(
        this ICurrentUserService currentUser,
        string targetUserId,
        IReadOnlyList<string>? elevatedRoles = null)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            throw new BusinessException(Error.Unauthorized(
                ErrorCodes.Unauthorized,
                "Kullanici oturum bilgisi okunamadi"));
        }

        if (string.Equals(currentUser.UserId, targetUserId, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var roles = elevatedRoles ?? DefaultElevatedRoles;
        if (currentUser.UserRoles.Any(r => roles.Any(er =>
                er.Equals(r, StringComparison.OrdinalIgnoreCase))))
        {
            return;
        }

        throw new BusinessException(Error.Forbidden(
            ErrorCodes.Forbidden,
            "Bu kullanicinin verisini goruntuleme yetkiniz yok"));
    }

    public static void EnsureCanActForBranch(
        this ICurrentUserService currentUser,
        string targetBranch,
        IReadOnlyList<string>? elevatedRoles = null)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            throw new BusinessException(Error.Unauthorized(
                ErrorCodes.Unauthorized,
                "Kullanici oturum bilgisi okunamadi"));
        }

        if (string.Equals(currentUser.UserBrach, targetBranch, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var roles = elevatedRoles ?? DefaultElevatedRoles;
        if (currentUser.UserRoles.Any(r => roles.Any(er =>
                er.Equals(r, StringComparison.OrdinalIgnoreCase))))
        {
            return;
        }

        throw new BusinessException(Error.Forbidden(
            ErrorCodes.Forbidden,
            "Bu subenin verisini goruntuleme yetkiniz yok"));
    }

    public static void EnsureCanActForDepartment(
        this ICurrentUserService currentUser,
        Guid targetDepartmentId,
        IReadOnlyList<string>? elevatedRoles = null)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            throw new BusinessException(Error.Unauthorized(
                ErrorCodes.Unauthorized,
                "Kullanici oturum bilgisi okunamadi"));
        }

        if (currentUser.UserDepartmentId.HasValue &&
            currentUser.UserDepartmentId.Value == targetDepartmentId)
        {
            return;
        }

        var roles = elevatedRoles ?? DefaultElevatedRoles;
        if (currentUser.UserRoles.Any(r => roles.Any(er =>
                er.Equals(r, StringComparison.OrdinalIgnoreCase))))
        {
            return;
        }

        throw new BusinessException(Error.Forbidden(
            ErrorCodes.Forbidden,
            "Bu departmanin verisini goruntuleme yetkiniz yok"));
    }

    public static void EnsureInAnyRole(
        this ICurrentUserService currentUser,
        params string[] allowedRoles)
    {
        if (string.IsNullOrWhiteSpace(currentUser.UserId))
        {
            throw new BusinessException(Error.Unauthorized(
                ErrorCodes.Unauthorized,
                "Kullanici oturum bilgisi okunamadi"));
        }

        if (allowedRoles.Length == 0)
        {
            throw new BusinessException(Error.Forbidden(
                ErrorCodes.Forbidden,
                "Bu islem icin izin verilen rol tanimlanmamis"));
        }

        if (currentUser.UserRoles.Any(r => allowedRoles.Any(ar =>
                ar.Equals(r, StringComparison.OrdinalIgnoreCase))))
        {
            return;
        }

        throw new BusinessException(Error.Forbidden(
            ErrorCodes.Forbidden,
            "Bu islemi yapma yetkiniz yok"));
    }
}
