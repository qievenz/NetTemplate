using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ReactTSWithNetCoreTemplate.API.Authentication;
using ReactTSWithNetCoreTemplate.API.BackgroundServices;
using ReactTSWithNetCoreTemplate.API.Middlewares;
using ReactTSWithNetCoreTemplate.API.Swagger;
using ReactTSWithNetCoreTemplate.Application.Services;
using ReactTSWithNetCoreTemplate.Core.Repositories;
using ReactTSWithNetCoreTemplate.Core.Services;
using ReactTSWithNetCoreTemplate.Core.Settings;
using ReactTSWithNetCoreTemplate.Infrastructure.Persistence;
using ReactTSWithNetCoreTemplate.Infrastructure.Repositories;
using System.Reflection;
using System.Text.Json.Serialization;

namespace ReactTSWithNetCoreTemplate.API.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var settingsTypes = Assembly.GetAssembly(typeof(AppSettings)).GetTypes()
                .Where(w => w.Name.EndsWith("Settings"))
                .ToList();

            foreach (var settingsType in settingsTypes)
            {
                var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
                    .GetMethod("Configure", new[] { typeof(IServiceCollection), typeof(IConfiguration) })
                    .MakeGenericMethod(settingsType);

                configureMethod.Invoke(null, new object[] { services, configuration.GetSection(settingsType.Name) });
            }

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "ReactTSWithNetCoreTemplate API", Version = "v1" });
                c.AddSecurityDefinition(ApiKeyAuthOptions.SchemeName, new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = $"Enter token below. Ex.: \"Bearer ABC123XYZ\""
                });
                c.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, "ReactTSWithNetCoreTemplate.API.xml"));

                c.OperationFilter<AuthorizeCheckOperationFilter>();
                c.OperationFilter<IFormFileOperationFilter>();
            });
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    if (!isDevelopment)
                        context.ProblemDetails.Detail = null;
                };
            });
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddApiKeyAuthentication(configuration);
            services.AddAuthorization();
            services.AddHealthChecks(configuration);

            services.AddScoped<IDataRepository, DataRepository>();

            services.AddScoped<IDataService, DataService>();
            services.AddSingleton<IChannelProcessorService<int>, ChannelProcessorService<int>>();

            services.AddHostedService<ChannelBackgroundService<int>>();

            return services;
        }

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {

            return services;
        }
    }
}
