using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployment.Models
{
    public interface IDeploymentService
    {
        Task<List<DeploymentDetails>> GetDeployment(string commitId);
    }
}
