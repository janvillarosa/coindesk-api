using System.Net.Http;
using coinbaseapi.Hubs;
using coinbaseapi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace coinbaseapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(options =>
               options.AddPolicy("CorsPolicy",
                   builder =>
                       builder.AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithOrigins("http://localhost:3000")
                       .AllowCredentials()));
            services.AddScoped<HttpClient, HttpClient>();
            services.AddTransient<IApiService, ApiService>();
            services.AddMemoryCache();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<CoinHub>("/coinhub");
            });
        }
    }
}
