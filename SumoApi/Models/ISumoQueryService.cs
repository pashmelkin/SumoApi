using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployment.Models
{
    public interface ISumoQueryService
    {
        Task<string> SearchForDeployments();
        Task<List<DeploymentDetails>> GetAllDeployments(string searchJobId);
        Task<bool> WaitJobIsReady(string searchJobId);
    }
}
