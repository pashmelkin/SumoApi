using System.Threading.Tasks;

namespace Deployment.Models
{
    public interface IDeploymentService
    {
        Task<DeploymentDetails> GetDeployment(string commitId);
    }
}
