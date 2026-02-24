using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Paddokk.Api.Filters;
using Paddokk.Api.Security;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Services;
using Paddokk.Data;
using Paddokk.Data.Repositories;


namespace Paddokk.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
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

                // Configure for EdDSA with JWKS
                if (algorithm.Equals("EdDSA", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(jwksUri))
                {                    
                    var httpClient = new HttpClient();
                    var jwksJson = httpClient.GetStringAsync(jwksUri).Result;
                    
                    var jwks = JsonSerializer.Deserialize<JsonElement>(jwksJson);
                    
                    var keys = new List<SecurityKey>();
                    
                    if (jwks.TryGetProperty("keys", out var keysArray))
                    {
                        foreach (var key in keysArray.EnumerateArray())
                        {
                            var alg = key.GetProperty("alg").GetString();
                            var kty = key.GetProperty("kty").GetString();
                            
                            if (alg == "EdDSA" && kty == "OKP")
                            {
                                var x = key.GetProperty("x").GetString();
                                var kid = key.GetProperty("kid").GetString();
                                
                                var edDsaKey = EdDsaSecurityKey.FromJwk(x!, kid!);
                                keys.Add(edDsaKey);
                                
                            }
                        }
                    }
                    
                    if (keys.Count == 0)
                        throw new InvalidOperationException("No valid EdDSA keys found in JWKS");
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = keys,
                        ValidAlgorithms = ["EdDSA"],
                        ClockSkew = TimeSpan.Zero,
                        CryptoProviderFactory = new EdDsaCryptoProviderFactory()
                    };
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
                    options.RequireHttpsMetadata = false; // Only for development
                }
                // Configure token validation based on algorithm
                else if (algorithm.StartsWith("EdDSA", StringComparison.OrdinalIgnoreCase) || 
                    algorithm.StartsWith("ES", StringComparison.OrdinalIgnoreCase))
                {
                    // EdDSA or ECDSA - requires public key
                    if (string.IsNullOrEmpty(publicKey))
                        throw new InvalidOperationException($"Public Key is required for {algorithm} algorithm");
                                        
                    // For EdDSA, we need to use JsonWebKey
                    var jsonWebKey = new JsonWebKey(publicKey);
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = jsonWebKey,
                        // EdDSA uses specific algorithms
                        ValidAlgorithms = new[] { "EdDSA", "ES256", "ES384", "ES512" },
                        ClockSkew = TimeSpan.Zero
                    };

                    //// Add custom crypto provider for EdDSA
                    if (algorithm.StartsWith("EdDSA", StringComparison.OrdinalIgnoreCase))
                    {
                        options.TokenValidationParameters.CryptoProviderFactory = new EdDsaCryptoProviderFactory();
                    }
                }
                else if (algorithm.StartsWith("RS", StringComparison.OrdinalIgnoreCase))
                {
                    // RSA - requires public key
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

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Paddokk API",
                Version = "v1",
                Description = "The automotive journey sharing platform API"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header. Enter your token in the text input below. Example: \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

            c.OperationFilter<DefaultResponsesOperationFilter>();
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
