namespace Infrastructure.Initialization;

public class DbInitializer(
    AppDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ILogger<DbInitializer> logger)
    : IDbInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var pending = await context.Database.GetPendingMigrationsAsync(cancellationToken);
            if (pending.Any())
                await context.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Veritabanı initialize edilirken hata oluştu.");
            throw;
        }
    }

    public async Task TrySeedAsync(string geoDataJsonPath, CancellationToken cancellationToken = default)
    {
        await new AdminSeeder(userManager, roleManager, context, logger).SeedAsync(cancellationToken);
    }
}

public static class DbInitializerExtensions
{
    public static async Task UseDbInitializerAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

        var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();


        await initializer.InitializeAsync();

        if (cfg.GetValue<bool>("SeedInitialData"))
        {
            var geoJsonPath = cfg.GetValue<string>("GeoSeed:JsonPath");
            if (string.IsNullOrWhiteSpace(geoJsonPath))
                throw new InvalidOperationException("GeoSeed:JsonPath missing");


            await initializer.TrySeedAsync(geoJsonPath);
        }
    }
}