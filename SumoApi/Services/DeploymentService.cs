using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deployment.Models;

namespace Deployment.Service
{
    public class DeploymentService : IDeploymentService
    {
        private readonly ISumoQueryService sumoService;

        public DeploymentService(ISumoQueryService sumoService)
        {
            this.sumoService = sumoService;
          

        }

        public async Task<List<DeploymentDetails>> GetDeployment(string commitSha)
        {
            var searchJobId = await sumoService.SearchForDeployments();
            await sumoService.WaitJobIsReady(searchJobId);
            var deployments = await sumoService.GetAllDeployments(searchJobId);

            deployments.Sort((x, y) => DateTime.Compare(x.date, y.date));
            var prodDeps = deployments.FindAll(d => d.environment.ToLower().Equals("prod") && d.commitSha.ToLower().Equals(commitSha));

            if (prodDeps.Count() > 0)
            {
                return new List<DeploymentDetails>
                {
                   new DeploymentDetails {
                       commitSha = prodDeps.First().commitSha,
                        date = Convert.ToDateTime(prodDeps.First().date)}
                };
            }

            var results = new List<DeploymentDetails>();
            var devDeps = deployments.FindAll(d => d.environment.ToLower().Equals("dev") && d.commitSha.ToLower().Equals(commitSha));
            var startDate = Convert.ToDateTime(devDeps.First().date);

            deployments.ForEach(d => results.Add( new DeploymentDetails()
                {
                    commitSha = d.commitSha,
                    date = Convert.ToDateTime(d.date)
                }
            ));
            return results;
                                
            
           }
    }
}
