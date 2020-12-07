using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployment.Models
{
    public interface ISumoQueryService
    {
        Task<string> SearchForDeployments(string product, string env);
        Task<List<DeploymentDetails>> GetAllDeployments(string searchJobId);
        Task<bool> WaitJobIsReady(string searchJobId);
    }
}
