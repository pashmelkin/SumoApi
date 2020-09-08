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
        private readonly IDeploymentService _deploymentService;
        private readonly IConfiguration config;
        private readonly Uri baseAddress;
       
        private static HttpClient client;

        public DeploymentController(ILogger<DeploymentController> logger, IConfiguration iConfig,
            IDeploymentService deploymentService)
        {
            client = new HttpClient();
            _logger = logger;
            _deploymentService = deploymentService;
            config = iConfig;

        }

        //[HttpGet]
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


       [HttpGet]
        public async Task<string> GetDeployment(string commitSha)
        {
            return "1";
        }
    }
    
}
