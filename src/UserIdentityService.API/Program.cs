using Scalar.AspNetCore;
using Shared.Observability.Core;

using UserIdentityService.API.Endpoints;
using UserIdentityService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddUserIdentityInfra(builder.Configuration);
builder.Services.AddOpenTelemetryObservability(builder.Configuration, options =>
{
    options.Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
});

var app = builder.Build();

app.MapOpenApi();
app.MapUserManagementEndpoints();
app.MapAuthenticationEndpoints();

app.UseHttpsRedirection();
app.MapScalarApiReference();

app.MapGet("/", () => Results.Redirect("/scalar")).ExcludeFromDescription();
app.UseExceptionHandler("/error");

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();