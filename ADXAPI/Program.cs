using ADXAPI.ScalarExtension;
using ADXService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ADXAPI
{
    public partial class Program { }
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi( options =>
            {
                options.UseJwtBearerAuthentication();

            });

            builder.Services.AddTransient<IADXAccess, ADXAccess>();
            builder.Services.AddTransient<IJWTGenerator, JWTGenerator>();

            var config = builder.Configuration;

            var key = Encoding.UTF8.GetBytes(config["JwtSettings:key"]);

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:Audience"],
                    //IssuerSigningKey = securityKey,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:key"]!)),
                    ValidateIssuer = true,
                    ValidateActor = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                  
                };

            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {              
                app.MapOpenApi();

                app.MapScalarApiReference();
            };

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
