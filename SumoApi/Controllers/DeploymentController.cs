using System.Net.Http;
using System.Threading.Tasks;
using Deployment.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

       [HttpGet]
        public async Task<string> GetDeployment(string commitSha)
        {
            var res = await _deploymentService.GetDeployment(commitSha);
            string output = JsonConvert.SerializeObject(res);
            return output;
        }
    }
    
}
