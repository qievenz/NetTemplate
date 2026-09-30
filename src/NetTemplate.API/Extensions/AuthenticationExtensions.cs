using Microsoft.AspNetCore.Authentication;
using NetTemplate.API.Authentication;

namespace NetTemplate.API.Extensions
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddApiKeyAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiKeyAuthOptions>(configuration.GetSection(nameof(ApiKeyAuthOptions)));

            return services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ApiKeyAuthOptions.SchemeName;
                options.DefaultChallengeScheme = ApiKeyAuthOptions.SchemeName;
            })
            .AddScheme<ApiKeyAuthOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthOptions.SchemeName, null);
        }
    }
}
