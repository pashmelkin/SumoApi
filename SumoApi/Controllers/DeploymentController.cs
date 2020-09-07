using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Deployment.Models;

namespace Deployment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeploymentController : ControllerBase
    {

        private readonly ILogger<DeploymentController> _logger;
        private readonly ISumoQueryService _sumoQueryService;
        private readonly IConfiguration config;
        private readonly Uri baseAddress;
       
        private static HttpClient client;

        public DeploymentController(ILogger<DeploymentController> logger, IConfiguration iConfig,
            ISumoQueryService sumoQueryService)
        {
            client = new HttpClient();
            _logger = logger;
            _sumoQueryService = sumoQueryService;
            config = iConfig;
            baseAddress = new Uri("https://api.au.sumologic.com/api/v1/search/jobs/");

            var authToken = Encoding.ASCII.GetBytes($"{AccessId}:{AccessKey}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(authToken));

            client.DefaultRequestHeaders.Add("Accept", "application/json");

        }

      [HttpGet]
        public async Task<string> GetRecords()
        {
            var address = new Uri("https://api.au.sumologic.com/api/v1/search/jobs/02253F48F19AD1A7/records?offset=0&limit=100");
            var response = await client.GetAsync(address);

            //will throw an exception if not successful
            //response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            return content;
            // return await Task.Run(() => JsonObject.Parse(content));
        }

       //[HttpGet]
        public async Task<string> Get(string jobId)
        {
            //var uri = baseAddress + new Uri("jobs/123"); messages?offset=0&limit=10
            var address = new Uri("https://api.au.sumologic.com/api/v1/search/jobs/02253F48F19AD1A7");
            var response = await client.GetAsync(address);

            //will throw an exception if not successful
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            return content;
            // return await Task.Run(() => JsonObject.Parse(content));
        }


       //[HttpGet]
        public async Task<string> GetDeployment(string commitSha)
        {
            var values = new Dictionary<string, string>
            {
                { "query", "_sourceCategory=\"/aws/release-prod\" and sme-web |" +
                " json field=_raw \"detail.env\" as env | json field=_raw \"detail.commit_id\" as commitId | json field=_raw \"time\"" +
                "| count by commitId, env, time" },
                { "from", "2020-09-01T12:00:00" },
                { "to", "2020-09-01T19:05:00" },
                { "timeZone", "IST" }
            };

            string id = string.Empty;
            var baseAddresspost = "https://api.au.sumologic.com/api/v1/search/jobs/";
            //string sumoAuth = config.GetSection("SumoAuth").Value;
            // use Ioptions


            client.DefaultRequestHeaders.Add("Accept", "application/json");


            //var response = await client.PostAsync(baseAddresspost, values);
            //
            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseAddresspost)))
            {
                var json = JsonConvert.SerializeObject(values);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;

                    using (var response = await client
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        string content = await response.Content.ReadAsStringAsync();
                        dynamic stuff = JsonConvert.DeserializeObject(content);
                        id = stuff.id;
                       
                    }
                }
            }



            return (id);
        }
    }
    
}
