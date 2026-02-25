using Paddokk.Data;
using Paddokk.Api.Extensions;
using Paddokk.Api.Middleware;
using Azure.Communication.Email;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Azure Communication Services Email
builder.Services.AddSingleton<EmailClient>(serviceProvider =>
{
    var connectionString = builder.Configuration["AzureEmail:ConnectionString"];
    return new EmailClient(connectionString);
});

// Database
builder.Services.AddDbContext<PaddokkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Email Service
builder.Services.AddEmailServices();

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
