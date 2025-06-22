using HoneyShop.Data;
using HoneyShop.Data.Repositories.Interfaces;
using HoneyShop.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using HoneyShop.Services.Interfaces;
using HoneyShop.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Identity;
using HoneyShop.Data.Entities;
using Microsoft.OpenApi.Models;

namespace HoneyShop
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IConfiguration _dbConfig;
        public Startup(IHostEnvironment hostEnv, IConfiguration configuration)
        {
            _dbConfig = new ConfigurationBuilder().SetBasePath(hostEnv.ContentRootPath).AddJsonFile("dbsettings.json").Build();
            _configuration = configuration;
        }

        // Add services to the container.

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<string>(_configuration.GetSection("Google"));
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(_dbConfig.GetConnectionString("DefaultConnection")));
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
                };
            });
            // Регистрация репозиториев
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
            services.AddScoped<IHoneyRepository, HoneyRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            // Регистрация сервисов
            services.AddScoped<IOrderStatusHistoryService, OrderStatusHistoryService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOrderService,  OrderService>();
            services.AddScoped<IHoneyService, HoneyService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://localhost:4200")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials());
            });

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c => {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "@ JWT Authorization",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement() 
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
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            services.AddAuthorization();

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")),
                RequestPath = "/uploads"
            });
            app.UseRouting();
            app.UseCors("AllowSpecificOrigin");
            app.UseSwagger();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
