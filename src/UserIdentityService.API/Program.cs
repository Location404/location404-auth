using UserIdentityService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddUserIdentityInfra(builder.Configuration);

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();

app.Run();
