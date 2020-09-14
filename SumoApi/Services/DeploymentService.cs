using System;
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

        public async Task<DeploymentDetails> GetDeployment(string commitSha)
        {
            var searchJobId = await sumoService.SearchForDeployments();
            await sumoService.WaitJobIsReady(searchJobId);
            var deployments = await sumoService.GetAllDeployments(searchJobId);

            deployments.Sort((x, y) => DateTime.Compare(x.date, y.date));
            var prodDeps = deployments.FindAll(d => d.environment.ToLower().Equals("prod"));
            var commitDeployment = prodDeps.Find(d => d.commitSha.ToLower().Equals(commitSha));

            if (commitDeployment != null)
            {
                return new DeploymentDetails
                {
                       commitSha = commitDeployment.commitSha,
                        date = Convert.ToDateTime(commitDeployment.date),
                        environment = "production"
                };
            }

            var devDeps = deployments.Find(d => d.environment.ToLower().Equals("dev") && d.commitSha.ToLower().Equals(commitSha));
            var startDate = Convert.ToDateTime(devDeps.date);

            var firstProdDeployment = prodDeps.Find(d => DateTime.Compare(d.date, startDate) > 0);

            
            return firstProdDeployment;
            
           }
    }
}
