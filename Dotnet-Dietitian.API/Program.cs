using System.Text;
using Dotnet_Dietitian.Application.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Dotnet_Dietitian.API.Extensions;
using Microsoft.OpenApi.Models;
using Dotnet_Dietitian.Persistence.Data;
using Dotnet_Dietitian.API.Middlewares;
using MassTransit;
using Dotnet_Dietitian.Infrastructure.Consumers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "MultiScheme";
    options.DefaultAuthenticateScheme = "MultiScheme";
    options.DefaultChallengeScheme = "MultiScheme";
})
.AddPolicyScheme("MultiScheme", "Cookie or JWT", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        // If it's an API request but has a CSRF token header, use cookie auth
        if (context.Request.Path.StartsWithSegments("/api") && 
            context.Request.Headers.ContainsKey("RequestVerificationToken"))
        {
            return CookieAuthenticationDefaults.AuthenticationScheme;
        }
        
        // Otherwise follow the normal path-based selection
        // API paths can use both JWT and Cookie authentication
        // First try JWT, and if that fails, try Cookie
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            var authorization = context.Request.Headers["Authorization"].FirstOrDefault();
            return !string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer ") 
                ? JwtBearerDefaults.AuthenticationScheme 
                : CookieAuthenticationDefaults.AuthenticationScheme;
        }
        
        return CookieAuthenticationDefaults.AuthenticationScheme;
    };
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
})
.AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = JwtTokenDefaults.ValidIssuer,
        ValidAudience = JwtTokenDefaults.ValidAudience,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtTokenDefaults.Key)),
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
    };
    
    // This is important - allow cookies to be sent with JWT bearer token requests
    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Check for the JWT token in the cookie if it's not in the Authorization header
            if (string.IsNullOrEmpty(context.Token))
            {
                if (context.Request.Cookies.TryGetValue("jwt_token", out string? token) && token != null)
                {
                    context.Token = token;
                }
            }
            return Task.CompletedTask;
        }
    };
});

// MVC servislerini ekle
builder.Services.AddControllersWithViews();

builder.Services.AddApplicationServices(builder.Configuration);

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dotnet-Dietitian API", Version = "v1" });
});

// SignalR ekleyin
builder.Services.AddSignalR(options => 
{
    // Increase timeout values for better reconnection experience
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
});

// Configure SignalR user identification
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();

// MassTransit yapılandırma işlemleri - sadece production'da çalışsın
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddMassTransit(config =>
    {
        config.AddConsumer<MesajGonderildiConsumer>();
        config.AddConsumer<MesajOkunduConsumer>();
        
        config.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
            
            cfg.ConfigureEndpoints(context);
        });
    });
}
else
{
    // Development ortamında boş bir MassTransit yapılandırması
    builder.Services.AddMassTransit(config =>
    {
        config.UsingInMemory((context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
        });
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dotnet-Dietitian API v1"));
}

//exception handler
app.UseMiddleware<ExceptionMiddleware>();

app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// SignalR hub endpoint'inizi ekleyin - Namespace değişikliğine dikkat edin!
app.MapHub<Dotnet_Dietitian.Infrastructure.Hubs.MesajlasmaChatHub>("/mesajlasmahub");

// Seed the database
if (app.Environment.IsDevelopment())
{
    await SeedData.SeedAsync(app.Services);
}

app.Run();