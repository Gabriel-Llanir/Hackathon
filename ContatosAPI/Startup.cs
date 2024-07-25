using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ContatosAPI.Data;
using ContatosAPI.Repositories;
using ContatosAPI.Services;
using ContatosAPI.Validators;
using ContatosAPI.Migrations;
using FluentValidation;

namespace ContatosAPI
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
