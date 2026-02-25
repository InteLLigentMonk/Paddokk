using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Paddokk.Api.Security;

public class EdDsaJwtBearerConfigureOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public EdDsaJwtBearerConfigureOptions(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public void Configure(JwtBearerOptions options) =>
        Configure(JwtBearerDefaults.AuthenticationScheme, options);

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme) return;

        var jwksUri = _configuration["BetterAuth:Jwt:JwksUri"]!;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAlgorithms = ["EdDSA"],
            ClockSkew = TimeSpan.Zero,
            CryptoProviderFactory = new EdDsaCryptoProviderFactory()
        };

        // Fetch JWKS asynchronously using IHttpClientFactory (no .Result)
        options.Events ??= new JwtBearerEvents();
        var originalOnMessageReceived = options.Events.OnMessageReceived;

        options.Events.OnMessageReceived = async context =>
        {
            if (originalOnMessageReceived != null)
                await originalOnMessageReceived(context);

            if (options.TokenValidationParameters.IssuerSigningKeys?.Any() == true)
                return;

            var httpClient = _httpClientFactory.CreateClient();
            var jwksJson = await httpClient.GetStringAsync(jwksUri);
            var jwks = JsonSerializer.Deserialize<JsonElement>(jwksJson);
            var keys = new List<SecurityKey>();

            if (jwks.TryGetProperty("keys", out var keysArray))
            {
                foreach (var key in keysArray.EnumerateArray())
                {
                    if (key.GetProperty("alg").GetString() == "EdDSA" &&
                        key.GetProperty("kty").GetString() == "OKP")
                    {
                        var x = key.GetProperty("x").GetString()!;
                        var kid = key.GetProperty("kid").GetString()!;
                        keys.Add(EdDsaSecurityKey.FromJwk(x, kid));
                    }
                }
            }

            if (keys.Count == 0)
                throw new InvalidOperationException("No valid EdDSA keys found in JWKS");

            options.TokenValidationParameters.IssuerSigningKeys = keys;
        };
    }
}