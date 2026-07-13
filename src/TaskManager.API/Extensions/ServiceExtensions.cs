using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Application.Common;
using TaskManager.Application.Services;
using TaskManager.Application.Settings;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Services;

namespace TaskManager.API.Extensions;

public static class ServiceExtensions
{
     public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
     {
          services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
          services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
          services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IAppDbContext).Assembly));
          services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
          services.AddScoped<IJwtService, JwtService>();
          services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
          var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
          services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
          {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
               };
               options.MapInboundClaims = false;
          });
          return services;
     }
}