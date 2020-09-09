using System;
using System.Threading.Tasks;

namespace Deployment.Models
{
    public class DeploymentService : IDeploymentService
    {
        private readonly ISumoQueryService sumoService;

        public DeploymentService(ISumoQueryService sumoService)
        {
            this.sumoService = sumoService;
          

        }

        public  async Task<DateTime> GetDeployment(string commitId)
        {
            var res = await sumoService.GetListDeployments();

            return new DateTime();
        }
    }
}
