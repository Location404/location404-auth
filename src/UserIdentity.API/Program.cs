var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapGet("/", () => "Hello World!");

app.UseHttpsRedirection();
app.Run();