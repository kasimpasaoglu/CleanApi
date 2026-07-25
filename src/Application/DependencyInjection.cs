
namespace Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var autoMapperKey = builder.Configuration["Licensing:AutoMapper"];
        var mediatRKey = builder.Configuration["Licensing:MediatR"];

        builder.Services.AddAutoMapper(cfg =>
        {
            if (!string.IsNullOrWhiteSpace(autoMapperKey))
                cfg.LicenseKey = autoMapperKey;
        }, typeof(DependencyInjection).Assembly);

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        


        builder.Services.AddMediatR(cfg =>
        {
            if (!string.IsNullOrWhiteSpace(mediatRKey))
                cfg.LicenseKey = mediatRKey;
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));
        });
    }
}
