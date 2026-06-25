using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.API.Application.Services;
using TaskManager.API.Application.Settings;
using TaskManager.API.Infrastructure.Persistence;

namespace TaskManager.API.Extensions;

public static class ServiceExtensions
{
     public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
     {
          services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
          services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
          services.AddScoped<IJwtService, JwtService>();
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