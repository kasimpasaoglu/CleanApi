namespace Infrastructure.Services;

public class IdentityService(
    RoleManager<ApplicationRole> roleManager,
    ISmtpEmailService smtpEmailService,
    UserManager<ApplicationUser> userManager,
    IAuthorizationService authorizationService,
    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
    IDateTimeProvider dateTimeProvider,
    AppDbContext context)
    : IIdentityService
{
    private const string SystemContactEmail = "info@yourdomain.com";


    public async Task<UserDto?> CompleteRegistrationAsync(UserDto user, string oldPassword, string newPassword)
    {
        var appUser = await userManager.Users
            .FirstOrDefaultAsync(x => x.Id == user.Id);
        if (appUser is null) return null;


        // Parola degistirme islemi
        var passwordResult = await userManager.ChangePasswordAsync(appUser, oldPassword, newPassword);
        if (!passwordResult.Succeeded)
        {
            var errors = string.Join(", ", passwordResult.Errors.Select(e => e.Description));
            return null;
        }

        appUser.SetIsCompletedRegistration();

        var updateResult = await userManager.UpdateAsync(appUser);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            return null;
        }

        var roles = await userManager.GetRolesAsync(appUser);

        return appUser.ToUserDto(roles);
    }


    public async Task<UserDto?> RegisterUserAsync(string firstName, string lastName, string phone, string email, string password, Guid departmentId)
    {
        var user = new ApplicationUser();
        user.InitRegistration(firstName, lastName, phone, email, departmentId);

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded) return null;

        var newUser = await GetUserInfoByIdAsync(user.Id);

        return newUser ?? null;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return false;

        await userManager.DeleteAsync(user);
        return true;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await userManager.Users
            .ToListAsync();

        var list = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            list.Add(user.ToUserDto(roles));
        }

        return list;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersIncludingDeletedAsync()
    {
        var users = await context.Users
            .IgnoreQueryFilters()
            .ToListAsync();

        var list = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            list.Add(user.ToUserDto(roles));
        }

        return list;
    }

    public async Task<bool> IsUserWithEmailExistsAsync(string email)
    {
        return await userManager.FindByEmailAsync(email) is not null;
    }

    public async Task<Result<ClaimsPrincipal>> AuthenticateAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result.Failure<ClaimsPrincipal>(Error.Unauthorized(ErrorCodes.InvalidCredentials, $"E-Posta Bulunamadı: {email}"));
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var lockoutEnd = user.LockoutEnd?.ToLocalTime().DateTime ?? dateTimeProvider.Now.LocalDateTime;
                var html = BuildLockoutEmailHtml(user.FirstName, user.LastName, dateTimeProvider.Now.LocalDateTime, lockoutEnd);

                await smtpEmailService.SendWithBccAsync(
                    user.Email,
                    [SystemContactEmail],
                    "CleanApi - Hesap Geçici Olarak Bloke Edildi",
                    html);
            }

            return Result.Failure<ClaimsPrincipal>(Error.Unauthorized(ErrorCodes.LockedOut, "Çok fazla hatalı giriş denemesi nedeniyle hesap geçici olarak kilitlendi."));
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
        {
            if (user.LockoutEnabled)
            {
                await userManager.AccessFailedAsync(user);
            }

            return Result.Failure<ClaimsPrincipal>(
                Error.Unauthorized(ErrorCodes.InvalidCredentials, $"E-Posta: {email}, Şifre Hatalı"));
        }

        if (user.AccessFailedCount > 0)
        {
            await userManager.ResetAccessFailedCountAsync(user);
        }

        var principal = await userClaimsPrincipalFactory.CreateAsync(user);

        return Result.Success(principal);
    }

    public async Task<Result<ClaimsPrincipal>> BuildPrincipalAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result.Failure<ClaimsPrincipal>(Error.NotFound(ErrorCodes.NotFound.User, $"Kullanıcı bulunamadı: {userId}"));
        }

        // AuthenticateAsync ile aynı claim factory — rol/claim'ler güncel DB durumundan üretilir.
        var principal = await userClaimsPrincipalFactory.CreateAsync(user);

        return Result.Success(principal);
    }

    public async Task<UserDto?> GetUserInfoByIdAsync(string userId)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);

        var result = user.ToUserDto(roles);

        return result;
    }

    public async Task<UserDto?> ResetPasswordAsync(string userId, string newPassword)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return null;
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded) return null;

        user.UnMarkRegistrationCompleted();
        await userManager.UpdateSecurityStampAsync(user);

        var roles = await userManager.GetRolesAsync(user);
        return user.ToUserDto(roles);
    }


    public async Task<Result<IEnumerable<RoleDto>>> GetAllRolesAsync()
    {
        var roles = await roleManager.Roles.ToListAsync();

        var result = roles.Select(role => role.ToRoleDto());

        return Result.Success(result);
    }

    private const string TickerId = "ticker";
    private const string TickerUserName = "Hatırlatıcı";

    public async Task<IReadOnlyDictionary<string, string?>> ResolveDisplayNamesAsync(IEnumerable<string?> userIds, CancellationToken ct)
    {
        var ids = userIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var dict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        // system’i doğrudan doldur
        if (ids.RemoveAll(id => id.Equals(TickerId, StringComparison.OrdinalIgnoreCase)) > 0)
            dict[TickerId] = TickerUserName;

        if (ids.Count == 0) return dict;

        var users = await GetUserInfoByIdsAsync(ids, ct, includeDeleted: true);
        foreach (var kvp in users)
            dict[kvp.Key] = kvp.Value.FullName;

        return dict;
    }

    public async Task<string?> ResolveDisplayNameAsync(string? userId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(userId)) return null;
        if (userId.Equals(TickerId, StringComparison.OrdinalIgnoreCase)) return TickerUserName;

        var u = await GetUserInfoByIdAsync(userId);
        return u?.FullName;
    }

    public async Task<Guid?> GetUserDepartmentIdAsync(string userId, CancellationToken ct)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null)
            throw new Exception($"User not found: {userId}");

        return user.DepartmentId;
    }

    public async Task<List<UserDtoBase>> GetUsersByRolesAsync(
        IEnumerable<string> roleNames,
        CancellationToken ct)
    {
        var roles = roleNames
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim())
            .ToList();

        if (roles.Count == 0)
            return [];

        // query syntax ile sorgu
        var query =
            from user in context.Users.AsNoTracking()
            join userRole in context.UserRoles on user.Id equals userRole.UserId
            join role in context.Roles on userRole.RoleId equals role.Id
            where roles.Contains(role.Name!)
            select new UserDtoBase
            {
                Id = user.Id,
                FullName = user.FullName ?? "Unknown",
            };

        var list = await query.Distinct().ToListAsync(ct);


        return list;
    }


    public async Task<bool> IsUserExistsAsync(string userId, CancellationToken cancellationToken)
    {
        return await userManager.Users.AnyAsync(x => x.Id == userId, cancellationToken);
    }


    public async Task<Dictionary<string, UserDto>> GetUserInfoByIdsAsync(List<string> userIds, CancellationToken cancellationToken, bool includeDeleted = false)
    {
        var users = includeDeleted
            ? await userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync(cancellationToken)
            : await userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync(cancellationToken);

        return users.ToDictionary(
            u => u.Id,
            u => u.ToUserDto());
    }

    public async Task<RoleDto?> CreateRoleAsync(string roleName, string description)
    {
        var role = new ApplicationRole
        {
            Name = roleName,
            Description = description
        };

        var result = await roleManager.CreateAsync(role);
        return !result.Succeeded ? null : role.ToRoleDto();
    }


    public async Task<bool> DeleteRoleAsync(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role is null) return false;
        var result = await roleManager.DeleteAsync(role);
        return result.Succeeded;
    }


    public async Task<UserDto?> UpdateRolesAsync(string userId, IEnumerable<string> newRoleIds)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null) return null;

        var currentRoles = await userManager.GetRolesAsync(user);

        var newRoleNames = await roleManager.Roles
            .Where(r => newRoleIds.Contains(r.Id))
            .Select(r => r.Name!)
            .ToListAsync();

        var rolesToAdd = newRoleNames.Except(currentRoles).ToList();
        var rolesToRemove = currentRoles.Except(newRoleNames).ToList();

        if (rolesToAdd.Any())
        {
            var addResult = await userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded) return null;
        }

        if (rolesToRemove.Any())
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded) return null;
        }

        var updatedUserRoles = await userManager.GetRolesAsync(user); // islem sonu kullanici rollerini al
        return user.ToUserDto(updatedUserRoles);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await userManager.FindByIdAsync(userId);

        return user != null && await userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await userClaimsPrincipalFactory.CreateAsync(user);

        var result = await authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }


    public async Task<List<string>> GetUserRolesAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return new List<string>();
        var roles = await userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    private static string BuildLockoutEmailHtml(string? firstName, string? lastName, DateTime attemptDate, DateTime lockoutEnd)
    {
        var fullName = WebUtility.HtmlEncode($"{firstName} {lastName}".Trim());
        if (string.IsNullOrWhiteSpace(fullName)) fullName = "Kullanıcı";

        var attemptDateText = WebUtility.HtmlEncode(attemptDate.ToString("dd.MM.yyyy HH:mm", new CultureInfo("tr-TR")));
        var lockoutEndText = WebUtility.HtmlEncode(lockoutEnd.ToString("dd.MM.yyyy HH:mm", new CultureInfo("tr-TR")));

        return $$"""
                 <!DOCTYPE html>
                 <html lang="tr">
                 <head>
                     <meta charset="UTF-8">
                     <meta name="viewport" content="width=device-width, initial-scale=1.0">
                     <title>Hesap Bloke Bildirimi</title>
                 </head>
                 <body style="margin:0;padding:0;background-color:#f4f6f8;font-family:'Segoe UI',Arial,sans-serif;color:#1f2937;">
                     <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:#f4f6f8;padding:32px 0;">
                         <tr>
                             <td align="center">
                                 <table role="presentation" width="600" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%;background-color:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,0.06);">
                                     <tr>
                                         <td style="background-color:#0f172a;padding:24px 32px;">
                                             <h1 style="margin:0;color:#ffffff;font-size:20px;font-weight:600;letter-spacing:0.3px;">CleanApi - Hesap Bloke Bildirimi</h1>
                                         </td>
                                     </tr>
                                     <tr>
                                         <td style="padding:32px;">
                                             <p style="margin:0 0 16px 0;font-size:16px;">Sayın <strong>{{fullName}}</strong>,</p>
                                             <p style="margin:0 0 16px 0;font-size:15px;line-height:1.6;">
                                                 <strong>{{attemptDateText}}</strong> tarihinde yapmış olduğunuz başarısız giriş denemeleri nedeniyle hesabınız
                                                 <strong>{{lockoutEndText}}</strong> tarihine kadar geçici olarak bloke edilmiştir.
                                             </p>
                                             <div style="background-color:#fef3c7;border-left:4px solid #f59e0b;padding:12px 16px;margin:20px 0;border-radius:4px;">
                                                 <p style="margin:0;font-size:14px;line-height:1.5;color:#78350f;">
                                                     Bu giriş denemelerini <strong>siz yapmadıysanız</strong>, lütfen derhal sistem yöneticinize başvurun.
                                                 </p>
                                             </div>
                                             <p style="margin:24px 0 0 0;font-size:13px;color:#6b7280;line-height:1.5;">
                                                 Bu bilgilendirme sistem tarafından otomatik olarak oluşturulmuştur.
                                                 <br>
                                                 Lütfen bu e-postayı yanıtlamayınız.
                                             </p>
                                         </td>
                                     </tr>
                                     <tr>
                                         <td style="background-color:#f9fafb;padding:16px 32px;border-top:1px solid #e5e7eb;">
                                             <p style="margin:0;font-size:12px;color:#9ca3af;text-align:center;">
                                                 &copy; YOUR BRAND
                                             </p>
                                         </td>
                                     </tr>
                                 </table>
                             </td>
                         </tr>
                     </table>
                 </body>
                 </html>
                 """;
    }

    public async Task<Dictionary<string, string>> GetUsersByRoleAndBranchAsync(IEnumerable<string> roleNames, CancellationToken ct)
    {
        var roles = roleNames
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim())
            .ToList();

        if (roles.Count == 0)
            return [];

        var query =
            from user in context.Users.AsNoTracking()
            join userRole in context.UserRoles on user.Id equals userRole.UserId
            join role in context.Roles on userRole.RoleId equals role.Id
            where roles.Contains(role.Name!)
            select new
            {
                user.Id,
                user.FullName
            };

        var list = await query.Distinct().ToListAsync(ct);

        return list.ToDictionary(x => x.Id!, x => x.FullName!);
    }
}