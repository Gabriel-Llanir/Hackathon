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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddFluentValidationAutoValidation()
                    .AddFluentValidationClientsideAdapters();

            services.AddValidatorsFromAssemblyContaining<UsuarioValidator>();

            services.AddSingleton<DataContext>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddSingleton(sp => new MongoDBMigrations(sp.GetRequiredService<DataContext>().Database));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MongoDBMigrations migrations)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseMetricServer();
            app.UseHttpMetrics();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            migrations.SeedDataAsync().Wait();
        }
    }
}
