using System;
using System.Collections.Generic;
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

        public  async Task<List<(string commitId, string env, string date)>> GetDeployment(string commitSha)
        {
            var searchJobId = await sumoService.SearchForDeployments();
            await sumoService.WaitJobIsReady(searchJobId);
            var res = await sumoService.GetAllDeployments(searchJobId);

            return res;
        }
    }
}
