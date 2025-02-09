using Prometheus;
using UpdateConsumer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using UpdateConsumer.Repositories;
using UpdateConsumer.Data;

namespace UpdateConsumer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUpdateService, UpdateService>();
            services.AddScoped<IUpdateRepository, UpdateRepository>();
            services.AddScoped<DataContext>();

            services.AddHostedService<Medicos_Worker>();
            services.AddHostedService<Pacientes_Worker>();
            services.AddHostedService<Consultas_Worker>();

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
