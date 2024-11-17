using ADXAPI.ScalarExtension;
using ADXService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Cryptography.X509Certificates;

namespace ADXAPI
{
    public class Program
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

            X509Certificate2 signingCert = X509CertificateLoader.LoadPkcs12FromFile($"{Environment.CurrentDirectory}{config["CertSettings:Path"]}", config["CertSettings:Password"]);
            X509SecurityKey securityKey = new X509SecurityKey(signingCert);

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
                    IssuerSigningKey = securityKey,
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
