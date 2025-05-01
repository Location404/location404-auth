using UserIdentity.API.Endpoints;
using UserIdentity.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddUserIdentityInfra(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapOpenApi();

app.MapUserEndpoints();
app.MapGet("/", () => "Hello World!");

app.Run();