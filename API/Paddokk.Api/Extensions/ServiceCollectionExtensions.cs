using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Paddokk.Api.OpenApi;
using Paddokk.Api.Security;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Services;
using Paddokk.Data;
using Paddokk.Data.Repositories;


namespace Paddokk.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var jwtSettings = configuration.GetSection("BetterAuth:Jwt");
        var secretKey = jwtSettings["SecretKey"];
        var publicKey = jwtSettings["PublicKey"];
        var algorithm = jwtSettings["Algorithm"] ?? "HS256";
        var jwksUri = jwtSettings["JwksUri"];

        if (string.IsNullOrEmpty(secretKey) && string.IsNullOrEmpty(publicKey) && string.IsNullOrEmpty(jwksUri))
            throw new InvalidOperationException("JWT Secret Key, Public Key, or JWKS URI must be configured for BetterAuth");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError("JWT Authentication failed: {Error}", context.Exception.Message);
                        logger.LogError("Exception Type: {Type}", context.Exception.GetType().Name);
                        if (context.Exception.InnerException != null)
                            logger.LogError("Inner Exception: {Inner}", context.Exception.InnerException.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogInformation("JWT Token validated successfully for user: {User}",
                            context.Principal?.Identity?.Name ?? "Unknown");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("JWT Challenge triggered. Error: {Error}, ErrorDescription: {ErrorDescription}",
                            context.Error, context.ErrorDescription);
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        var token = context.Request.Headers["Authorization"].ToString();
                        logger.LogInformation("Received token: {HasToken}", !string.IsNullOrEmpty(token));
                        return Task.CompletedTask;
                    }
                };

                // EdDSA with JWKS is handled by EdDsaJwtBearerConfigureOptions (registered below)
                if (algorithm.Equals("EdDSA", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(jwksUri))
                {
                    // No-op here — keys are loaded lazily and asynchronously via IConfigureNamedOptions
                }
                // If JWKS URI is provided (non-EdDSA), use it to fetch keys automatically
                else if (!string.IsNullOrEmpty(jwksUri))
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.MetadataAddress = jwksUri;
                    options.RequireHttpsMetadata = !environment.IsDevelopment();
                }
                // Configure token validation based on algorithm
                else if (algorithm.StartsWith("EdDSA", StringComparison.OrdinalIgnoreCase) ||
                    algorithm.StartsWith("ES", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(publicKey))
                        throw new InvalidOperationException($"Public Key is required for {algorithm} algorithm");

                    var jsonWebKey = new JsonWebKey(publicKey);

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = jsonWebKey,
                        ValidAlgorithms = new[] { "EdDSA", "ES256", "ES384", "ES512" },
                        ClockSkew = TimeSpan.Zero
                    };

                    if (algorithm.StartsWith("EdDSA", StringComparison.OrdinalIgnoreCase))
                        options.TokenValidationParameters.CryptoProviderFactory = new EdDsaCryptoProviderFactory();
                }
                else if (algorithm.StartsWith("RS", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(publicKey))
                        throw new InvalidOperationException($"Public Key is required for {algorithm} algorithm");

                    var rsa = RSA.Create();
                    rsa.ImportFromPem(publicKey);
                    var rsaKey = new RsaSecurityKey(rsa);

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = rsaKey,
                        ClockSkew = TimeSpan.Zero
                    };
                }
                else
                {
                    // HS256/HS384/HS512 - symmetric key
                    var keyBytes = Encoding.UTF8.GetBytes(secretKey!);

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                        ClockSkew = TimeSpan.Zero
                    };
                }
            });

        // Register async JWKS loader for EdDSA — runs on first request, not at startup
        if (algorithm.Equals("EdDSA", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(jwksUri))
        {
            services.AddHttpClient();
            services.AddSingleton<IConfigureOptions<JwtBearerOptions>, EdDsaJwtBearerConfigureOptions>();
        }

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddOpenApiWithJwt(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            // Info is set via a document transformer in Microsoft.OpenApi v2
            options.AddDocumentTransformer((document, context, ct) =>
            {
                document.Info = new()
                {
                    Title = "Paddokk API",
                    Version = "v1",
                    Description = "The automotive journey sharing platform API"
                };
                return Task.CompletedTask;
            });

            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddOperationTransformer<DefaultResponsesOperationTransformer>();
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services here
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICarService, CarService>();
        services.AddScoped<IJourneyService, JourneyService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<ICommentService, CommentService>();

        // Register application repositories here
        services.AddScoped<ICarRepository, CarRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IJourneyRepository, JourneyRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection AddEmailServices(this IServiceCollection services)
    {
        // Register the Azure Email Service implementation
        services.AddScoped<IEmailService, AzureEmailService>();
        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowNextJsApp", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }
}
