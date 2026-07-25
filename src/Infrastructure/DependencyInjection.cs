using Domain.Interfaces.Repositories;
using Infrastructure.Initialization;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Models;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var connectionString = ConnectionStringResolver.GetActiveConnectionString(configuration);

        // Interceptors
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntitySaveChangesInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditLogInterceptor>();

        // DbContext
        builder.Services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString, sql =>
                {
                    sql.CommandTimeout(1800); //30dk
                    sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); // Query execution stratejisini değiştirir, performans için eklenmiştir, risk yok, production güvenliği artar
                }
            );

            options.ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS));
        });


        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "CleanApi_session";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromDays(3);
                options.SlidingExpiration = true;
            });

        builder.Services.AddAuthorizationBuilder();

        // Identity
        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // Yetkiler
        builder.Services.AddAuthorization(options =>
        {
            //"CanCreateProduct" adında bir policy tanımla. Bu policy, "Administrator" rolüne sahip kullanıcıları kabul etsin.
            options.AddPolicy(Policies.CanCreateProduct, policy => policy.RequireRole(Roles.Administrator));
        });

        // IOptions Models
        builder.Services.Configure<FileStorageOptions>(configuration.GetSection("FileStorage"));
        builder.Services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

        

        #region Services & Factories Registrations

        // Factories
        builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppUserClaimsPrincipalFactory>();

        // Services
        builder.Services.AddScoped<IRequestCancellationClassifier, RequestCancellationClassifier>();
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddScoped<IIdentityService, IdentityService>();
        builder.Services.AddScoped<IDbInitializer, DbInitializer>();
        builder.Services.AddScoped<ICsvFileBuilderService, CsvFileBuilderService>();
        builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
        builder.Services.AddScoped<ISmtpEmailService, SmtpEmailService>();
        builder.Services.AddScoped<IAuditLogWriter, AuditLogWriter>();

        #endregion

        #region Repository Registrations
        
        builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        builder.Services.AddScoped<ISampleEntityRepository, SampleEntityRepository>();
        builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        builder.Services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));

        #endregion

        // TimeProvider
        builder.Services.AddSingleton(TimeProvider.System);


        return builder.Services;
    }
}