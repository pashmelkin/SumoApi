using System;
using System.Threading.Tasks;

namespace Deployment.Models
{
    public interface IDeploymentService
    {
        Task<DateTime> GetDeployment(string commitId);
    }
}
