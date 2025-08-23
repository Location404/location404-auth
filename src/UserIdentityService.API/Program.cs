using Scalar.AspNetCore;
using Shared.Observability.Core;

using UserIdentityService.API.Endpoints;
using UserIdentityService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddUserIdentityInfra(builder.Configuration);
builder.Services.AddObservability(builder.Configuration);

var app = builder.Build();

app.MapOpenApi();
app.MapUserManagementEndpoints();

app.UseObservability();
app.UseHttpsRedirection();
app.MapScalarApiReference();

app.MapGet("/", () => Results.Redirect("/scalar")).ExcludeFromDescription();
app.UseExceptionHandler("/error");

app.Run();
