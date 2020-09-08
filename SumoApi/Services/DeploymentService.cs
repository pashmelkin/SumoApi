using System;
using Deployment.Models;

namespace DeploymentAPI.Controllers
{
    public class DeploymentService : IDeploymentService
    {
        private readonly ISumoQueryService sumoService;

        public DeploymentService(ISumoQueryService sumoService)
        {
            this.sumoService = sumoService;
          

        }

        public DateTime GetDeployment(string commitId)
        {
            sumoService.GetListDeployments();

            return new DateTime();
        }
    }
}
