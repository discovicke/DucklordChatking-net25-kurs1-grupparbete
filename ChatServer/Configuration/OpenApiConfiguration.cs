using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace ChatServer.Configuration;

public static class OpenApiConfiguration
{
    public static void AddCustomOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            // 1. FIRST APPLY YOUR TRANSFORMER
            options.AddDocumentTransformer((document, context, ct) =>
            {
                // Ensure the document is not null
                document.Info ??= new();
                document.Components ??= new();
                document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
                document.SecurityRequirements ??= [];

                // Configure the document
                document.Info.Title = "Ducklord Chatking's Super Secure Server API Docs";
                document.Info.Version = "v0.0.2";

                const string schemeKey = "SessionAuth";

                // Ensure this overwrites any previous default scheme
                document.Components.SecuritySchemes[schemeKey] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "AuthSessionToken",
                    In = ParameterLocation.Header,
                    Description = "Session auth token which is normally returned in the server response when user logs in (/auth/login/)."
                };

                // Apply globally
                document.SecurityRequirements.Clear();
                document.SecurityRequirements.Add(new OpenApiSecurityRequirement
                {
                    { document.Components.SecuritySchemes[schemeKey], Array.Empty<string>() }
                });

                return Task.CompletedTask;
            });

            // 2. THEN APPLY SCALAR TRANSFORMERS LAST
            options.AddScalarTransformers();
        });
    }
}
