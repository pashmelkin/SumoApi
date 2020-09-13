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

        public  async Task<List<DeploymentAPI.Models.Deployment>> GetDeployment(string commitSha)
        {
            var searchJobId = await sumoService.SearchForDeployments();
            await sumoService.WaitJobIsReady(searchJobId);
            var deployments = await sumoService.GetAllDeployments(searchJobId);


            var prodDeps = deployments.FindAll(d => d.env.ToLower().Equals("prod") && d.commitId.ToLower().Equals(commitSha));
            var results = new List<DeploymentAPI.Models.Deployment>();

            if (prodDeps.Count() == 0)
            {
                deployments.ForEach(d => results.Add( new DeploymentAPI.Models.Deployment()
                    {
                        commitSha = d.commitId,
                        date = Convert.ToDateTime(d.date)
                    }
                ));
                return results;
                
            }

            results.Add(new DeploymentAPI.Models.Deployment()
            {
                commitSha = prodDeps.First().commitId,
                date = Convert.ToDateTime(prodDeps.First().date)
            });
                 
                return results;
                
            
           }
    }
}
