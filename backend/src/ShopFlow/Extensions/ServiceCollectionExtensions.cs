using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShopFlow.Common;
using ShopFlow.DAL;
using ShopFlow.DAL.Repositories;
using ShopFlow.DAL.Repositories.Interfaces;
using ShopFlow.Services;
using ShopFlow.Services.Interfaces;
using ShopFlow.Web;

namespace ShopFlow.Extensions;

public static class ServiceCollectionExtensions
{
    public const string FrontendCorsPolicy = "ShopFlow.Frontend";

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found. " +
                "Set it with: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"<your connection string>\"");
        }

        services.AddDbContext<ShopFlowDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IShopService, ShopService>();
        services.AddScoped<ICourierService, CourierService>();

        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }

    public static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<FluentValidationFilter>();

        // AddProblemDetails is the fallback UseExceptionHandler() needs registered.
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        AddJwtAuthentication(services, configuration);
        AddSwagger(services);
        AddFrontendCors(services, configuration);

        // Deny by default: any action that is not explicitly marked [AllowAnonymous]
        // requires a signed-in user, so a forgotten attribute cannot expose an endpoint.
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        services.AddControllers(options =>
        {
            options.Filters.AddService<FluentValidationFilter>();
        })
        .AddJsonOptions(options =>
        {
            // Statuses travel as "OutForDelivery" rather than 1, so a client can read them.
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        // The validation filter already answers with ApiResponse, so the built-in
        // automatic 400 is turned off to keep a single error format.
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        return services;
    }

    private static void AddJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        var jwt = jwtSection.Get<JwtOptions>() ?? new JwtOptions();

        if (string.IsNullOrWhiteSpace(jwt.SigningKey) || Encoding.UTF8.GetByteCount(jwt.SigningKey) < 32)
        {
            throw new InvalidOperationException(
                "Jwt:SigningKey is missing or shorter than 32 bytes. " +
                "Set it with: dotnet user-secrets set \"Jwt:SigningKey\" \"<a long random string>\"");
        }

        services.Configure<JwtOptions>(jwtSection);
        services.AddSingleton<ITokenService, JwtTokenService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };

                    // A missing or expired token answers with JSON, so the front-end can
                    // show a banner instead of trying to parse an empty body.
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsJsonAsync(
                                ApiResponse.Fail("You need to sign in to continue."));
                        },
                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsJsonAsync(
                                ApiResponse.Fail("Your account does not have permission to open this page."));
                        }
                    };
                });
    }

    private static void AddFrontendCors(IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                      ?? new[] { "http://localhost:5173" };

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendCorsPolicy, policy => policy
                .WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod());
        });
    }

    private static void AddSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "ShopFlow API", Version = "v1" });

            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Paste the token returned by /api/account/login.",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = Array.Empty<string>() });
        });
    }
}
