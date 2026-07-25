

namespace Infrastructure.Initialization.Admin;

public class AdminSeeder(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    AppDbContext context,
    ILogger<DbInitializer> logger)
{
    private readonly List<ApplicationRole> _allRoles =
    [
        new() { Name = Roles.Administrator, Description = "Sistem Admin" },
    ];

    private readonly List<Department> _allDepartments =
    [
        new() { Name = "Bilgi Teknolojileri" },
    ];


    public async Task SeedAsync(CancellationToken ct = default)
    {
        foreach (var role in _allRoles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
                await roleManager.CreateAsync(role);
        }

        var departmentsToAdd = new List<Department>();

        foreach (var department in _allDepartments)
        {
            if (!await context.Departments.AnyAsync(d => d.Name == department.Name, cancellationToken: ct))
            {
                departmentsToAdd.Add(department);
            }
        }

        if (departmentsToAdd.Count > 0)
        {
            await context.Departments.AddRangeAsync(departmentsToAdd, ct);
            await context.SaveChangesAsync(ct);
        }

        var adminDepartment = await context.Departments
            .FirstOrDefaultAsync(x => x.Name == "Bilgi Teknolojileri", ct);

        if (adminDepartment == null)
            throw new InvalidOperationException("Admin departmani bulunamadi: Bilgi Teknolojileri");


        const string adminName = "Name";
        const string adminSurname = "Surname";
        const string adminPhone = "999999999";
        const string adminEmail = "admin@yourdomain.com";
        const string adminPassword = "Admin123*";


        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin != null)
        {
            logger.LogInformation("Admin kullanicisi zaten mevcut.");
            return;
        }

        var newAdminAccount = new ApplicationUser();
        newAdminAccount.InitRegistration(adminName, adminSurname, adminPhone, adminEmail, adminDepartment.Id);
        

        var result = await userManager.CreateAsync(newAdminAccount, adminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(newAdminAccount, Roles.Administrator);
        else
        {
            logger.LogError("Admin oluşturulamadı: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}