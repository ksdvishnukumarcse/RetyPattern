using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using RetyPattern.Api.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RetyPattern.Api
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
            services.AddHttpClient<SampleController>(AppConstants.JsonPlaceFolderClientName, client =>
             {
                 client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
                 client.DefaultRequestHeaders.Add("Accept", "application/json");
                 client.DefaultRequestHeaders.Add("User-Agent", "retry-pattern-api-app");
             }).AddPolicyHandler(p =>
                    HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(3, attempt =>
                     {
                         //Tell the service about attempt number  
                         p.Headers.Remove("X-Retry");
                         p.Headers.Add("X-Retry", attempt.ToString());

                         //Set the wait interval  
                         return TimeSpan.FromSeconds(attempt * 3);
                     })
            );

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}
