using UserIdentity.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapUserEndpoints();
app.MapGet("/", () => "Hello World!");
app.MapOpenApi();

app.UseHttpsRedirection();
app.Run();