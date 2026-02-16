using corelink_server.Middleware;
using Corelink.Application;
using Corelink.Infrastructure;
using DotNetEnv;
using System.Text.Json.Serialization;

{
    var root = Directory.GetCurrentDirectory();
    var candidates = new[]
    {
        Path.Combine(root, ".env"),
        Path.Combine(root, "Corelink.Presentation", ".env")
    };

    foreach (var path in candidates)
    {
        if (File.Exists(path))
        {
            Env.Load(path);
            break;
        }
    }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: false));
    });

// Application (use-cases/services)
builder.Services.AddApplication();

// Postgres (Dapper/ADO.NET)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();