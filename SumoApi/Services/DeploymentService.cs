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
            var searchJobId = await sumoService.SearchForDeployments();
            var res = await sumoService.GetAllDeployments(searchJobId);

            return new DateTime();
        }
    }
}
