
using BankingApp.Application.Handlers;
using BankingApp.Application.Mappings;
using BankingApp.Application.Validators;
using BankingApp.Infrastructure.Database;
using BankingWebAPI.Application.Interfaces;
using BankingWebAPI.Infrastructure.Database;
using BankingWebAPI.Infrastructure.Integration;
using BankingWebAPI.Infrastructure.Middleware;
using BankingWebAPI.Infrastructure.RateLimiter;
using BankingWebAPI.Infrastructure.Repositories;
using BankingWebAPI.Infrastructure.Repostries;
using BankingWebAPI.Infrastructure.Services;
using BankingWebAPI.Infrastructure.Services.Interfaces;
using BankingWebAPI.Infrastructure.UnitOfWork;
using BankingWebAPI.Infrastruture.Redis;
using BankingWebAPI.Infrastruture.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;
using System.Text;

namespace BankingWebAPIMain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Host.UseSerilog((ctx, lc) =>
            {
                lc.ReadFrom.Configuration(ctx.Configuration);
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();


            builder.Services.AddDbContext<BankingDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));

            builder.Services.AddDbContext<BankingAuthDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("IRechargeAuthDB")));


            builder.Services.AddIdentityCore<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<BankingAuthDbContext>()
                .AddDefaultTokenProviders();


            var jwtKey = builder.Configuration["JwtSettings:Key"];
            var issuer = builder.Configuration["JwtSettings:Issuer"];
            var audience = builder.Configuration["JwtSettings:Audience"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });


            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "Banking API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });


            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "BankingApp_";
            });

            builder.Services.AddSingleton<ICacheService, RedisCacheService>();


            builder.Services.AddHttpClient<IPaymentGateway, PaystackService>(client =>
            {
                client.BaseAddress = new Uri("https://api.paystack.co");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            var mainDbConn = builder.Configuration.GetConnectionString("DefaultSQLConnection") ?? string.Empty;
            var authDbConn = builder.Configuration.GetConnectionString("IRechargeAuthDB") ?? string.Empty;
            var healthChecksUIConn = builder.Configuration.GetConnectionString("HealthChecksUI") ?? string.Empty;

            builder.Services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: mainDbConn,
                    name: "main-db",
                    healthQuery: "SELECT 1;",
                    tags: new[] { "database" })
                .AddSqlServer(
                    connectionString: authDbConn,
                    name: "auth-db",
                    healthQuery: "SELECT 1;",
                    tags: new[] { "database", "auth" });

            builder.Services.AddHealthChecksUI(options =>
            {
                options.AddHealthCheckEndpoint("Main DB", "/health/main-db");
                options.AddHealthCheckEndpoint("Auth DB", "/health/auth-db");
                options.SetEvaluationTimeInSeconds(30);
            })
       .AddInMemoryStorage();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });


            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IAccountingHistoryRepository, AccountingHistoryRepository>();


            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            builder.Services.AddScoped<IAccountTransfer, AccountTransfer>();
            builder.Services.AddScoped<IBankingService, BankingService>();
            builder.Services.AddScoped<IViewAccountBalance, ViewAccountBalance>();



            builder.Services.AddScoped<IAuthManager, AuthManager>();

            builder.Services.AddAutoMapper(typeof(MappinConfigs));


            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<AccountOpeningValidator>();

            builder.Services.AddSingleton<IRateLimiter>(new TokenBucketRateLimiter(maxRequests: 100, timeWindowInSeconds: 60));


            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetAccountTransactionHistoryQueryHandler>());



            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");


            app.UseMiddleware<RateLimitingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecks("/health/maindb", new HealthCheckOptions
            {
                Predicate = reg => reg.Tags.Contains("database"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecks("/health/authdb", new HealthCheckOptions
            {
                Predicate = reg => reg.Tags.Contains("auth"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecksUI(options =>
            {
                options.UIPath = "/health-ui";
                options.ApiPath = "/health-api";
            });

            app.MapControllers();
            app.Run();
        }
    }
}
