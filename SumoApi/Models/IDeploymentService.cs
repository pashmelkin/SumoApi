using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployment.Models
{
    public interface IDeploymentService
    {
        Task<List<DeploymentAPI.Models.Deployment>> GetDeployment(string commitId);
    }
}
