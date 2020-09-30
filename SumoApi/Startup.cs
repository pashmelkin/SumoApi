using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Deployment.Models;
using Deployment.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Polly;

namespace Deployment
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
            services.AddScoped<ISumoQueryService, SumoQueryService>();
            services.AddScoped<IDeploymentService, DeploymentService>();
            services.AddMemoryCache();

            ConfigureHttpClient(services);
       
        }

        private void ConfigureHttpClient(IServiceCollection services)
        {
            var sumoAuth = new SumoAuth();
            Configuration.GetSection(SumoAuth.SumoAuthSection).Bind(sumoAuth);

            var authToken = Encoding.ASCII.GetBytes($"{sumoAuth.AccessID}:{sumoAuth.AccessKey}");

            var retryPolicy = Policy.Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(response => MyCustomResponsePredicate(response))
.           WaitAndRetryAsync(new[]
            {
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20)
            });

            services.AddHttpClient("SumoClient", client =>
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(authToken));

                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddPolicyHandler(retryPolicy);


        }

        private bool MyCustomResponsePredicate(HttpResponseMessage response)
        {
            if (response.RequestMessage.Method == HttpMethod.Post)
                return false;
            var content =  response.Content.ReadAsStringAsync().Result;
            dynamic result = JsonConvert.DeserializeObject(content);
            string state = Convert.ToString(result.state);
            return !(string.IsNullOrEmpty(state) || state.Equals("DONE GATHERING RESULTS"));

        }

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
