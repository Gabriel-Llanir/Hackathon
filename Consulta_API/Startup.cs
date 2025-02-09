using FluentValidation.AspNetCore;
using FluentValidation;
using Prometheus;
using Consulta.Data;
using Consulta.Validators;
using Consulta.Repositories;
using Consulta.Services;

namespace Consulta
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddFluentValidationAutoValidation()
                    .AddFluentValidationClientsideAdapters();

            services.AddValidatorsFromAssemblyContaining<MedicosValidator>();
            services.AddValidatorsFromAssemblyContaining<PacientesValidator>();

            services.AddSingleton<DataContext>();
            services.AddScoped<IConsultaRepository, ConsultaRepository>();
            services.AddScoped<IConsultaService, ConsultaService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseMetricServer();
            app.UseHttpMetrics();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
