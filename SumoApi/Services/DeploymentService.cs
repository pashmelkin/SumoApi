using System;
using System.Collections.Generic;
using System.Linq;
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

        public  async Task<List<(string, DateTime)>> GetDeployment(string commitSha)
        {
            var searchJobId = await sumoService.SearchForDeployments();
            await sumoService.WaitJobIsReady(searchJobId);
            var deployments = await sumoService.GetAllDeployments(searchJobId);


            var prodDeps = deployments.FindAll(d => d.env.ToLower().Equals("prod") && d.commitId.ToLower().Equals(commitSha));
            var results = new List<(string, DateTime)>();

            if (prodDeps.Count() == 0)
            {
                deployments.ForEach(d => results.Add((d.commitId, Convert.ToDateTime(d.date))));
                return results;
                
            }
            else if (prodDeps.Count() == 1)
            {
                 results.Add(
                    (prodDeps.First().commitId,
                    Convert.ToDateTime(prodDeps.First().date)));
                return results;
                
            }
            
            return new List<(string, DateTime)>() { ("Error, many deployments found", new DateTime())};
        }
    }
}
