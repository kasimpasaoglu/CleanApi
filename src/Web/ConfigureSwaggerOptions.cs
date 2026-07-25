namespace Web;

public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        const string schemeId = "Bearer";
        // JWT Security Scheme (Authorize butonu için)
        options.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
        {
            Description = "Sadece JWT'yi girin (Swagger 'Bearer ' önekini otomatik ekler). Örn: eyJhbGciOi...",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        // Swashbuckle 10 + Microsoft.OpenApi yeni imza: delegate + SchemeReference
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            { new OpenApiSecuritySchemeReference(schemeId, document), [] }
        });

        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(desc.GroupName, new OpenApiInfo
            {
                Title = "Nova CRM API",
                Version = desc.ApiVersion.ToString()
            });
        }
    }
}