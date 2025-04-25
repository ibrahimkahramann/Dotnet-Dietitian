using Dotnet_Dietitian.API.Extensions;
using Microsoft.OpenApi.Models;
using Dotnet_Dietitian.Persistence.Data;
using Dotnet_Dietitian.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApplicationServices(builder.Configuration);

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dotnet-Dietitian API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dotnet-Dietitian API v1"));
}

//exception handler
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// Seed the database
if (app.Environment.IsDevelopment())
{
    await SeedData.SeedAsync(app.Services);
}

app.Run();