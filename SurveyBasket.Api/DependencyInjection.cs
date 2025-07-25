using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SurveyBasket.Api.Authentication.Filters;
using SurveyBasket.Api.Health;

namespace SurveyBasket.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();

        services.AddHybridCache();

        services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
            )
        );


        services.AddAuthConfig(configuration);

        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
            options.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        services
            .AddSwaggerServices()
            .AddMapsterConfig()
            .AddFluentValidationConfig();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IRoleService, RoleService>();

        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IResultService, ResultService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserService, UserService>();

        services.AddBackgroundJobsConfig(configuration);
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddHttpContextAccessor();

        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        services.AddHealthChecks()
            .AddSqlServer(connectionString: connectionString, name: "database")
            .AddHangfire(options => { options.MinimumAvailableServers = 1; })
            .AddCheck<MailProviderHealthCheck>("mail service");

        services.AddRateLimitingConfig();


        return services;
    }

    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Survey Basket API",
                Version = "v1",
                Description = "API for Survey Basket",
                Contact = new OpenApiContact
                {
                    Name = "Survey Basket",
                    Url = new Uri("https://github.com/ziadhanii/Survey-Basket-API"),
                    Email = "ziadhani64@gmail.com"
                },
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Enter JWT Bearer token **_only_**",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        return services;
    }

    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection AddAuthConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience
                };
            });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });

        return services;
    }


    private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        services.AddHangfireServer();

        return services;
    }

    private static IServiceCollection AddRateLimitingConfig(this IServiceCollection services)
    {
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            rateLimiterOptions.AddPolicy(RateLimiters.IpLimiter, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromSeconds(20)
                    }
                )
            );

            rateLimiterOptions.AddPolicy(RateLimiters.UserLimiter, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.GetUserId(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromSeconds(20)
                    }
                )
            );

            rateLimiterOptions.AddConcurrencyLimiter(RateLimiters.Concurrency, options =>
            {
                options.PermitLimit = 1000;
                options.QueueLimit = 100;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            //rateLimiterOptions.AddTokenBucketLimiter("token", options =>
            //{
            //    options.TokenLimit = 2;
            //    options.QueueLimit = 1;
            //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //    options.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
            //    options.TokensPerPeriod = 2;
            //    options.AutoReplenishment = true;
            //});

            //rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
            //{
            //    options.PermitLimit = 2;
            //    options.Window = TimeSpan.FromSeconds(20);
            //    options.QueueLimit = 1;
            //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //});

            //rateLimiterOptions.AddSlidingWindowLimiter("sliding", options =>
            //{
            //    options.PermitLimit = 2;
            //    options.Window = TimeSpan.FromSeconds(20);
            //    options.SegmentsPerWindow = 2;
            //    options.QueueLimit = 1;
            //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //});
        });

        return services;
    }
}