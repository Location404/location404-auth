using UserIdentity.API.Endpoints;
using UserIdentity.Infra;
using ObservabilitySdk.Observability;
using ObservabilitySdk.AspNet;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddObservability(builder.Configuration, builder.Environment);
builder.Services.AddUserIdentityInfra(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapOpenApi();

app.MapUserEndpoints();
app.MapAuthenticationEndpoints();
app.UseObservability();

app.UseExceptionHandler("/error");

app.Run();