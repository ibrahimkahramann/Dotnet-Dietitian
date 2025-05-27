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
        if (context.Request.Path.StartsWithSegments("/api"))
            return JwtBearerDefaults.AuthenticationScheme;
        
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
});
// Add services to the container
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(); // Hot reload

// Antiforgery ayarları
builder.Services.AddAntiforgery(options => {
    options.HeaderName = "RequestVerificationToken";
    options.SuppressXFrameOptionsHeader = false;
    // Cookie'yi SameSiteStrict olarak ayarla - CSRF saldırılarına karşı ek koruma
    options.Cookie.SameSite = SameSiteMode.Strict;
    // Cookie güvenliğini artır
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddApplicationServices(builder.Configuration);

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dotnet-Dietitian API", Version = "v1" });
});

// SignalR ekleyin
builder.Services.AddSignalR();

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

// Middleware sıralaması kritiktir!
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dotnet-Dietitian API v1"));
}

//exception handler - en üstte olmalı!
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Kimlik doğrulama ve yetkilendirme middleware'leri sırasıyla çalışmalı
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