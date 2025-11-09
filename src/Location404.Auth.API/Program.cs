using Scalar.AspNetCore;
using Shared.Observability.Core;

using Location404.Auth.API.Endpoints;
using Location404.Auth.API.Middleware;
using Location404.Auth.Infrastructure;
using Location404.Auth.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddUserIdentityInfra(builder.Configuration);
builder.Services.AddOpenTelemetryObservability(builder.Configuration, options =>
{
    options.Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
});

builder.Services.AddObservabilityHealthChecks(builder.Configuration, checks =>
{
    var postgresConnection = builder.Configuration.GetConnectionString("UserIdentityDatabaseProduction");
    if (!string.IsNullOrEmpty(postgresConnection))
    {
        checks.AddNpgSql(postgresConnection, name: "postgres", tags: new[] { "ready", "db" }, timeout: TimeSpan.FromSeconds(3));
    }
});

var app = builder.Build();

// Aplicar migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<UserIdentityDbContext>();
        Console.WriteLine("🔄 Applying database migrations...");
        await context.Database.MigrateAsync();
        Console.WriteLine("✅ Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Database migration failed (service will still start): {ex.Message}");
    }
}

app.MapOpenApi();
app.MapUserManagementEndpoints();
app.MapAuthenticationEndpoints();
app.MapObservabilityHealthChecks();

app.UseHttpsRedirection();
app.MapScalarApiReference();

app.MapGet("/", () => Results.Redirect("/scalar")).ExcludeFromDescription();
app.UseExceptionHandler("/error");

app.UseMiddleware<ObservabilityMiddleware>();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();