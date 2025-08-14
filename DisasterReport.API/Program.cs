using DisasterReport.Data;
using DisasterReport.Services;
using DisasterReport.WebApi.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DisasterReport.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community; //for PDF export 

            // Add services to the container.
        

            // 1 . Read Connection string 
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            // 2 . Register Data Layer 
            builder.Services.AddDataServices(connectionString); //get from DataServiceControllerEntension... 
            // 3 .  Register Service Layer 
            builder.Services.AddServiceLayer(builder.Configuration);
            // Memory Cache
            builder.Services.AddMemoryCache();

            // ADD CORS CONFIGURATION
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:5173") // Your frontend URL
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials(); // Important for cookies
                });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                // 1. Define the Security Scheme
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token in this format - Bearer {your token here} to access this API"
                });

                // 2. Add a global Security Requirement
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            // 4. Configure Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    RoleClaimType = ClaimTypes.Role
                };

                //options.Events = new JwtBearerEvents
                //{
                //    OnMessageReceived = context =>
                //    {
                //        // Read token from cookie instead of header
                //        if (context.Request.Cookies.TryGetValue("access_token", out var token))
                //        {
                //            context.Token = token;
                //        }
                //        return Task.CompletedTask;
                //    }
                //};

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Try to get token from query string (for SignalR)
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        // This must match your hub endpoint path (case-sensitive)
                        var isHubRequest = path.StartsWithSegments("/hubs/notification");

                        if (!string.IsNullOrEmpty(accessToken) && isHubRequest)
                        {
                            context.Token = accessToken;
                        }
                        else if (context.Request.Cookies.TryGetValue("access_token", out var cookieToken))
                        {
                            context.Token = cookieToken;
                        }

                        return Task.CompletedTask;
                    }
                };

            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<NotificationHub>("/hubs/notification");

            app.Run();
        }
    }
}
