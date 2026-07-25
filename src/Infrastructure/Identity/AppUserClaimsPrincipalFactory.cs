namespace Infrastructure.Identity;

public class AppUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<IdentityOptions> options)
    : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>(userManager, roleManager, options)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Name claim 
        identity.AddClaim(new Claim("fullName", $"{user.FirstName} {user.LastName}".Trim()));
        

        // department varsa departmentId claim
        if (user.DepartmentId.HasValue)
        {
            identity.AddClaim(
                new Claim("departmentId", user.DepartmentId.Value.ToString())
            );
        }

        return identity;
    }
}