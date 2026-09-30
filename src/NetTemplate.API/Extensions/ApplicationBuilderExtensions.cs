using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using NetTemplate.Infrastructure.Persistence;
using Serilog;

namespace NetTemplate.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication AddUsings(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var connected = false;

                while (!connected)
                {
                    try
                    {
                        context.Database.Migrate();
                        connected = true;
                    }
                    catch (Exception ex)
                    {
                        Log.Warning($"Database is not ready yet: {ex.Message}. Retrying in 5 seconds...");
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseExceptionHandler();
            app.MapControllers();
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            return app;
        }
    }
}
