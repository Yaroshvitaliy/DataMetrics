using System;
using System.IO;
using System.Linq;
using System.Reflection;
using App.Metrics;
using App.Metrics.Extensions.Configuration;
using App.Metrics.Formatters.Prometheus;
using DataMetrics.Infra;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;

namespace DataMetrics.Api.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => 
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DataMetricsConfig>(Configuration.GetSection("DataMetricsConfig"));

            services.AddSingleton<MetricsRegistry>();

            ConfigureMetrics(services);

            services.AddSingleton(resolver =>
            {
                var logger = resolver.GetService<ILogger>();
                var metrics = resolver.GetService<IMetrics>();
                var config = resolver.GetService<IOptions<DataMetricsConfig>>().Value;
                var dep = GlobalDependency.Create(logger, metrics, config);
                var compositionRoot = new CompositionRoot(dep);

                return compositionRoot;
            });

            services.AddControllers().AddMetrics();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "DataMetrics API",
                    Description = "API for writing metrics"
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CompositionRoot compositionRoot)
        {
            app.UseMetricsAllMiddleware();
            app.UseMetricsAllEndpoints();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DataMetrics API V1");
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.UseSerilogRequestLogging();

            compositionRoot.Init();
        }

        private void ConfigureMetrics(IServiceCollection services)
        {
            var metrics = AppMetrics.CreateDefaultBuilder()
                .Configuration.ReadFrom(Configuration)
                .Configuration.Configure(options => { })
                .OutputMetrics.AsPrometheusPlainText()
                .Build();

            services.AddMetricsTrackingMiddleware(Configuration);
            services.AddMetricsEndpoints(options =>
            {
                options.MetricsTextEndpointOutputFormatter =
                    App.Metrics.Metrics.Instance.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
            });
            services.AddMetricsReportingHostedService();
            //services.AddMetricsAuthorization();
            services.AddMetrics(metrics);
        }
    }
}