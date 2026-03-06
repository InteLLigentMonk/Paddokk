using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Data;
using Paddokk.Api.Extensions;
using Paddokk.Api.Middleware;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApiVersioningV1();
builder.Services.AddValidation();
builder.Services.AddMediator();

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("writes", o =>
    {
        o.PermitLimit = 30;
        o.Window = TimeSpan.FromMinutes(1);
    });

    options.AddFixedWindowLimiter("reads", o =>
    {
        o.PermitLimit = 200;
        o.Window = TimeSpan.FromMinutes(1);
    });
});

// Database
builder.Services.AddDbContext<PaddokkDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//Azure Blob Storage
builder.Services.AddSingleton(x =>
{
    var connectionString = builder.Configuration.GetConnectionString("AzureStorage")
        ?? builder.Configuration["AzureStorage:ConnectionString"];
    return new BlobServiceClient(connectionString);
});

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);

// Application Services
builder.Services.AddApplicationServices();

// Swagger
builder.Services.AddOpenApiWithJwt();

// CORS
builder.Services.AddCorsPolicy();

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<PaddokkDbContext>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // serves /openapi/v1.json

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        _ = Task.Run(async () =>
        {
            try
            {
                using var client = new HttpClient();
                var url = app.Urls.FirstOrDefault() ?? "http://localhost:5158";
                var json = await client.GetStringAsync($"{url}/openapi/v1.json");
                var outputPath = Path.GetFullPath(
                    Path.Combine(app.Environment.ContentRootPath, "..", "..", "client", "swagger.json"));
                await File.WriteAllTextAsync(outputPath, json);
                app.Logger.LogInformation("OpenAPI spec saved to {Path}", outputPath);
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning("Failed to save OpenAPI spec: {Error}", ex.Message);
            }
        });
    });

    var devToken = app.Configuration["Development:BearerToken"];
    app.MapScalarApiReference(options =>
    {
        options.AddPreferredSecuritySchemes("Bearer");
        if (!string.IsNullOrEmpty(devToken))
            options.AddHttpAuthentication("Bearer", auth => auth.Token = devToken);
    });
}

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowNextJsApp");

app.UseRateLimiter();

app.UseAuthentication();
app.UseMiddleware<UserSyncMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PaddokkDbContext>();
    await context.Database.MigrateAsync();
}

app.Run();
