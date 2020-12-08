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

        public DeploymentController(IDeploymentService deploymentService)
        {
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
