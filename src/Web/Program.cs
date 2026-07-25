using System.Globalization;
using System.Runtime.ExceptionServices;
using Application;
using Infrastructure;
using Infrastructure.Initialization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Web;
using Web.Middleware;

var tr = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture = tr;
CultureInfo.DefaultThreadCurrentUICulture = tr;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
    o.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(30);
});

#region SeriLog

builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithProcessId()
    .Enrich.WithEnvironmentName()
    //.WriteTo.Console()
    .CreateLogger();

#endregion

#region Diagnostics — Process-level Exception Hooks

AppDomain.CurrentDomain.UnhandledException += (_, e) =>
{
    var ex = e.ExceptionObject as Exception;
    Log.Logger.Fatal(
        ex,
        "AppDomain UnhandledException. IsTerminating={IsTerminating} ThreadId={ThreadId} IsBackground={IsBackground}",
        e.IsTerminating,
        Environment.CurrentManagedThreadId,
        Thread.CurrentThread.IsBackground);
    Log.CloseAndFlush();
};

TaskScheduler.UnobservedTaskException += (_, e) =>
{
    Log.Logger.Fatal(
        e.Exception,
        "TaskScheduler UnobservedTaskException. ThreadId={ThreadId} IsBackground={IsBackground}",
        Environment.CurrentManagedThreadId,
        Thread.CurrentThread.IsBackground);
    e.SetObserved();
};

#endregion

#region Cache

builder.Services.AddOutputCache(opt =>
{
    opt.AddPolicy("revalidate-24h", new PublicCachePolicy(TimeSpan.FromHours(24)));
    opt.AddPolicy("revalidate-2h", new PublicCachePolicy(TimeSpan.FromHours(2)));
    opt.AddPolicy("revalidate-1h", new PublicCachePolicy(TimeSpan.FromHours(1)));
    opt.AddPolicy("revalidate-15m", new PublicCachePolicy(TimeSpan.FromMinutes(15)));
    opt.AddPolicy("revalidate-5m", new PublicCachePolicy(TimeSpan.FromMinutes(5)));
});

#endregion

builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

#region ForwardedHeaders

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor
        | ForwardedHeaders.XForwardedProto
        | ForwardedHeaders.XForwardedHost;
});

#endregion


#region DataProtection

if (builder.Environment.IsProduction()) //prod'ta data protection anahtarlarını dosya sistemine kaydet ve DPAPI ile koru, dev'de ise varsayılan (in-memory) kullan
{
    var dpSection = builder.Configuration.GetSection("DataProtection");
    var keysPath = dpSection.GetValue<string>("KeysPath");
    var appName = dpSection.GetValue<string>("ApplicationName") ?? "CleanApi";

    if (string.IsNullOrWhiteSpace(keysPath))
        throw new InvalidOperationException("DataProtection:KeysPath is not configured.");

    Directory.CreateDirectory(keysPath);

    var dp = builder.Services
        .AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
        .SetApplicationName(appName);

    if (OperatingSystem.IsWindows())
        dp.ProtectKeysWithDpapi(protectToLocalMachine: true);
}

#endregion


var app = builder.Build();

#region Diagnostics — Host Lifetime Hooks

app.Lifetime.ApplicationStopping.Register(() =>
{
    Log.Logger.Fatal(
        "ApplicationStopping triggered. ThreadId={ThreadId} IsBackground={IsBackground} Stack:\n{Stack}",
        Environment.CurrentManagedThreadId,
        Thread.CurrentThread.IsBackground,
        Environment.StackTrace);
});

app.Lifetime.ApplicationStopped.Register(() =>
{
    Log.Logger.Fatal("ApplicationStopped — final. Flushing Serilog buffers.");
    Log.CloseAndFlush();
});

#endregion


app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    var apiVersionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseCors("CleanApi FrontEnd");

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var desc in apiVersionProvider.ApiVersionDescriptions)
            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
    });


    app.UseExceptionHandler(new ExceptionHandlerOptions
    {
        ExceptionHandler = context =>
        {
            var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            // CustomExceptionHandler (Business/Validation) handle ettiyse zaten response yazıldı ve buraya düşmez.
            // Buraya düşen: handle edilmemiş gerçek exception -> tekrar fırlat, debugger durur.
            if (ex is not null)
                ExceptionDispatchInfo.Capture(ex).Throw();

            return Task.CompletedTask;
        }
    });
}
else
{
    app.UseExceptionHandler();
}

app.UseRouting();

await app.UseDbInitializerAsync();


app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<RequestAuditMiddleware>();

app.UseHealthChecks("/health");

app.UseOutputCache();

app.MapControllers();

Log.Information(
    "Application started — env={Env}, version={Version}, pid={Pid}",
    app.Environment.EnvironmentName,
    typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown",
    Environment.ProcessId);

app.Run();

Log.CloseAndFlush();