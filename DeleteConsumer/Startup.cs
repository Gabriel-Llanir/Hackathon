using Prometheus;
using DeleteConsumer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using DeleteConsumer.Repositories;
using DeleteConsumer.Data;

namespace DeleteConsumer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<DataContext>();

            services.AddHostedService<Worker>();

            services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMetricServer();
            app.UseHttpMetrics();

            app.UseRouting();

            app.UseAuthorization();
        }
    }
}
