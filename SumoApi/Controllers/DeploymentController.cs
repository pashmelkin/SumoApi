using System;
using System.Net.Http;
using System.Threading.Tasks;
using Deployment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Deployment.Controllers
{
    [ApiController]
    [Route("deployment")]
    public class DeploymentController : ControllerBase
    {

        private readonly IDeploymentService _deploymentService;       
        private static HttpClient client;

        public DeploymentController(IDeploymentService deploymentService)
        {
            client = new HttpClient();
            _deploymentService = deploymentService;

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
            var res = await _deploymentService.GetDeployment(commitSha);
            return "hello";
        }
    }
    
}
