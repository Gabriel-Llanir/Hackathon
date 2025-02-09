using Prometheus;
using RegisterConsumer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using RegisterConsumer.Repositories;
using RegisterConsumer.Data;

namespace RegisterConsumer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<IRegisterRepository, RegisterRepository>();
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
