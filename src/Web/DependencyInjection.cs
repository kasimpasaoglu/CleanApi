using Web.Services;

namespace Web;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {

        #region CORS

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CleanApi FrontEnd", policy =>
            {
                policy.WithOrigins(
                        "http://localhost:3000" // Next.js dev server
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); // Eğer frontend cookie veya authorization header gönderiyorsa
            });
        });

        #endregion

        builder.Services.AddSingleton<ICacheInvalidator, OutputCacheInvalidator>();


        builder.Services.AddHttpContextAccessor();

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>();

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        });

        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<CustomExceptionHandler>();

        builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"),
                    new QueryStringApiVersionReader("api-version")
                );
            }).AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; // v1, v2
                options.SubstituteApiVersionInUrl = true;
            });


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(); // doc’ları burada eklemiyoruz
        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    }
}