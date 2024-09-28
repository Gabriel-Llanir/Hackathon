using Prometheus;
using DeleteConsumer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace DeleteConsumer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        => services.AddScoped<IUsuarioService, UsuarioService>();

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMetricServer();
            app.UseHttpMetrics();
        }
    }
}
