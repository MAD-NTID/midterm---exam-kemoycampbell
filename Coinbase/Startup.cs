using System.IO;
using Coinbase.Repositories;
using Coinbase.Services;
using Coinbase.Exceptions;
using Coinbase.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog;

namespace Coinbase
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
            //setting up the logger configuration
            string nLogPath = Directory.GetCurrentDirectory() + "/nlog.config";
            LogManager.LoadConfiguration(nLogPath);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("CoinbaseConnection");
            services.AddDbContextPool<DatabaseContext>(options => 
                options.UseMySql(connection, ServerVersion.AutoDetect(connection))
            );
            
            
            services.AddAuthentication("APIAuthenticationService")
                .AddScheme<AuthenticationSchemeOptions, APIAuthenticationService>("APIAuthenticationService", null);
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Coinbase", Version = "v1"}); });

            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddScoped<ICryptocurrencyRepository, CryptoCurrencyRepository>();
            services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Coinbase v1"));
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}