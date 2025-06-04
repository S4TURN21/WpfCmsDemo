using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Remake.WebApi.Entities.Models;
using Remake.WebApi.Services;
using System.Text;

namespace Remake.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DbContext
            builder.Services.AddDbContext<ApplicationContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                if (connectionString == null)
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                }

                options.UseSqlServer(connectionString);
            });

            // Add Identity
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 7;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;

                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationContext>();

            // Add Authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["validIssuer"],
                    ValidAudience = jwtSettings["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["securityKey"]))
                };
            });


            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(Program));

            // Add Services
            builder.Services.AddSingleton<ITokenService, TokenService>();

            // Add Controllers
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var securityDefinition = new OpenApiSecurityScheme
                {
                    Name = "Authentication",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer"
                };

                options.AddSecurityDefinition("Bearer", securityDefinition);

                var securityScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                var securityRequirements = new OpenApiSecurityRequirement
                {
                    { securityScheme, new string[] {} }
                };

                options.AddSecurityRequirement(securityRequirements);

                options.IncludeXmlComments(Path.Combine(builder.Environment.ContentRootPath, "Remake.WebApi.xml"), includeControllerXmlComments: true);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
